import { Link, Outlet, useNavigate } from "react-router-dom";
import { Button } from "@/components/ui";
import { useAuth } from "@/hooks/useAuth";

const navItems = [
  { to: "/dashboard", label: "Dashboard" },
  { to: "/recipes", label: "Recipes" },
  { to: "/meal-planner", label: "Meal Planner" },
  { to: "/pantry", label: "Pantry" },
  { to: "/grocery", label: "Grocery" },
  { to: "/todos", label: "Todos" },
  { to: "/alerts", label: "Alerts" },
];

export function AppLayout() {
  const navigate = useNavigate();
  const { user, logout } = useAuth();

  async function handleLogout() {
    await logout();
    navigate("/login", { replace: true });
  }

  return (
    <div className="min-h-screen bg-slate-50" data-testid="app-layout">
      <header className="border-b border-slate-200 bg-white">
        <div className="mx-auto flex max-w-6xl items-center justify-between gap-4 px-4 py-4">
          <div>
            <p className="text-sm font-semibold text-brand-700">Smart Meal Planner</p>
            {user ? (
              <p className="text-xs text-slate-500">
                {user.displayName} · {user.householdName}
              </p>
            ) : null}
          </div>
          <Button variant="secondary" onClick={handleLogout}>
            Sign out
          </Button>
        </div>
        <nav className="mx-auto flex max-w-6xl gap-1 overflow-x-auto px-4 pb-3">
          {navItems.map((item) => (
            <Link
              key={item.to}
              to={item.to}
              className="rounded-lg px-3 py-2 text-sm font-medium text-slate-600 hover:bg-slate-100 hover:text-slate-900"
            >
              {item.label}
            </Link>
          ))}
        </nav>
      </header>
      <main className="mx-auto max-w-6xl px-4 py-8">
        <Outlet />
      </main>
    </div>
  );
}
