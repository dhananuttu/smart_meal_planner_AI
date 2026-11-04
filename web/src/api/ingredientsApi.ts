import { apiClient } from "./client";
import type { ApiResult, PaginatedList } from "@/types/api";
import type {
  CreateIngredientRequest,
  Ingredient,
  IngredientListQuery,
} from "@/types/ingredient";

export async function getIngredients(
  query: IngredientListQuery = {},
): Promise<PaginatedList<Ingredient>> {
  const { data } = await apiClient.get<ApiResult<PaginatedList<Ingredient>>>(
    "/api/ingredients",
    { params: { pageSize: 100, ...query } },
  );
  return data.data;
}

export async function createIngredient(request: CreateIngredientRequest): Promise<Ingredient> {
  const { data } = await apiClient.post<ApiResult<Ingredient>>("/api/ingredients", request);
  return data.data;
}
