import type { Meal, MealSlotType } from "@/types/mealPlan";

interface MealSlotCellProps {
  date: string;
  mealType: MealSlotType;
  meals: Meal[];
  onAssign: (date: string, mealType: MealSlotType, meal?: Meal) => void;
}

const slotAccent: Record<MealSlotType, string> = {
  Breakfast: "border-amber-200 bg-amber-50/50 hover:bg-amber-50",
  Lunch: "border-sky-200 bg-sky-50/50 hover:bg-sky-50",
  Dinner: "border-violet-200 bg-violet-50/50 hover:bg-violet-50",
  Snack: "border-emerald-200 bg-emerald-50/50 hover:bg-emerald-50",
};

export function MealSlotCell({ date, mealType, meals, onAssign }: MealSlotCellProps) {
  const primaryMeal = meals[0];

  return (
    <button
      type="button"
      onClick={() => onAssign(date, mealType, primaryMeal)}
      className={`min-h-[88px] w-full rounded-xl border p-3 text-left transition ${slotAccent[mealType]}`}
    >
      {primaryMeal?.recipe ? (
        <div>
          <p className="line-clamp-2 text-sm font-medium text-slate-900">
            {primaryMeal.recipe.name}
          </p>
          {primaryMeal.notes ? (
            <p className="mt-1 line-clamp-2 text-xs text-slate-500">{primaryMeal.notes}</p>
          ) : null}
          {meals.length > 1 ? (
            <p className="mt-2 text-xs font-medium text-slate-500">+{meals.length - 1} more</p>
          ) : null}
        </div>
      ) : (
        <p className="text-sm text-slate-500">+ Assign recipe</p>
      )}
    </button>
  );
}
