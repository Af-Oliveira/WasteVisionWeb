import { FormValidationError } from "@/lib/utils";
import { RoboflowModel, RoboflowModelSchema } from "@/core/domain/RoboflowModel";
import { 
  CreateRoboflowModelDTO, 
  UpdateRoboflowModelDTO,
  RoboflowModelSearchParamsDTO 
} from "@/data/dto/roboflowModel-dto";
import { useState } from "react";

export function useRoboflowForm(initialData?: Partial<RoboflowModel>) {
  const [description, setDescription] = useState(initialData?.description || "");
  const [apiKey, setApiKey] = useState(initialData?.apiKey || "");
  const [modelUrl, setModelUrl] = useState(initialData?.modelUrl || "");
  const [localModel, setLocalModel] = useState<File | null>(null);
  const [endPoint, setEndPoint] = useState(initialData?.endPoint || "");
  const [map, setMap] = useState<string>(initialData?.map || "");
  const [recall, setRecall] = useState<string>(initialData?.recall || "");
  const [precision, setPrecision] = useState<string>(initialData?.precision || "");
  

  const reset = () => {
    setDescription("");
    setApiKey("");
    setModelUrl("");
    setLocalModel(null);
    setEndPoint("");
    setMap("");
    setRecall("");
    setPrecision("");
  };

  const getCreateFormData = (): CreateRoboflowModelDTO => {
    if (!localModel) {
      throw new FormValidationError("localModel", "Model file is required");
    }
    return {
      description,
      apiKey,
      modelUrl,
      modelFile: localModel,
    };
  };

  const getUpdateFormData = (): UpdateRoboflowModelDTO => {
    if (!localModel) {
      throw new FormValidationError("localModel", "Model file is required");
    }
    return {
      description,
      apiKey,
      modelUrl,
      modelFile: localModel,
    };
  };

  const isFormValid = (): boolean => {
    // Basic validation
    if (!description.trim()) {
      throw new FormValidationError("description", "Description is required");
    }
    if (!apiKey.trim()) {
      throw new FormValidationError("apiKey", "API Key is required");
    }
    if (!modelUrl.trim()) {
      throw new FormValidationError("modelUrl", "Model URL is required");
    }
    if (!localModel) {
      throw new FormValidationError("localModel", "Model file is required");
    }
    
    // File validation
    const allowedTypes = ['.pt', '.onnx', '.weights'];
    const fileExtension = localModel.name.toLowerCase().substring(localModel.name.lastIndexOf('.'));
    if (!allowedTypes.includes(fileExtension)) {
      throw new FormValidationError("localModel", "Invalid file type. Only .pt, .onnx, and .weights files are allowed");
    }
    
    return true;
  };

  return {
    formData: {
      description,
      apiKey,
      modelUrl,
      localModel,
      endPoint,
      map,
      recall,
      precision,
    },
    setters: {
      setDescription,
      setApiKey,
      setModelUrl,
      setLocalModel,
      setEndPoint,
      setMap,
      setRecall,
      setPrecision,
    },
    reset,
    isFormValid,
    getCreateFormData,
    getUpdateFormData,
  };
}

export function useRoboFlowModelSearchForm() {
  const [description, setDescription] = useState("");
  const [modelUrl, setModelUrl] = useState("");
  const [active, setActive] = useState("");

  const reset = () => {
    setDescription("");
    setModelUrl("");
    setActive("");
  };

  const getSearchParams = (): RoboflowModelSearchParamsDTO => ({
    description: description || undefined,
    modelUrl: modelUrl || undefined,
    active: active || undefined
  });

  return {
    formData: {
      description,
      modelUrl,
      active
    },
    setters: {
      setDescription,
      setModelUrl,
      setActive
    },
    reset,
    getSearchParams,
  };
}
