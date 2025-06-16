import { config } from "@/config";
import axios, { AxiosError, AxiosRequestConfig } from "axios";

export interface HttpClient {
  get<T>(url: string, config?: AxiosRequestConfig): Promise<T>;
  post<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T>;
  put<T>(url: string, data: any, config?: AxiosRequestConfig): Promise<T>;
  patch<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T>;
  delete<T>(url: string, config?: AxiosRequestConfig): Promise<T>;
}

export class ApiError extends Error {
  constructor(
    message: string,
    public status?: number,
    public details?: unknown
  ) {
    super(message);
    this.name = "ApiError";
  }
}

const axiosInstance = axios.create({
  baseURL: config.API_URL,
  timeout: 50 * 1000,
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: true,
});

const handleError = (error: unknown): never => {
  if (axios.isAxiosError(error)) {
    const axiosError = error as AxiosError;
    const status = axiosError.response?.status;

    let message = "An unexpected error occurred";
    if (status === 401)
      message = "Your session has expired. Please log in again.";
    else if (status === 403)
      message = "You don't have permission to perform this action.";
    else if (status === 404) message = "The requested resource was not found.";
    else if (status === 422) message = "The provided data is invalid.";
    else if (status === 500)
      message = "An internal server error occurred. Please try again later.";
    else if (status) message = `Request failed with status: ${status}`;
    else if (axiosError.request)
      message = "Unable to reach the server. Please check your connection.";

    interface ApiResponseData {
      message?: string;
      title?: string;
      data?: { message?: string };
    }

    const apiMessage =
      (axiosError.response?.data as ApiResponseData)?.message ||
      (axiosError.response?.data as ApiResponseData)?.title ||
      (axiosError.response?.data as ApiResponseData)?.data?.message;

    throw new ApiError(apiMessage || message, status, error);
  }

  if (error instanceof Error) {
    throw new ApiError(error.message, undefined, error);
  }

  throw new ApiError("An unknown error occurred", undefined, error);
};

export const httpClient: HttpClient = {
  async get<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    try {
      const response = await axiosInstance.get<T>(url, config);
      return response.data;
    } catch (error) {
      return handleError(error);
    }
  },

  async post<T>(
    url: string,
    data?: any,
    config?: AxiosRequestConfig
  ): Promise<T> {
    try {
      const response = await axiosInstance.post<T>(url, data, config);
      return response.data;
    } catch (error) {
      return handleError(error);
    }
  },

  async put<T>(
    url: string,
    data: any,
    config?: AxiosRequestConfig
  ): Promise<T> {
    try {
      const response = await axiosInstance.put<T>(url, data, config);
      return response.data;
    } catch (error) {
      return handleError(error);
    }
  },

  async patch<T>(
    url: string,
    data?: any,
    config?: AxiosRequestConfig
  ): Promise<T> {
    try {
      const response = await axiosInstance.patch<T>(url, data, config);
      return response.data;
    } catch (error) {
      return handleError(error);
    }
  },

  async delete<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    try {
      const response = await axiosInstance.delete<T>(url, config);
      return response.data;
    } catch (error) {
      return handleError(error);
    }
  },
};
