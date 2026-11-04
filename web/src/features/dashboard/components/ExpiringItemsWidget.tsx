import { Link } from "react-router-dom";
import type { PantryItem } from "@/types/pantry";
import { DashboardWidget } from "./DashboardWidget";

interface ExpiringItemsWidgetProps {
  items: PantryItem[];
  isLoading: boolean;
  isError: boolean;
}

export function ExpiringItemsWidget({ items, isLoading, isError }: ExpiringItemsWidgetProps) {
  return (
    <DashboardWidget
      title="Expiring Items"
      href="/pantry"
      isLoading={isLoading}
      isError={isError}
      isEmpty={items.length === 0}
      emptyMessage="Nothing expiring soon."
      emptyAction={
        <Link
          to="/pantry"
          className="text-sm font-medium text-brand-700 hover:text-brand-600"
        >
          View pantry
        </Link>
      }
    >
      <ul className="space-y-2">
        {items.map((item) => (
          <li
            key={item.id}
            className="flex items-center justify-between gap-3 rounded-lg bg-amber-50 px-3 py-2.5"
          >
            <p className="truncate text-sm font-medium text-slate-900">
              {item.ingredient.name}
            </p>
            <p className="shrink-0 text-sm text-amber-800">
              {item.expirationDate
                ? new Date(item.expirationDate + "T00:00:00").toLocaleDateString(undefined, {
                    month: "short",
                    day: "numeric",
                  })
                : "Soon"}
            </p>
          </li>
        ))}
      </ul>
    </DashboardWidget>
  );
}
