import { apiClient } from "./client";
import type { ApiResult } from "@/types/api";
import type { InventoryAlert } from "@/types/inventoryAlert";

export async function getAlerts(): Promise<InventoryAlert[]> {
  const { data } = await apiClient.get<ApiResult<InventoryAlert[]>>(
    "/api/inventory-alerts",
    { params: { includeDismissed: false, includeRead: true } },
  );
  return data.data;
}
