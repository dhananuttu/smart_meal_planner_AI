import { FormEvent, useEffect, useState } from "react";
import { Button, Input, Label, Modal, Select } from "@/components/ui";
import { useRecipes } from "@/hooks/useRecipes";
import type { Meal, MealSlotType } from "@/types/mealPlan";
import { formatDisplayDate } from "@/utils/dateUtils";
import { getApiErrorMessage } from "@/utils/apiError";

interface AssignRecipeDialogProps {
  open: boolean;
  date: string;
  mealType: MealSlotType;
  meal?: Meal;
  isSaving: boolean;
  isRemoving: boolean;
  onClose: () => void;
  onSave: (recipeId: string, notes: string) => Promise<void>;
  onRemove: () => Promise<void>;
}

export function AssignRecipeDialog({
  open,
  date,
  mealType,
  meal,
  isSaving,
  isRemoving,
  onClose,
  onSave,
  onRemove,
}: AssignRecipeDialogProps) {
  const [recipeId, setRecipeId] = useState("");
  const [notes, setNotes] = useState("");
  const [error, setError] = useState<string | null>(null);
  const { data: recipesData, isLoading } = useRecipes({ page: 1, pageSize: 100 });

  useEffect(() => {
    if (open) {
      setRecipeId(meal?.recipeId ?? "");
      setNotes(meal?.notes ?? "");
      setError(null);
    }
  }, [open, meal]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    if (!recipeId) {
      setError("Please select a recipe.");
      return;
    }

    try {
      await onSave(recipeId, notes.trim());
      onClose();
    } catch (err) {
      setError(getApiErrorMessage(err, "Unable to assign recipe."));
    }
  }

  async function handleRemove() {
    setError(null);
    try {
      await onRemove();
      onClose();
    } catch (err) {
      setError(getApiErrorMessage(err, "Unable to remove assignment."));
    }
  }

  return (
    <Modal
      open={open}
      title={`${mealType} · ${formatDisplayDate(date)}`}
      onClose={onClose}
    >
      <form className="space-y-5" onSubmit={handleSubmit}>
        {error ? (
          <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
            {error}
          </div>
        ) : null}

        <div>
          <Label htmlFor="recipeId">Recipe</Label>
          <Select
            id="recipeId"
            value={recipeId}
            onChange={(event) => setRecipeId(event.target.value)}
            disabled={isLoading}
            required
          >
            <option value="">Select a recipe…</option>
            {(recipesData?.items ?? []).map((recipe) => (
              <option key={recipe.id} value={recipe.id}>
                {recipe.name}
              </option>
            ))}
          </Select>
        </div>

        <div>
          <Label htmlFor="notes">Notes (optional)</Label>
          <Input
            id="notes"
            value={notes}
            onChange={(event) => setNotes(event.target.value)}
            placeholder="Prep reminders, substitutions…"
            maxLength={500}
          />
        </div>

        <div className="flex flex-col-reverse gap-3 border-t border-slate-100 pt-5 sm:flex-row sm:justify-between">
          <div>
            {meal ? (
              <Button
                type="button"
                variant="ghost"
                className="text-red-600 hover:bg-red-50"
                isLoading={isRemoving}
                onClick={handleRemove}
              >
                Remove
              </Button>
            ) : null}
          </div>
          <div className="flex flex-col-reverse gap-3 sm:flex-row">
            <Button type="button" variant="secondary" onClick={onClose}>
              Cancel
            </Button>
            <Button type="submit" isLoading={isSaving}>
              {meal ? "Update" : "Assign"}
            </Button>
          </div>
        </div>
      </form>
    </Modal>
  );
}
