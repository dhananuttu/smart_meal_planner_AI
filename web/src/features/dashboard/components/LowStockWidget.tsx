import { Link } from "react-router-dom";
import type { PantryItem } from "@/types/pantry";
import { DashboardWidget } from "./DashboardWidget";

interface LowStockWidgetProps {
  items: PantryItem[];
  isLoading: boolean;
  isError: boolean;
}

export function LowStockWidget({ items, isLoading, isError }: LowStockWidgetProps) {
  return (
    <DashboardWidget
      title="Low Stock Inventory"
      href="/pantry"
      isLoading={isLoading}
      isError={isError}
      isEmpty={items.length === 0}
      emptyMessage="Pantry levels look good."
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
            className="flex items-center justify-between gap-3 rounded-lg bg-orange-50 px-3 py-2.5"
          >
            <p className="truncate text-sm font-medium text-slate-900">
              {item.ingredient.name}
            </p>
            <p className="shrink-0 text-sm text-orange-700">
              {item.quantity} {item.unit}
            </p>
          </li>
        ))}
      </ul>
    </DashboardWidget>
  );
}
