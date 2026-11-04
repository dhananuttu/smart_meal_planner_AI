import { apiClient } from "./client";
import type { ApiResult } from "@/types/api";
import type {
  CreateMealRequest,
  Meal,
  UpdateMealRequest,
  WeeklyMealPlan,
} from "@/types/mealPlan";

export async function getWeeklyPlan(weekStart: string): Promise<WeeklyMealPlan> {
  const { data } = await apiClient.get<ApiResult<WeeklyMealPlan>>(
    "/api/meal-plans/weekly",
    { params: { weekStart } },
  );
  return data.data;
}

export async function createMeal(request: CreateMealRequest): Promise<Meal> {
  const { data } = await apiClient.post<ApiResult<Meal>>(
    "/api/meal-plans/meals",
    request,
  );
  return data.data;
}

export async function updateMeal(id: string, request: UpdateMealRequest): Promise<Meal> {
  const { data } = await apiClient.put<ApiResult<Meal>>(
    `/api/meal-plans/meals/${id}`,
    request,
  );
  return data.data;
}

export async function deleteMeal(id: string): Promise<void> {
  await apiClient.delete(`/api/meal-plans/meals/${id}`);
}
