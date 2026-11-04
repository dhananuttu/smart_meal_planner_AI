import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import * as ingredientsApi from "@/api/ingredientsApi";
import type { PaginatedList } from "@/types/api";
import type { CreateIngredientRequest, Ingredient, IngredientListQuery } from "@/types/ingredient";

export const ingredientKeys = {
  all: ["ingredients"] as const,
  list: (query: IngredientListQuery) => [...ingredientKeys.all, "list", query] as const,
};

export function useIngredients(query: IngredientListQuery = {}) {
  return useQuery({
    queryKey: ingredientKeys.list(query),
    queryFn: () => ingredientsApi.getIngredients(query),
  });
}

export function useCreateIngredient() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateIngredientRequest) => ingredientsApi.createIngredient(request),
    onSuccess: (ingredient) => {
      queryClient.setQueryData<PaginatedList<Ingredient>>(
        ingredientKeys.list({}),
        (current) => {
          if (!current) {
            return {
              items: [ingredient],
              page: 1,
              pageSize: 100,
              totalCount: 1,
              totalPages: 1,
              hasPreviousPage: false,
              hasNextPage: false,
            };
          }

          if (current.items.some((item) => item.id === ingredient.id)) {
            return current;
          }

          return {
            ...current,
            items: [...current.items, ingredient].sort((a, b) => a.name.localeCompare(b.name)),
            totalCount: current.totalCount + 1,
          };
        },
      );
      queryClient.invalidateQueries({ queryKey: ingredientKeys.all });
    },
  });
}
