import { Button } from "@/components/ui";
import type { GroceryItem } from "@/types/grocery";

interface GroceryItemRowProps {
  item: GroceryItem;
  isUpdating: boolean;
  isDeleting: boolean;
  onTogglePurchased: (item: GroceryItem, isPurchased: boolean) => void;
  onDelete: (item: GroceryItem) => void;
}

export function GroceryItemRow({
  item,
  isUpdating,
  isDeleting,
  onTogglePurchased,
  onDelete,
}: GroceryItemRowProps) {
  return (
    <li
      className={`flex items-center gap-3 rounded-xl border px-4 py-3 transition ${
        item.isPurchased
          ? "border-slate-200 bg-slate-50"
          : "border-slate-200 bg-white hover:border-brand-200"
      }`}
    >
      <input
        type="checkbox"
        checked={item.isPurchased}
        disabled={isUpdating || isDeleting}
        onChange={(event) => onTogglePurchased(item, event.target.checked)}
        className="h-4 w-4 rounded border-slate-300 text-brand-600 focus:ring-brand-500"
        aria-label={`Mark ${item.name} as purchased`}
      />

      <div className="min-w-0 flex-1">
        <p
          className={`font-medium ${
            item.isPurchased ? "text-slate-400 line-through" : "text-slate-900"
          }`}
        >
          {item.name}
        </p>
        <p className={`text-sm ${item.isPurchased ? "text-slate-400" : "text-slate-600"}`}>
          {item.quantity} {item.unit}
          {item.note ? ` · ${item.note}` : ""}
        </p>
      </div>

      <Button
        variant="ghost"
        className="shrink-0 text-red-600 hover:bg-red-50"
        disabled={isDeleting}
        onClick={() => onDelete(item)}
      >
        Delete
      </Button>
    </li>
  );
}
