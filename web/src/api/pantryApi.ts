import { apiClient } from "./client";
import type { ApiResult, PaginatedList } from "@/types/api";
import type {
  CreatePantryItemRequest,
  PantryInventoryQuery,
  PantryItem,
  UpdatePantryItemRequest,
} from "@/types/pantry";

export async function getInventory(
  query: PantryInventoryQuery = {},
): Promise<PaginatedList<PantryItem>> {
  const { data } = await apiClient.get<ApiResult<PaginatedList<PantryItem>>>(
    "/api/pantry/items",
    { params: query },
  );
  return data.data;
}

export async function getById(id: string): Promise<PantryItem> {
  const { data } = await apiClient.get<ApiResult<PantryItem>>(`/api/pantry/items/${id}`);
  return data.data;
}

export async function createItem(request: CreatePantryItemRequest): Promise<PantryItem> {
  const { data } = await apiClient.post<ApiResult<PantryItem>>("/api/pantry/items", request);
  return data.data;
}

export async function updateItem(
  id: string,
  request: UpdatePantryItemRequest,
): Promise<PantryItem> {
  const { data } = await apiClient.put<ApiResult<PantryItem>>(
    `/api/pantry/items/${id}`,
    request,
  );
  return data.data;
}

export async function deleteItem(id: string): Promise<void> {
  await apiClient.delete(`/api/pantry/items/${id}`);
}
