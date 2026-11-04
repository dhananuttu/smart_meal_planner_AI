import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import { useMutation } from "@tanstack/react-query";
import * as authApi from "@/api/authApi";
import type { AuthSession, AuthUser, LoginRequest, RegisterRequest } from "@/types/auth";
import {
  clearStoredSession,
  getStoredSession,
  setStoredSession,
  toAuthSession,
} from "@/utils/tokenStorage";

interface AuthContextValue {
  user: AuthUser | null;
  isAuthenticated: boolean;
  isBootstrapping: boolean;
  login: (request: LoginRequest) => Promise<void>;
  register: (request: RegisterRequest) => Promise<void>;
  logout: () => Promise<void>;
  isLoggingIn: boolean;
  isRegistering: boolean;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<AuthSession | null>(() => getStoredSession());
  const isBootstrapping = false;

  const applySession = useCallback((next: AuthSession) => {
    setStoredSession(next);
    setSession(next);
  }, []);

  const loginMutation = useMutation({
    mutationFn: authApi.login,
    onSuccess: (response) => {
      applySession(toAuthSession(response));
    },
  });

  const registerMutation = useMutation({
    mutationFn: authApi.register,
    onSuccess: (response) => {
      applySession(toAuthSession(response));
    },
  });

  const logoutMutation = useMutation({
    mutationFn: async () => {
      const refreshToken = session?.refreshToken;
      try {
        await authApi.logout(refreshToken ? { refreshToken } : {});
      } finally {
        clearStoredSession();
        setSession(null);
      }
    },
  });

  const value = useMemo<AuthContextValue>(
    () => ({
      user: session?.user ?? null,
      isAuthenticated: session !== null,
      isBootstrapping,
      login: (request) => loginMutation.mutateAsync(request).then(() => undefined),
      register: (request) => registerMutation.mutateAsync(request).then(() => undefined),
      logout: () => logoutMutation.mutateAsync().then(() => undefined),
      isLoggingIn: loginMutation.isPending,
      isRegistering: registerMutation.isPending,
    }),
    [session, isBootstrapping, loginMutation, registerMutation, logoutMutation],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuthContext(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuthContext must be used within AuthProvider");
  }
  return context;
}
