import { apiClient } from "./client";
import type { ApiResult } from "@/types/api";
import type {
  AuthResponse,
  LoginRequest,
  LogoutRequest,
  RefreshTokenRequest,
  RegisterRequest,
} from "@/types/auth";

export async function login(request: LoginRequest): Promise<AuthResponse> {
  const { data } = await apiClient.post<ApiResult<AuthResponse>>(
    "/api/auth/login",
    request,
  );
  return data.data;
}

export async function register(request: RegisterRequest): Promise<AuthResponse> {
  const { data } = await apiClient.post<ApiResult<AuthResponse>>(
    "/api/auth/register",
    request,
  );
  return data.data;
}

export async function refreshToken(request: RefreshTokenRequest): Promise<AuthResponse> {
  const { data } = await apiClient.post<ApiResult<AuthResponse>>(
    "/api/auth/refresh",
    request,
  );
  return data.data;
}

export async function logout(request: LogoutRequest = {}): Promise<void> {
  await apiClient.post("/api/auth/logout", request);
}
