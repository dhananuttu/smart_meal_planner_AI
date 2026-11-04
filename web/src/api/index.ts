/**
 * HTTP client and API module boundaries.
 *
 * - client.ts     — Axios instance, interceptors, auth header injection
 * - modules/*     — One file per backend domain (maps to Api controllers)
 *
 * Do not import axios directly from features; use module functions here.
 */

export { apiClient } from "./client";
export * as authApi from "./authApi";
