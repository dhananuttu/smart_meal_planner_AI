import axios from "axios";
import type { ApiResult } from "@/types/api";
import type { AuthResponse } from "@/types/auth";
import {
  clearStoredSession,
  getStoredSession,
  setStoredSession,
  toAuthSession,
} from "@/utils/tokenStorage";

const baseURL = import.meta.env.VITE_API_BASE_URL ?? "";

export const apiClient = axios.create({
  baseURL,
  headers: {
    "Content-Type": "application/json",
  },
});

const refreshClient = axios.create({
  baseURL,
  headers: {
    "Content-Type": "application/json",
  },
});

let refreshPromise: Promise<string | null> | null = null;

apiClient.interceptors.request.use((config) => {
  const session = getStoredSession();
  if (session?.accessToken) {
    config.headers.Authorization = `Bearer ${session.accessToken}`;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    const isAuthRoute =
      typeof originalRequest?.url === "string"
      && ["/api/auth/login", "/api/auth/register", "/api/auth/refresh"].some((path) =>
        originalRequest.url.includes(path),
      );

    if (
      error.response?.status !== 401
      || !originalRequest
      || originalRequest._retry
      || isAuthRoute
    ) {
      return Promise.reject(error);
    }

    originalRequest._retry = true;

    if (!refreshPromise) {
      refreshPromise = attemptTokenRefresh().finally(() => {
        refreshPromise = null;
      });
    }

    const accessToken = await refreshPromise;
    if (!accessToken) {
      clearStoredSession();
      return Promise.reject(error);
    }

    originalRequest.headers.Authorization = `Bearer ${accessToken}`;
    return apiClient(originalRequest);
  },
);

async function attemptTokenRefresh(): Promise<string | null> {
  const session = getStoredSession();
  if (!session?.refreshToken) {
    return null;
  }

  try {
    const { data } = await refreshClient.post<ApiResult<AuthResponse>>(
      "/api/auth/refresh",
      { refreshToken: session.refreshToken },
    );
    const nextSession = toAuthSession(data.data);
    setStoredSession(nextSession);
    return nextSession.accessToken;
  } catch {
    clearStoredSession();
    return null;
  }
}

declare module "axios" {
  export interface AxiosRequestConfig {
    _retry?: boolean;
  }
}
