import { useQueries } from "@tanstack/react-query";
import * as groceryApi from "@/api/groceryApi";
import * as mealPlansApi from "@/api/mealPlansApi";
import * as pantryApi from "@/api/pantryApi";
import * as todosApi from "@/api/todosApi";
import type { GroceryListSummary } from "@/types/grocery";
import type { Meal } from "@/types/mealPlan";
import type { PantryItem } from "@/types/pantry";
import type { Todo } from "@/types/todo";
import { formatDateIso, getWeekStartMonday } from "@/utils/dateUtils";

const MEAL_TYPE_ORDER = ["Breakfast", "Lunch", "Dinner", "Snack"];

export interface ShoppingListSummary {
  list: GroceryListSummary;
  remainingCount: number;
}

export interface DashboardData {
  todayIso: string;
  meals: Meal[];
  tasks: Todo[];
  lowStockItems: PantryItem[];
  expiringItems: PantryItem[];
  shoppingSummary: ShoppingListSummary | null;
}

export interface WidgetState {
  isLoading: boolean;
  isError: boolean;
}

export interface DashboardState {
  data: DashboardData;
  meals: WidgetState;
  tasks: WidgetState;
  lowStock: WidgetState;
  expiring: WidgetState;
  shopping: WidgetState;
  hasAnyError: boolean;
}

function sortMeals(meals: Meal[]): Meal[] {
  return [...meals].sort(
    (a, b) => MEAL_TYPE_ORDER.indexOf(a.mealType) - MEAL_TYPE_ORDER.indexOf(b.mealType),
  );
}

function pickShoppingSummary(lists: GroceryListSummary[]): ShoppingListSummary | null {
  if (lists.length === 0) {
    return null;
  }

  const sorted = [...lists].sort(
    (a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime(),
  );

  const active = sorted.find((list) => list.itemCount > list.purchasedItemCount) ?? sorted[0];
  return {
    list: active,
    remainingCount: Math.max(active.itemCount - active.purchasedItemCount, 0),
  };
}

async function fetchTodayMeals(todayIso: string, weekStart: string): Promise<Meal[]> {
  const plan = await mealPlansApi.getWeeklyPlan(weekStart);
  const today = plan.days.find((day) => day.date === todayIso);
  return sortMeals(today?.meals ?? []);
}

function toWidgetState(query: { isLoading: boolean; isError: boolean }): WidgetState {
  return { isLoading: query.isLoading, isError: query.isError };
}

export function useDashboard(): DashboardState {
  const today = new Date();
  const todayIso = formatDateIso(today);
  const weekStart = getWeekStartMonday(today);

  const results = useQueries({
    queries: [
      {
        queryKey: ["dashboard", "meals", todayIso],
        queryFn: () => fetchTodayMeals(todayIso, weekStart),
      },
      {
        queryKey: ["dashboard", "todos", todayIso],
        queryFn: todosApi.getToday,
      },
      {
        queryKey: ["dashboard", "low-stock"],
        queryFn: () =>
          pantryApi.getInventory({ lowStockOnly: true, pageSize: 5, page: 1 }),
      },
      {
        queryKey: ["dashboard", "expiring"],
        queryFn: () =>
          pantryApi.getInventory({ expiringSoonOnly: true, pageSize: 5, page: 1 }),
      },
      {
        queryKey: ["dashboard", "grocery-lists"],
        queryFn: groceryApi.getLists,
      },
    ],
  });

  const [mealsQuery, tasksQuery, lowStockQuery, expiringQuery, groceryQuery] = results;

  return {
    data: {
      todayIso,
      meals: mealsQuery.data ?? [],
      tasks: tasksQuery.data ?? [],
      lowStockItems: lowStockQuery.data?.items ?? [],
      expiringItems: expiringQuery.data?.items ?? [],
      shoppingSummary: groceryQuery.data ? pickShoppingSummary(groceryQuery.data) : null,
    },
    meals: toWidgetState(mealsQuery),
    tasks: toWidgetState(tasksQuery),
    lowStock: toWidgetState(lowStockQuery),
    expiring: toWidgetState(expiringQuery),
    shopping: toWidgetState(groceryQuery),
    hasAnyError: results.some((query) => query.isError),
  };
}
