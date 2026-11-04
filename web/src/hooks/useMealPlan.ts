import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import * as mealPlansApi from "@/api/mealPlansApi";
import type { CreateMealRequest, UpdateMealRequest } from "@/types/mealPlan";

export const mealPlanKeys = {
  all: ["meal-plans"] as const,
  weekly: (weekStart: string) => [...mealPlanKeys.all, "weekly", weekStart] as const,
};

export function useWeeklyMealPlan(weekStart: string) {
  return useQuery({
    queryKey: mealPlanKeys.weekly(weekStart),
    queryFn: () => mealPlansApi.getWeeklyPlan(weekStart),
  });
}

export function useCreateMeal(weekStart: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateMealRequest) => mealPlansApi.createMeal(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: mealPlanKeys.weekly(weekStart) });
    },
  });
}

export function useUpdateMeal(weekStart: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, request }: { id: string; request: UpdateMealRequest }) =>
      mealPlansApi.updateMeal(id, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: mealPlanKeys.weekly(weekStart) });
    },
  });
}

export function useDeleteMeal(weekStart: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => mealPlansApi.deleteMeal(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: mealPlanKeys.weekly(weekStart) });
    },
  });
}
