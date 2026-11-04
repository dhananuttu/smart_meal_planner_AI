import { Button } from "@/components/ui";
import type { PantryItem } from "@/types/pantry";
import { formatExpirationDate, getPantryStatuses } from "../helpers/pantryStatus";
import { PantryStatusBadge } from "./PantryStatusBadge";

interface PantryTableProps {
  items: PantryItem[];
  onEdit: (item: PantryItem) => void;
  onDelete: (item: PantryItem) => void;
  isDeleting: boolean;
}

export function PantryTable({ items, onEdit, onDelete, isDeleting }: PantryTableProps) {
  return (
    <>
      {/* Desktop table */}
      <div className="hidden overflow-x-auto md:block">
        <table className="min-w-full border-collapse">
          <thead>
            <tr className="border-b border-slate-100 bg-slate-50 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
              <th className="px-4 py-3">Ingredient</th>
              <th className="px-4 py-3">Quantity</th>
              <th className="px-4 py-3">Expires</th>
              <th className="px-4 py-3">Threshold</th>
              <th className="px-4 py-3">Status</th>
              <th className="px-4 py-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {items.map((item) => {
              const statuses = getPantryStatuses(item);
              const rowClass = item.isExpired
                ? "bg-red-50/40"
                : item.isExpiringSoon
                  ? "bg-amber-50/40"
                  : item.isLowStock
                    ? "bg-orange-50/40"
                    : "";

              return (
                <tr key={item.id} className={`border-b border-slate-100 ${rowClass}`}>
                  <td className="px-4 py-4">
                    <p className="font-medium text-slate-900">{item.ingredient.name}</p>
                    {item.ingredient.category ? (
                      <p className="text-xs text-slate-500">{item.ingredient.category}</p>
                    ) : null}
                  </td>
                  <td className="px-4 py-4 text-sm text-slate-700">
                    {item.quantity} {item.unit}
                  </td>
                  <td className="px-4 py-4 text-sm text-slate-700">
                    {formatExpirationDate(item.expirationDate)}
                  </td>
                  <td className="px-4 py-4 text-sm text-slate-700">
                    {item.lowStockThreshold != null ? `${item.lowStockThreshold} ${item.unit}` : "—"}
                  </td>
                  <td className="px-4 py-4">
                    <div className="flex flex-wrap gap-1.5">
                      {statuses.length > 0 ? (
                        statuses.map((status) => (
                          <PantryStatusBadge key={status} status={status} />
                        ))
                      ) : (
                        <span className="text-xs text-slate-400">OK</span>
                      )}
                    </div>
                  </td>
                  <td className="px-4 py-4 text-right">
                    <div className="flex justify-end gap-2">
                      <Button variant="secondary" onClick={() => onEdit(item)}>
                        Edit
                      </Button>
                      <Button
                        variant="ghost"
                        className="text-red-600 hover:bg-red-50"
                        disabled={isDeleting}
                        onClick={() => onDelete(item)}
                      >
                        Delete
                      </Button>
                    </div>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>

      {/* Mobile cards */}
      <div className="space-y-3 md:hidden">
        {items.map((item) => {
          const statuses = getPantryStatuses(item);
          const cardClass = item.isExpired
            ? "border-red-200 bg-red-50/30"
            : item.isExpiringSoon
              ? "border-amber-200 bg-amber-50/30"
              : item.isLowStock
                ? "border-orange-200 bg-orange-50/30"
                : "border-slate-200 bg-white";

          return (
            <article key={item.id} className={`rounded-xl border p-4 shadow-sm ${cardClass}`}>
              <div className="flex items-start justify-between gap-3">
                <div>
                  <h3 className="font-semibold text-slate-900">{item.ingredient.name}</h3>
                  <p className="mt-1 text-sm text-slate-600">
                    {item.quantity} {item.unit}
                  </p>
                </div>
                <div className="flex flex-wrap justify-end gap-1.5">
                  {statuses.map((status) => (
                    <PantryStatusBadge key={status} status={status} />
                  ))}
                </div>
              </div>
              <dl className="mt-3 grid grid-cols-2 gap-2 text-xs text-slate-600">
                <div>
                  <dt className="font-medium text-slate-500">Expires</dt>
                  <dd>{formatExpirationDate(item.expirationDate)}</dd>
                </div>
                <div>
                  <dt className="font-medium text-slate-500">Low stock at</dt>
                  <dd>
                    {item.lowStockThreshold != null
                      ? `${item.lowStockThreshold} ${item.unit}`
                      : "—"}
                  </dd>
                </div>
              </dl>
              <div className="mt-4 flex gap-2">
                <Button variant="secondary" className="flex-1" onClick={() => onEdit(item)}>
                  Edit
                </Button>
                <Button
                  variant="ghost"
                  className="flex-1 text-red-600 hover:bg-red-50"
                  disabled={isDeleting}
                  onClick={() => onDelete(item)}
                >
                  Delete
                </Button>
              </div>
            </article>
          );
        })}
      </div>
    </>
  );
}
