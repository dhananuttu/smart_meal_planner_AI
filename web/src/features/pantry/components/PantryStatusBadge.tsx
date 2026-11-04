import type { PantryStatus } from "../helpers/pantryStatus";

const styles: Record<PantryStatus, string> = {
  "low-stock": "bg-orange-100 text-orange-800",
  expiring: "bg-amber-100 text-amber-800",
  expired: "bg-red-100 text-red-800",
};

const labels: Record<PantryStatus, string> = {
  "low-stock": "Low stock",
  expiring: "Expiring soon",
  expired: "Expired",
};

interface PantryStatusBadgeProps {
  status: PantryStatus;
}

export function PantryStatusBadge({ status }: PantryStatusBadgeProps) {
  return (
    <span
      className={`inline-flex rounded-full px-2.5 py-0.5 text-xs font-medium ${styles[status]}`}
    >
      {labels[status]}
    </span>
  );
}
