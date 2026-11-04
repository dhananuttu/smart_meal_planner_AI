import { useAuth } from "@/hooks/useAuth";
import { formatDisplayDate } from "@/utils/dateUtils";
import { useDashboard } from "../hooks/useDashboard";
import { ExpiringItemsWidget } from "./ExpiringItemsWidget";
import { LowStockWidget } from "./LowStockWidget";
import { ShoppingListWidget } from "./ShoppingListWidget";
import { TodaysMealsWidget } from "./TodaysMealsWidget";
import { TodaysTasksWidget } from "./TodaysTasksWidget";

export function DashboardView() {
  const { user } = useAuth();
  const { data, meals, tasks, lowStock, expiring, shopping, hasAnyError } = useDashboard();

  return (
    <div className="space-y-6">
      <header>
        <p className="text-sm font-medium text-brand-700">Dashboard</p>
        <h1 className="mt-1 text-2xl font-bold text-slate-900 sm:text-3xl">
          {user ? `Welcome back, ${user.displayName}` : "Welcome back"}
        </h1>
        <p className="mt-2 text-sm text-slate-600 sm:text-base">
          {formatDisplayDate(data.todayIso)} · {user?.householdName}
        </p>
      </header>

      {hasAnyError ? (
        <div
          className="rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-800"
          role="alert"
        >
          Some sections could not be loaded. Other widgets may still show current data.
        </div>
      ) : null}

      <div className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
        <TodaysMealsWidget
          meals={data.meals}
          isLoading={meals.isLoading}
          isError={meals.isError}
        />
        <TodaysTasksWidget
          tasks={data.tasks}
          isLoading={tasks.isLoading}
          isError={tasks.isError}
        />
        <LowStockWidget
          items={data.lowStockItems}
          isLoading={lowStock.isLoading}
          isError={lowStock.isError}
        />
        <ExpiringItemsWidget
          items={data.expiringItems}
          isLoading={expiring.isLoading}
          isError={expiring.isError}
        />
        <ShoppingListWidget
          summary={data.shoppingSummary}
          isLoading={shopping.isLoading}
          isError={shopping.isError}
        />
      </div>
    </div>
  );
}
