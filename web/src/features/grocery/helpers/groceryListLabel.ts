import type { GroceryListSummary } from "@/types/grocery";

export function formatGroceryListLabel(entry: GroceryListSummary): string {
  const created = new Date(entry.createdAt).toLocaleString(undefined, {
    month: "short",
    day: "numeric",
    hour: "numeric",
    minute: "2-digit",
  });

  return `${entry.name} · ${created} · ${entry.purchasedItemCount}/${entry.itemCount} bought`;
}

export function sortGroceryLists(lists: GroceryListSummary[]): GroceryListSummary[] {
  return [...lists].sort(
    (a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime(),
  );
}
