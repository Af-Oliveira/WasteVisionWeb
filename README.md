# WasteVision Python Backend (YOLOv8 Flask Service)

This is the Python microservice for the WasteVision project. It provides REST API endpoints for object detection using YOLOv8 models. The service is designed to be called by the main .NET backend and supports both file uploads and base64-encoded images.

---

## Features

- **YOLOv8 Inference:** Detects objects in images using Ultralytics YOLOv8.
- **REST API:** Flask-based endpoints for detection and health checks.
- **Flexible Input:** Accepts both file uploads and base64-encoded images.
- **Model Selection:** Supports specifying a custom model per request or using a default model.
- **Error Handling:** Returns clear error messages for invalid input or model issues.

---

---

## Developed by

Afonso Guilherme Vieira da Silva Oliveira  
Student Number: 1221160

---

# Waste Vision Application Setup and Run Guide

This guide provides comprehensive instructions for setting up and running the Waste Vision application, which consists of backend (.NET Core) and frontend (React/TypeScript) components.

## Prerequisites
- .NET SDK 8.0 or later
- Node.js and npm/pnpm
- MySQL Server
- Python 3.x (for the Python backend component)
- Docker (optional, for containerized deployment)

## Backend Setup (WasteVisionWebBE)

### Setting Up the Backend

1.  Navigate to the backend directory:
    ```bash
    cd WasteVisionWebBE
    ```
2.  Restore .NET packages:
    ```bash
    dotnet restore
    ```
3.  Configure database connection in `appsettings.json` if needed.

### Running the Backend (Windows)

Use the provided batch script:
```bash
run-dddnetcore.bat
```

This script will:
- Set Development environment
- Build the project (using `build.bat`)
- Update the database (using `update_db.bat`)
- Run tests
- Start the application

### Running the Backend (Linux/Mac)

```bash
sh run-dddnetcore.sh
```

For quicker startup (skipping tests):
```bash
sh quick-run.sh
```

### Backend Services

Once running, the backend exposes the following services:
- Web API (HTTP): http://localhost:3000
- Web API (HTTPS): https://localhost:3001
- MySQL: localhost:3306
- phpMyAdmin (LOCAL): http://localhost:8080
- phpMyAdmin (PROD): http://localhost:8081

## Frontend Setup (WasteVisionWebFE)

### Setting Up the Frontend

1.  Navigate to the frontend directory:
    ```bash
    cd WasteVisionWebFE
    ```
2.  Install dependencies:

    *   Using npm:
        ```bash
        npm install
        ```
    *   Or using pnpm (recommended based on the presence of `pnpm-lock.yaml`):
        ```bash
        pnpm install
        ```
3.  Run the development server:
    *   Using npm:
        ```bash
        npm run dev
        ```
    *   Or using pnpm:
        ```bash
        pnpm dev
        ```

This will start the Vite development server, typically on port 5173 (http://localhost:5173).

## Python Backend Component

For the machine learning/prediction functionality:

1.  Navigate to the Python backend directory:
    ```bash
    cd WasteVisionWebBE/PythonBE
    ```
2.  Install Python dependencies:
    ```bash
    pip install -r requirements.txt
    ```
3.  Run the Python service:
    ```bash
    python app.py
    ```

## Using Docker (Optional)

For containerized deployment:
```bash
cd WasteVisionWebBE
docker-compose up
```

This will start all the required services as defined in the `docker-compose.yml` file.

## Application Structure

-   **Backend**: DDD (Domain-Driven Design) architecture with:
    -   Controllers for API endpoints
    -   Domain models and services
    -   Infrastructure layer for data persistence
    -   Authentication and authorization services
-   **Frontend**: React application with:
    -   TypeScript for type safety
    -   Component library using shadcn/ui components
    -   Context-based state management
    -   Protected routes for authentication

## Troubleshooting

-   If database connection fails, verify MySQL is running and credentials in `appsettings.json` are correct.
-   For authentication issues, check if the Keycloak service is properly configured (`keycloak_data` folder indicates Keycloak integration).
-   Check backend logs in `mylogs.csv` for errors.

## Additional Information


**Note:** Ensure that the `Admin` and `User` roles are inserted into the database to facilitate user logins.
