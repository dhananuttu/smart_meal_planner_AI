import type { AxiosError } from "axios";
import type { ApiProblemDetails } from "@/types/api";

export function getApiErrorMessage(error: unknown, fallback = "Something went wrong."): string {
  if (!isAxiosError(error)) {
    return fallback;
  }

  const data = error.response?.data as ApiProblemDetails | undefined;
  if (data?.detail) {
    return data.detail;
  }

  if (data?.errors) {
    const first = Object.values(data.errors).flat()[0];
    if (first) {
      return first;
    }
  }

  if (data?.title) {
    return data.title;
  }

  return fallback;
}

function isAxiosError(error: unknown): error is AxiosError {
  return typeof error === "object" && error !== null && "isAxiosError" in error;
}
