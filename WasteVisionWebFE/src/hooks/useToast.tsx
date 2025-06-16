import toast, { Toast } from "react-hot-toast";
import { createElement } from "react";
import { XCircle, CheckCircle2, AlertCircle, Info } from "lucide-react";

interface ToastOptions {
  duration?: number;
  position?:
    | "top-left"
    | "top-center"
    | "top-right"
    | "bottom-left"
    | "bottom-center"
    | "bottom-right";
}

// Custom styles for different toast types
const toastStyles = {
  success: {
    style: {
      background: "#10B981",
      color: "white",
      padding: "16px",
      borderRadius: "8px",
      boxShadow: "0 4px 6px -1px rgb(0 0 0 / 0.1)",
    },
    icon: CheckCircle2,
  },
  error: {
    style: {
      background: "#EF4444",
      color: "white",
      padding: "16px",
      borderRadius: "8px",
      boxShadow: "0 4px 6px -1px rgb(0 0 0 / 0.1)",
    },
    icon: XCircle,
  },
  warning: {
    style: {
      background: "#F59E0B",
      color: "white",
      padding: "16px",
      borderRadius: "8px",
      boxShadow: "0 4px 6px -1px rgb(0 0 0 / 0.1)",
    },
    icon: AlertCircle,
  },
  info: {
    style: {
      background: "#3B82F6",
      color: "white",
      padding: "16px",
      borderRadius: "8px",
      boxShadow: "0 4px 6px -1px rgb(0 0 0 / 0.1)",
    },
    icon: Info,
  },
};

export function useToast() {
  const showToast = (
    message: string,
    type: keyof typeof toastStyles = "info",
    options?: ToastOptions
  ) => {
    const { icon: Icon, style } = toastStyles[type];

    return toast(message, {
      duration: options?.duration || 3000,
      position: options?.position || "bottom-right",
      style,
      icon: createElement(Icon, {
        size: 20,
        className: "stroke-white",
      }),
    });
  };

  const success = (message: string, options?: ToastOptions) =>
    showToast(message, "success", options);

  const error = (message: string, options?: ToastOptions) =>
    showToast(message, "error", options);

  const warning = (message: string, options?: ToastOptions) =>
    showToast(message, "warning", options);

  const info = (message: string, options?: ToastOptions) =>
    showToast(message, "info", options);

  const dismiss = (toastId: Toast["id"]) => toast.dismiss(toastId);

  const promise = <T,>(
    promise: Promise<T>,
    {
      loading = "Loading...",
      success = "Success!",
      error = "Error occurred",
    } = {},
    options?: ToastOptions
  ) => {
    return toast.promise(
      promise,
      {
        loading,
        success,
        error,
      },
      {
        style: toastStyles.info.style,
        ...options,
      }
    );
  };

  return {
    show: showToast,
    success,
    error,
    warning,
    info,
    promise,
    dismiss,
    // Export the original toast for advanced usage
    toast,
  };
}
