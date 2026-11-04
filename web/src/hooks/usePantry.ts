import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import * as pantryApi from "@/api/pantryApi";
import type {
  CreatePantryItemRequest,
  PantryInventoryQuery,
  UpdatePantryItemRequest,
} from "@/types/pantry";

export const pantryKeys = {
  all: ["pantry"] as const,
  lists: () => [...pantryKeys.all, "list"] as const,
  list: (query: PantryInventoryQuery) => [...pantryKeys.lists(), query] as const,
  detail: (id: string) => [...pantryKeys.all, "detail", id] as const,
};

export function usePantryInventory(query: PantryInventoryQuery) {
  return useQuery({
    queryKey: pantryKeys.list(query),
    queryFn: () => pantryApi.getInventory(query),
  });
}

export function useCreatePantryItem() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreatePantryItemRequest) => pantryApi.createItem(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: pantryKeys.lists() });
    },
  });
}

export function useUpdatePantryItem() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, request }: { id: string; request: UpdatePantryItemRequest }) =>
      pantryApi.updateItem(id, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: pantryKeys.lists() });
    },
  });
}

export function useDeletePantryItem() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => pantryApi.deleteItem(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: pantryKeys.lists() });
    },
  });
}
