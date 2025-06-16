import React, { createContext, useContext } from "react";
import { IUserService } from "@/core/services/interfaces/user-service";
import { UserService } from "@/core/services/user-service.impl";
import { IAuthService } from "@/core/services/interfaces/auth-service";
import { AuthService } from "@/core/services/auth-service.impl";
import { HttpClient, httpClient } from "@/data/http/httpClient";
import { IRoleService } from "@/core/services/interfaces/role-service";
import { IRoleApi } from "@/data/api/interfaces/role-api";
import { RoleService } from "@/core/services/role-service.impl";
import { RoleApi } from "@/data/api/role-api.impl";
import { IUserApi } from "@/data/api/interfaces/user-api";
import { UserApi } from "@/data/api/user-api.impl";
import { IAuthApi } from "@/data/api/interfaces/auth-api";
import { AuthApi } from "@/data/api/auth-api.impl";

import { IObjectPredictionApi } from "@/data/api/interfaces/objectPrediction-api";
import { ObjectPredictionApi } from "@/data/api/objectPrediction-api.impl";
import { IObjectPredictionService } from "@/core/services/interfaces/objectPrediction-service";
import { ObjectPredictionService } from "@/core/services/objectPrediction-service.impl";
import { IPredictionApi } from "@/data/api/interfaces/prediction-api";
import { PredictionApi } from "@/data/api/prediction-api.impl";
import { IPredictionService } from "@/core/services/interfaces/prediction-service";
import { PredictionService } from "@/core/services/prediction-service.impl";
import { ILogService} from "@/core/services/interfaces/log-service";
import { LogService } from "@/core/services/log-service.impl";
import { ILogApi } from "@/data/api/interfaces/log-api";
import { LogApi } from "@/data/api/log-api.impl";


import { IRoboflowModelService } from "@/core/services/interfaces/roboflowModel-service";
import { RoboflowModelService } from "@/core/services/roboflowModel-service.impl";
import { IRoboflowModelApi } from "@/data/api/interfaces/roboflowModel-api";
import { RoboflowModelApi } from "@/data/api/roboflowModel-api.impl";

// API layer instances
const roleApi = new RoleApi(httpClient);
const userApi = new UserApi(httpClient);
const authApi = new AuthApi(httpClient);
const objectPredictionApi = new ObjectPredictionApi(httpClient);
const predictionApi = new PredictionApi(httpClient);
const roboflowModelApi = new RoboflowModelApi(httpClient);
const logApi = new LogApi(httpClient);

// Service layer instances
const userService = new UserService(userApi);
const authService = new AuthService(authApi);
const roleService = new RoleService(roleApi);
const logService = new LogService(logApi);
const objectPredictionService = new ObjectPredictionService(objectPredictionApi);
const predictionService = new PredictionService(predictionApi);

const roboflowModelService = new RoboflowModelService(roboflowModelApi);

interface DIContainer {
  httpClient: HttpClient;
  roleApi: IRoleApi;
  userApi: IUserApi;
  authApi: IAuthApi;
  objectPredictionApi: IObjectPredictionApi;
  predictionApi: IPredictionApi;
  userService: IUserService;
  authService: IAuthService;
  roleService: IRoleService;
  objectPredictionService: IObjectPredictionService;
  predictionService: IPredictionService;
  roboflowModelService: IRoboflowModelService;
  roboflowModelApi: IRoboflowModelApi;
  logService: ILogService;
  logApi: ILogApi;
}

const DIContext = createContext<DIContainer>({
  httpClient,
  roleApi,
  userApi,
  authApi,
  objectPredictionApi,
  predictionApi,
  userService,
  authService,
  roleService,
  objectPredictionService,
  predictionService,
  roboflowModelService,
  roboflowModelApi,
  logService,
  logApi,
});

export const DIProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  return (
    <DIContext.Provider
      value={{
        httpClient,
        roleApi,
        userApi,
        authApi,
        objectPredictionApi,
        predictionApi,
        userService,
        authService,
        roleService,
        objectPredictionService,
        predictionService,
        roboflowModelService,
        roboflowModelApi,
        logService,
        logApi,
      }}
    >
      {children}
    </DIContext.Provider>
  );
};

export const useDIContainer = () => useContext(DIContext);

export const useHttpClient = () => useContext(DIContext).httpClient;
export const useUserService = () => useContext(DIContext).userService;
export const useUserApi = () => useContext(DIContext).userApi;
export const useAuthService = () => useContext(DIContext).authService;
export const useAuthApi = () => useContext(DIContext).authApi;
export const useRoleService = () => useContext(DIContext).roleService;
export const useRoleApi = () => useContext(DIContext).roleApi;
export const useObjectPredictionService = () => useContext(DIContext).objectPredictionService;
export const useObjectPredictionApi = () => useContext(DIContext).objectPredictionApi;
export const usePredictionService = () => useContext(DIContext).predictionService;
export const usePredictionApi = () => useContext(DIContext).predictionApi;
export const useRoboflowModelService = () => useContext(DIContext).roboflowModelService;
export const useRoboflowModelApi = () => useContext(DIContext).roboflowModelApi;
export const useLogService = () => useContext(DIContext).logService;
export const useLogApi = () => useContext(DIContext).logApi;
