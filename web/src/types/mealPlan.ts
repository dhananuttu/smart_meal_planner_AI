export interface MealRecipeSummary {
  id: string;
  name: string;
  isAvailable: boolean;
}

export interface Meal {
  id: string;
  mealPlanId: string;
  date: string;
  mealType: string;
  recipeId?: string;
  recipe?: MealRecipeSummary;
  notes?: string;
  createdAt: string;
  updatedAt: string;
}

export interface DailyMeals {
  date: string;
  meals: Meal[];
}

export interface WeeklyMealPlan {
  planId: string;
  weekStartDate: string;
  weekEndDate: string;
  days: DailyMeals[];
}

export interface CreateMealRequest {
  date: string;
  mealType: string;
  recipeId?: string;
  notes?: string;
}

export interface UpdateMealRequest {
  date: string;
  mealType: string;
  recipeId?: string;
  notes?: string;
}

export type MealSlotType = "Breakfast" | "Lunch" | "Dinner" | "Snack";

export const MEAL_SLOTS: MealSlotType[] = ["Breakfast", "Lunch", "Dinner", "Snack"];
