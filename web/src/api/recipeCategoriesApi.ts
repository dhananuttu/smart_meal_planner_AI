import { apiClient } from "./client";
import type { ApiResult, PaginatedList } from "@/types/api";
import type {
  CreateRecipeCategoryRequest,
  RecipeCategory,
  RecipeCategoryListQuery,
} from "@/types/recipeCategory";

export async function getCategories(
  query: RecipeCategoryListQuery = {},
): Promise<PaginatedList<RecipeCategory>> {
  const { data } = await apiClient.get<ApiResult<PaginatedList<RecipeCategory>>>(
    "/api/recipe-categories",
    { params: { pageSize: 100, ...query } },
  );
  return data.data;
}

export async function createCategory(
  request: CreateRecipeCategoryRequest,
): Promise<RecipeCategory> {
  const { data } = await apiClient.post<ApiResult<RecipeCategory>>(
    "/api/recipe-categories",
    request,
  );
  return data.data;
}
