import { useMemo, useState } from "react";
import { LoadingSpinner } from "@/components/common";
import { Button } from "@/components/ui";
import {
  useCreateMeal,
  useDeleteMeal,
  useUpdateMeal,
  useWeeklyMealPlan,
} from "@/hooks/useMealPlan";
import type { Meal, MealSlotType } from "@/types/mealPlan";
import { MEAL_SLOTS } from "@/types/mealPlan";
import {
  addDaysToIso,
  formatDisplayDate,
  formatShortDay,
  getWeekStartMonday,
  isToday,
} from "@/utils/dateUtils";
import { AssignRecipeDialog } from "./AssignRecipeDialog";
import { MealSlotCell } from "./MealSlotCell";
import { buildMealGrid, getWeekDays } from "../helpers/mealPlanGrid";

interface DialogState {
  date: string;
  mealType: MealSlotType;
  meal?: Meal;
}

export function WeeklyMealPlannerView() {
  const [weekStart, setWeekStart] = useState(() => getWeekStartMonday(new Date()));
  const [dialog, setDialog] = useState<DialogState | null>(null);

  const { data: plan, isLoading, isError } = useWeeklyMealPlan(weekStart);
  const createMeal = useCreateMeal(weekStart);
  const updateMeal = useUpdateMeal(weekStart);
  const deleteMeal = useDeleteMeal(weekStart);

  const grid = useMemo(() => (plan ? buildMealGrid(plan) : null), [plan]);
  const weekDays = plan ? getWeekDays(plan) : [];

  const isMutating =
    createMeal.isPending || updateMeal.isPending || deleteMeal.isPending;

  async function handleSave(recipeId: string, notes: string) {
    if (!dialog) {
      return;
    }

    const payload = {
      date: dialog.date,
      mealType: dialog.mealType,
      recipeId,
      notes: notes || undefined,
    };

    if (dialog.meal) {
      await updateMeal.mutateAsync({ id: dialog.meal.id, request: payload });
    } else {
      await createMeal.mutateAsync(payload);
    }
  }

  async function handleRemove() {
    if (!dialog?.meal) {
      return;
    }

    await deleteMeal.mutateAsync(dialog.meal.id);
  }

  if (isLoading) {
    return <LoadingSpinner label="Loading meal plan…" />;
  }

  if (isError || !plan || !grid) {
    return (
      <div className="rounded-2xl border border-red-200 bg-red-50 p-8 text-center text-sm text-red-700">
        Unable to load the weekly meal plan.
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div>
          <p className="text-sm font-medium text-brand-700">Meal Planner</p>
          <h1 className="mt-1 text-2xl font-bold text-slate-900 sm:text-3xl">Weekly plan</h1>
          <p className="mt-2 text-sm text-slate-600">
            {formatDisplayDate(plan.weekStartDate)} – {formatDisplayDate(plan.weekEndDate)}
          </p>
        </div>

        <div className="flex items-center gap-2">
          <Button
            variant="secondary"
            onClick={() => setWeekStart((current) => addDaysToIso(current, -7))}
          >
            Previous week
          </Button>
          <Button variant="secondary" onClick={() => setWeekStart(getWeekStartMonday(new Date()))}>
            Today
          </Button>
          <Button
            variant="secondary"
            onClick={() => setWeekStart((current) => addDaysToIso(current, 7))}
          >
            Next week
          </Button>
        </div>
      </div>

      {/* Desktop grid: meal types as rows, days as columns */}
      <div className="hidden overflow-x-auto rounded-2xl border border-slate-200 bg-white shadow-sm lg:block">
        <table className="min-w-full border-collapse">
          <thead>
            <tr className="border-b border-slate-100 bg-slate-50">
              <th className="w-28 px-4 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
                Meal
              </th>
              {weekDays.map((date) => (
                <th
                  key={date}
                  className={`min-w-[140px] px-3 py-3 text-center text-sm font-semibold ${
                    isToday(date) ? "text-brand-700" : "text-slate-700"
                  }`}
                >
                  <div>{formatShortDay(date)}</div>
                  {isToday(date) ? (
                    <span className="mt-1 inline-block rounded-full bg-brand-100 px-2 py-0.5 text-xs font-medium text-brand-700">
                      Today
                    </span>
                  ) : null}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {MEAL_SLOTS.map((mealType) => (
              <tr key={mealType} className="border-b border-slate-100 last:border-b-0">
                <td className="px-4 py-4 align-top text-sm font-medium text-slate-700">
                  {mealType}
                </td>
                {weekDays.map((date) => (
                  <td key={`${date}-${mealType}`} className="px-3 py-3 align-top">
                    <MealSlotCell
                      date={date}
                      mealType={mealType}
                      meals={grid[date]?.[mealType] ?? []}
                      onAssign={(d, type, meal) => setDialog({ date: d, mealType: type, meal })}
                    />
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Mobile: one card per day */}
      <div className="space-y-4 lg:hidden">
        {weekDays.map((date) => (
          <section
            key={date}
            className={`rounded-2xl border bg-white p-4 shadow-sm ${
              isToday(date) ? "border-brand-200" : "border-slate-200"
            }`}
          >
            <div className="mb-4 flex items-center justify-between">
              <h2 className="text-base font-semibold text-slate-900">{formatShortDay(date)}</h2>
              {isToday(date) ? (
                <span className="rounded-full bg-brand-100 px-2.5 py-1 text-xs font-medium text-brand-700">
                  Today
                </span>
              ) : null}
            </div>
            <div className="space-y-3">
              {MEAL_SLOTS.map((mealType) => (
                <div key={mealType}>
                  <p className="mb-1.5 text-xs font-semibold uppercase tracking-wide text-slate-500">
                    {mealType}
                  </p>
                  <MealSlotCell
                    date={date}
                    mealType={mealType}
                    meals={grid[date]?.[mealType] ?? []}
                    onAssign={(d, type, meal) => setDialog({ date: d, mealType: type, meal })}
                  />
                </div>
              ))}
            </div>
          </section>
        ))}
      </div>

      {dialog ? (
        <AssignRecipeDialog
          open
          date={dialog.date}
          mealType={dialog.mealType}
          meal={dialog.meal}
          isSaving={isMutating}
          isRemoving={deleteMeal.isPending}
          onClose={() => setDialog(null)}
          onSave={handleSave}
          onRemove={handleRemove}
        />
      ) : null}
    </div>
  );
}
