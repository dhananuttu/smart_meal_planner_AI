import type { Meal, MealSlotType, WeeklyMealPlan } from "@/types/mealPlan";
import { MEAL_SLOTS } from "@/types/mealPlan";

export type MealGrid = Record<string, Record<MealSlotType, Meal[]>>;

export function buildMealGrid(plan: WeeklyMealPlan): MealGrid {
  const grid: MealGrid = {};

  for (const day of plan.days) {
    grid[day.date] = {
      Breakfast: [],
      Lunch: [],
      Dinner: [],
      Snack: [],
    };

    for (const meal of day.meals) {
      const slot = meal.mealType as MealSlotType;
      if (grid[day.date][slot]) {
        grid[day.date][slot].push(meal);
      }
    }
  }

  return grid;
}

export function getWeekDays(plan: WeeklyMealPlan): string[] {
  return plan.days.map((day) => day.date);
}

export { MEAL_SLOTS };
