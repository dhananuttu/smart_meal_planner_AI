import {
  useMutation,
  useQuery,
  useQueryClient,
  type UseQueryOptions,
} from "@tanstack/react-query";
import * as recipeCategoriesApi from "@/api/recipeCategoriesApi";
import * as recipesApi from "@/api/recipesApi";
import type { PaginatedList } from "@/types/api";
import type {
  CreateRecipeRequest,
  Recipe,
  RecipeListQuery,
  UpdateRecipeRequest,
} from "@/types/recipe";
import type { CreateRecipeCategoryRequest, RecipeCategory } from "@/types/recipeCategory";

export const recipeKeys = {
  all: ["recipes"] as const,
  lists: () => [...recipeKeys.all, "list"] as const,
  list: (query: RecipeListQuery) => [...recipeKeys.lists(), query] as const,
  details: () => [...recipeKeys.all, "detail"] as const,
  detail: (id: string) => [...recipeKeys.details(), id] as const,
};

export const recipeCategoryKeys = {
  all: ["recipe-categories"] as const,
  list: () => [...recipeCategoryKeys.all, "list"] as const,
};

export function useRecipes(query: RecipeListQuery) {
  return useQuery({
    queryKey: recipeKeys.list(query),
    queryFn: () => recipesApi.getRecipes(query),
  });
}

export function useRecipe(
  id: string,
  options?: Pick<UseQueryOptions<Recipe>, "enabled">,
) {
  return useQuery({
    queryKey: recipeKeys.detail(id),
    queryFn: () => recipesApi.getRecipeById(id),
    enabled: options?.enabled ?? Boolean(id),
  });
}

export function useRecipeCategories() {
  return useQuery({
    queryKey: recipeCategoryKeys.list(),
    queryFn: () => recipeCategoriesApi.getCategories(),
    staleTime: 0,
  });
}

function appendRecipeCategoryToCache(
  queryClient: ReturnType<typeof useQueryClient>,
  category: RecipeCategory,
) {
  queryClient.setQueryData<PaginatedList<RecipeCategory>>(
    recipeCategoryKeys.list(),
    (current) => {
      if (!current) {
        return {
          items: [category],
          page: 1,
          pageSize: 100,
          totalCount: 1,
          totalPages: 1,
          hasPreviousPage: false,
          hasNextPage: false,
        };
      }

      if (current.items.some((item) => item.id === category.id)) {
        return current;
      }

      return {
        ...current,
        items: [...current.items, category].sort((a, b) => a.name.localeCompare(b.name)),
        totalCount: current.totalCount + 1,
      };
    },
  );
}

export function useCreateRecipeCategory() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateRecipeCategoryRequest) =>
      recipeCategoriesApi.createCategory(request),
    onSuccess: (category) => {
      appendRecipeCategoryToCache(queryClient, category);
      queryClient.invalidateQueries({ queryKey: recipeCategoryKeys.all });
    },
  });
}

export function useCreateRecipe() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateRecipeRequest) => recipesApi.createRecipe(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: recipeKeys.lists() });
    },
  });
}

export function useUpdateRecipe(id: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: UpdateRecipeRequest) => recipesApi.updateRecipe(id, request),
    onSuccess: (recipe) => {
      queryClient.setQueryData(recipeKeys.detail(id), recipe);
      queryClient.invalidateQueries({ queryKey: recipeKeys.lists() });
    },
  });
}

export function useDeleteRecipe() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => recipesApi.deleteRecipe(id),
    onSuccess: (_data, id) => {
      queryClient.removeQueries({ queryKey: recipeKeys.detail(id) });
      queryClient.invalidateQueries({ queryKey: recipeKeys.lists() });
    },
  });
}
