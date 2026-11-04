import { apiClient } from "./client";
import type { ApiResult } from "@/types/api";
import type {
  AddGroceryItemRequest,
  CreateGroceryListRequest,
  GenerateGroceryListFromMealPlanRequest,
  GroceryItem,
  GroceryListDetail,
  GroceryListSummary,
  MarkGroceryItemPurchasedRequest,
} from "@/types/grocery";

export async function getLists(): Promise<GroceryListSummary[]> {
  const { data } = await apiClient.get<ApiResult<GroceryListSummary[]>>(
    "/api/grocery/lists",
  );
  return data.data;
}

export async function getListById(id: string): Promise<GroceryListDetail> {
  const { data } = await apiClient.get<ApiResult<GroceryListDetail>>(
    `/api/grocery/lists/${id}`,
  );
  return data.data;
}

export async function createList(
  request: CreateGroceryListRequest = {},
): Promise<GroceryListDetail> {
  const { data } = await apiClient.post<ApiResult<GroceryListDetail>>(
    "/api/grocery/lists",
    request,
  );
  return data.data;
}

export async function generateFromMealPlan(
  request: GenerateGroceryListFromMealPlanRequest,
): Promise<GroceryListDetail> {
  const { data } = await apiClient.post<ApiResult<GroceryListDetail>>(
    "/api/grocery/lists/generate-from-meal-plan",
    request,
  );
  return data.data;
}

export async function addItem(
  listId: string,
  request: AddGroceryItemRequest,
): Promise<GroceryItem> {
  const { data } = await apiClient.post<ApiResult<GroceryItem>>(
    `/api/grocery/lists/${listId}/items`,
    request,
  );
  return data.data;
}

export async function markItemPurchased(
  itemId: string,
  request: MarkGroceryItemPurchasedRequest,
): Promise<GroceryItem> {
  const { data } = await apiClient.patch<ApiResult<GroceryItem>>(
    `/api/grocery/lists/items/${itemId}/purchased`,
    request,
  );
  return data.data;
}

export async function deleteItem(itemId: string): Promise<void> {
  await apiClient.delete(`/api/grocery/lists/items/${itemId}`);
}
