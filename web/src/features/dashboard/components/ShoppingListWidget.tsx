import { Link } from "react-router-dom";
import type { ShoppingListSummary } from "../hooks/useDashboard";
import { DashboardWidget } from "./DashboardWidget";

interface ShoppingListWidgetProps {
  summary: ShoppingListSummary | null;
  isLoading: boolean;
  isError: boolean;
}

export function ShoppingListWidget({ summary, isLoading, isError }: ShoppingListWidgetProps) {
  const isEmpty = !summary || summary.list.itemCount === 0;

  return (
    <DashboardWidget
      title="Shopping List Summary"
      href="/grocery"
      isLoading={isLoading}
      isError={isError}
      isEmpty={isEmpty}
      emptyMessage="No active grocery lists."
      emptyAction={
        <Link
          to="/grocery"
          className="text-sm font-medium text-brand-700 hover:text-brand-600"
        >
          Create a list
        </Link>
      }
    >
      {summary ? (
        <div className="space-y-4">
          <div>
            <p className="text-lg font-semibold text-slate-900">{summary.list.name}</p>
            <p className="mt-1 text-sm text-slate-500">
              Updated{" "}
              {new Date(summary.list.updatedAt).toLocaleDateString(undefined, {
                month: "short",
                day: "numeric",
              })}
            </p>
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div className="rounded-xl bg-slate-50 px-4 py-3 text-center">
              <p className="text-2xl font-bold text-slate-900">{summary.remainingCount}</p>
              <p className="text-xs text-slate-500">To buy</p>
            </div>
            <div className="rounded-xl bg-brand-50 px-4 py-3 text-center">
              <p className="text-2xl font-bold text-brand-700">
                {summary.list.purchasedItemCount}
              </p>
              <p className="text-xs text-brand-700/80">Purchased</p>
            </div>
          </div>

          {summary.list.itemCount > 0 ? (
            <div className="h-2 overflow-hidden rounded-full bg-slate-100">
              <div
                className="h-full rounded-full bg-brand-600 transition-all"
                style={{
                  width: `${Math.round(
                    (summary.list.purchasedItemCount / summary.list.itemCount) * 100,
                  )}%`,
                }}
              />
            </div>
          ) : null}
        </div>
      ) : null}
    </DashboardWidget>
  );
}
