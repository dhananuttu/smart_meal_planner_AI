import type { PantryItem } from "@/types/pantry";

export type PantryStatus = "expired" | "expiring" | "low-stock";

export function getPantryStatuses(item: PantryItem): PantryStatus[] {
  const statuses: PantryStatus[] = [];
  if (item.isExpired) {
    statuses.push("expired");
  } else if (item.isExpiringSoon) {
    statuses.push("expiring");
  }
  if (item.isLowStock) {
    statuses.push("low-stock");
  }
  return statuses;
}

export function formatExpirationDate(expirationDate?: string): string {
  if (!expirationDate) {
    return "—";
  }

  return new Date(expirationDate + "T00:00:00").toLocaleDateString(undefined, {
    month: "short",
    day: "numeric",
    year: "numeric",
  });
}
