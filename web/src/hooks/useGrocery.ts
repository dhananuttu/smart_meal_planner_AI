import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import * as groceryApi from "@/api/groceryApi";
import type {
  AddGroceryItemRequest,
  CreateGroceryListRequest,
  GenerateGroceryListFromMealPlanRequest,
  GroceryListDetail,
  GroceryListSummary,
  MarkGroceryItemPurchasedRequest,
} from "@/types/grocery";

export const groceryKeys = {
  all: ["grocery"] as const,
  lists: () => [...groceryKeys.all, "lists"] as const,
  list: (id: string) => [...groceryKeys.all, "list", id] as const,
};

function toSummary(list: GroceryListDetail): GroceryListSummary {
  return {
    id: list.id,
    name: list.name,
    mealPlanId: list.mealPlanId,
    isArchived: list.isArchived,
    itemCount: list.items.length,
    purchasedItemCount: list.items.filter((item) => item.isPurchased).length,
    createdAt: list.createdAt,
    updatedAt: list.updatedAt,
  };
}

function prependListSummary(
  queryClient: ReturnType<typeof useQueryClient>,
  list: GroceryListDetail,
) {
  const summary = toSummary(list);

  queryClient.setQueryData<GroceryListSummary[]>(groceryKeys.lists(), (current) => {
    if (!current) {
      return [summary];
    }

    const without = current.filter((entry) => entry.id !== summary.id);
    return [summary, ...without];
  });

  queryClient.setQueryData(groceryKeys.list(list.id), list);
}

export function useGroceryLists() {
  return useQuery({
    queryKey: groceryKeys.lists(),
    queryFn: groceryApi.getLists,
    staleTime: 0,
  });
}

export function useGroceryList(id: string | null) {
  return useQuery({
    queryKey: groceryKeys.list(id ?? ""),
    queryFn: () => groceryApi.getListById(id!),
    enabled: Boolean(id),
    staleTime: 0,
  });
}

export function useCreateGroceryList() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateGroceryListRequest) => groceryApi.createList(request),
    onSuccess: (list) => {
      prependListSummary(queryClient, list);
      queryClient.invalidateQueries({ queryKey: groceryKeys.all });
    },
  });
}

export function useGenerateGroceryList() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: GenerateGroceryListFromMealPlanRequest) =>
      groceryApi.generateFromMealPlan(request),
    onSuccess: (list) => {
      prependListSummary(queryClient, list);
      queryClient.invalidateQueries({ queryKey: groceryKeys.all });
    },
  });
}

export function useAddGroceryItem(listId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: AddGroceryItemRequest) => groceryApi.addItem(listId, request),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: groceryKeys.list(listId) });
      await queryClient.invalidateQueries({ queryKey: groceryKeys.lists() });
    },
  });
}

export function useMarkGroceryItemPurchased(listId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      itemId,
      request,
    }: {
      itemId: string;
      request: MarkGroceryItemPurchasedRequest;
    }) => groceryApi.markItemPurchased(itemId, request),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: groceryKeys.list(listId) });
      await queryClient.invalidateQueries({ queryKey: groceryKeys.lists() });
    },
  });
}

export function useDeleteGroceryItem(listId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (itemId: string) => groceryApi.deleteItem(itemId),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: groceryKeys.list(listId) });
      await queryClient.invalidateQueries({ queryKey: groceryKeys.lists() });
    },
  });
}
