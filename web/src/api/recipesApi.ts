import { apiClient } from "./client";
import type { ApiResult, PaginatedList } from "@/types/api";
import type {
  CreateRecipeRequest,
  Recipe,
  RecipeListQuery,
  UpdateRecipeRequest,
} from "@/types/recipe";

export async function getRecipes(
  query: RecipeListQuery = {},
): Promise<PaginatedList<Recipe>> {
  const { data } = await apiClient.get<ApiResult<PaginatedList<Recipe>>>(
    "/api/recipes",
    { params: query },
  );
  return data.data;
}

export async function getRecipeById(id: string): Promise<Recipe> {
  const { data } = await apiClient.get<ApiResult<Recipe>>(`/api/recipes/${id}`);
  return data.data;
}

export async function createRecipe(request: CreateRecipeRequest): Promise<Recipe> {
  const { data } = await apiClient.post<ApiResult<Recipe>>("/api/recipes", request);
  return data.data;
}

export async function updateRecipe(
  id: string,
  request: UpdateRecipeRequest,
): Promise<Recipe> {
  const { data } = await apiClient.put<ApiResult<Recipe>>(
    `/api/recipes/${id}`,
    request,
  );
  return data.data;
}

export async function deleteRecipe(id: string): Promise<void> {
  await apiClient.delete(`/api/recipes/${id}`);
}
