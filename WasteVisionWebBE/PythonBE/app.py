# filepath: app.py
from flask import Flask, request, jsonify
from werkzeug.utils import secure_filename
import os
import json
import cv2
from ultralytics import YOLO
import base64
import numpy as np
from PIL import Image
import io

app = Flask(__name__)
app.config['UPLOAD_FOLDER'] = 'uploads'
app.config['MAX_CONTENT_LENGTH'] = 16 * 1024 * 1024  # 16MB max file size

# Ensure upload folder exists (still needed for /detect-file)
os.makedirs(app.config['UPLOAD_FOLDER'], exist_ok=True)

# Global model, loaded at startup (e.g., for health checks or other endpoints)
default_model = None

def load_default_model():
    global default_model
    try:
        # This is the path for the default model
        model_path = './models/best.pt'
        if not os.path.exists(model_path):
            # Create directory if it doesn't exist, but model file should be present
            os.makedirs(os.path.dirname(model_path), exist_ok=True)
            print(f"Default model path {model_path} not found. Please ensure the model file exists.")
            return

        default_model = YOLO(model_path)
        print(f"Default YOLO model loaded successfully from {model_path}")
    except Exception as e:
        print(f"Error loading default model: {e}")
        default_model = None # Ensure it's None if loading failed

def decode_base64_image(base64_string):
    """Decode base64 string to image"""
    try:
        if ',' in base64_string:
            base64_string = base64_string.split(',')[1]
        img_data = base64.b64decode(base64_string)
        img = Image.open(io.BytesIO(img_data))
        img_array = np.array(img)
        if len(img_array.shape) == 3: # Ensure it's a color image
            img_array = cv2.cvtColor(img_array, cv2.COLOR_RGB2BGR)
        return img_array
    except Exception as e:
        print(f"Error decoding base64 image: {e}")
        return None

def process_yolo_results(results, img_width, img_height):
    """Convert YOLO results to API format"""
    predictions = []
    for result in results:
        if result.boxes is not None:
            for box in result.boxes:
                x1, y1, x2, y2 = box.xyxy[0].cpu().numpy()
                center_x = (x1 + x2) / 2
                center_y = (y1 + y2) / 2
                width = x2 - x1
                height = y2 - y1
                class_id = int(box.cls[0].cpu().numpy())
                confidence = float(box.conf[0].cpu().numpy())
                class_name = result.names[class_id]
                prediction = {
                    "x": float(center_x),
                    "y": float(center_y),
                    "width": float(width),
                    "height": float(height),
                    "confidence": confidence,
                    "class": class_name,
                    "class_id": class_id,
                    "detection_id": f"yolo_{len(predictions)}"
                }
                predictions.append(prediction)
    return {"predictions": predictions}

@app.route('/health', methods=['GET'])
def health_check():
    """Health check endpoint"""
    return jsonify({
        "status": "healthy",
        "default_model_loaded": default_model is not None
    })

@app.route('/detect', methods=['POST'])
def detect_objects():
    """Object detection endpoint that uses a model path from the request."""
    try:
        # Get model_path from form data
        model_path_from_request = request.form.get('model_path')
        if not model_path_from_request:
            return jsonify({"error": "model_path form field is required"}), 400

        # Check if the model file exists
        if not os.path.exists(model_path_from_request):
            return jsonify({"error": f"Model file not found at path: {model_path_from_request}"}), 400

        # Load the specified YOLO model for this request
        try:
            current_yolo_model = YOLO(model_path_from_request)
            print(f"Using model for this request: {model_path_from_request}")
        except Exception as e:
            print(f"Error loading model from path {model_path_from_request}: {e}")
            return jsonify({"error": f"Failed to load model '{model_path_from_request}': {str(e)}"}), 500

        img_array = None
        # Check for file upload (expected from C# client)
        if 'file' in request.files:
            file = request.files['file']
            if file.filename == '':
                return jsonify({"error": "No file selected"}), 400
            
            # Read image bytes directly from the uploaded file stream
            img_bytes = file.read()
            nparr = np.frombuffer(img_bytes, np.uint8)
            img_array = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
            
            if img_array is None:
                return jsonify({"error": "Could not decode image from uploaded file"}), 400
        
        # Fallback for other content types (less likely if C# client is primary)
        elif request.content_type == 'application/json':
            data = request.get_json()
            if 'image' in data:
                img_array = decode_base64_image(data['image'])
        elif request.content_type == 'application/x-www-form-urlencoded':
            base64_data = request.get_data(as_text=True)
            img_array = decode_base64_image(base64_data)
        
        if img_array is None:
            return jsonify({"error": "Invalid or missing image data"}), 400
        
        img_height, img_width = img_array.shape[:2]
        
        # Run YOLO inference with the request-specific model
        results = current_yolo_model(img_array, conf=0.25)
        
        response_data = process_yolo_results(results, img_width, img_height)
        return jsonify(response_data)
        
    except Exception as e:
        # Log the full error for debugging on the server
        import traceback
        print(f"Error in /detect endpoint: {str(e)}")
        traceback.print_exc()
        return jsonify({"error": f"Detection failed due to an internal server error: {str(e)}"}), 500

@app.route('/detect-file', methods=['POST'])
def detect_objects_file():
    """File upload detection endpoint (uses default_model)"""
    try:
        if 'file' not in request.files:
            return jsonify({"error": "No file provided"}), 400
        
        file = request.files['file']
        if file.filename == '':
            return jsonify({"error": "No file selected"}), 400
        
        if default_model is None: # Uses the globally loaded default model
            return jsonify({"error": "Default model not loaded"}), 500
        
        filename = secure_filename(file.filename)
        filepath = os.path.join(app.config['UPLOAD_FOLDER'], filename)
        file.save(filepath)
        
        try:
            img_array = cv2.imread(filepath)
            if img_array is None:
                return jsonify({"error": "Invalid image file"}), 400
            
            img_height, img_width = img_array.shape[:2]
            
            # Run detection using the default_model
            results = default_model(filepath, conf=0.25) 
            response = process_yolo_results(results, img_width, img_height)
            
            return jsonify(response)
            
        finally:
            if os.path.exists(filepath):
                os.remove(filepath)
                
    except Exception as e:
        import traceback
        print(f"Error in /detect-file endpoint: {str(e)}")
        traceback.print_exc()
        return jsonify({"error": f"Detection failed: {str(e)}"}), 500

if __name__ == '__main__':
    load_default_model() # Load the default model at startup
    app.run(host='0.0.0.0', port=5000, debug=True)
