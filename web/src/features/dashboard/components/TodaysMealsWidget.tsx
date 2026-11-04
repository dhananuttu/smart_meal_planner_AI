import { Link } from "react-router-dom";
import type { Meal } from "@/types/mealPlan";
import { DashboardWidget } from "./DashboardWidget";

interface TodaysMealsWidgetProps {
  meals: Meal[];
  isLoading: boolean;
  isError: boolean;
}

const mealTypeStyles: Record<string, string> = {
  Breakfast: "bg-amber-50 text-amber-800",
  Lunch: "bg-sky-50 text-sky-800",
  Dinner: "bg-violet-50 text-violet-800",
  Snack: "bg-emerald-50 text-emerald-800",
};

export function TodaysMealsWidget({ meals, isLoading, isError }: TodaysMealsWidgetProps) {
  return (
    <DashboardWidget
      title="Today's Meals"
      href="/meal-planner"
      isLoading={isLoading}
      isError={isError}
      isEmpty={meals.length === 0}
      emptyMessage="No meals planned for today."
      emptyAction={
        <Link
          to="/meal-planner"
          className="text-sm font-medium text-brand-700 hover:text-brand-600"
        >
          Plan meals
        </Link>
      }
      className="lg:col-span-2"
    >
      <ul className="space-y-3">
        {meals.map((meal) => (
          <li
            key={meal.id}
            className="flex items-start justify-between gap-3 rounded-xl border border-slate-100 bg-slate-50 px-4 py-3"
          >
            <div className="min-w-0">
              <span
                className={`inline-flex rounded-full px-2.5 py-0.5 text-xs font-medium ${
                  mealTypeStyles[meal.mealType] ?? "bg-slate-100 text-slate-700"
                }`}
              >
                {meal.mealType}
              </span>
              <p className="mt-2 truncate font-medium text-slate-900">
                {meal.recipe?.name ?? "No recipe assigned"}
              </p>
              {meal.notes ? (
                <p className="mt-1 truncate text-sm text-slate-500">{meal.notes}</p>
              ) : null}
            </div>
          </li>
        ))}
      </ul>
    </DashboardWidget>
  );
}
