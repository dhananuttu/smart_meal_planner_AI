import { Outlet } from "react-router-dom";

export function AuthLayout() {
  return (
    <div
      className="flex min-h-screen items-center justify-center bg-gradient-to-br from-brand-50 via-white to-slate-100 px-4 py-12"
      data-testid="auth-layout"
    >
      <Outlet />
    </div>
  );
}
