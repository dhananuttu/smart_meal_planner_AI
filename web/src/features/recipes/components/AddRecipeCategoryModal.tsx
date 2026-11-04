import { FormEvent, useEffect, useState } from "react";
import { Button, Input, Label, Modal } from "@/components/ui";
import { useCreateRecipeCategory } from "@/hooks/useRecipes";
import type { RecipeCategory, RecipeCategoryFormValues } from "@/types/recipeCategory";
import {
  CUISINE_SUGGESTIONS,
  RECIPE_CATEGORY_COLOR_PRESETS,
  emptyRecipeCategoryFormValues,
  isValidRecipeCategoryColor,
} from "@/types/recipeCategory";
import { getApiErrorMessage } from "@/utils/apiError";

interface AddRecipeCategoryModalProps {
  open: boolean;
  onClose: () => void;
  onCreated: (category: RecipeCategory) => void;
}

export function AddRecipeCategoryModal({
  open,
  onClose,
  onCreated,
}: AddRecipeCategoryModalProps) {
  const [values, setValues] = useState<RecipeCategoryFormValues>(emptyRecipeCategoryFormValues);
  const [error, setError] = useState<string | null>(null);
  const createCategory = useCreateRecipeCategory();

  useEffect(() => {
    if (open) {
      setValues(emptyRecipeCategoryFormValues);
      setError(null);
    }
  }, [open]);

  async function handleSubmit(event?: FormEvent<HTMLFormElement>) {
    event?.preventDefault();
    setError(null);

    if (!values.name.trim()) {
      setError("Cuisine name is required.");
      return;
    }

    if (!isValidRecipeCategoryColor(values.color)) {
      setError("Pick a color or enter a valid hex value (e.g. #4CAF50).");
      return;
    }

    try {
      const category = await createCategory.mutateAsync({
        name: values.name.trim(),
        color: values.color.trim(),
      });
      onCreated(category);
      onClose();
    } catch (err) {
      setError(getApiErrorMessage(err, "Unable to create cuisine category."));
    }
  }

  return (
    <Modal open={open} title="New cuisine category" onClose={onClose}>
      <form className="space-y-5" onSubmit={handleSubmit}>
        <p className="text-sm text-slate-600">
          Recipe categories describe <strong>cuisine or region</strong> (Italian, Korean, North
          Indian, etc.). They are separate from pantry ingredient types like Produce or Dairy.
        </p>

        {error ? (
          <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
            {error}
          </div>
        ) : null}

        <div>
          <Label htmlFor="cuisineName">Cuisine name</Label>
          <Input
            id="cuisineName"
            value={values.name}
            onChange={(event) => setValues((v) => ({ ...v, name: event.target.value }))}
            placeholder="Italian, North Indian, Korean…"
            required
            maxLength={100}
            autoFocus
          />
        </div>

        <div>
          <p className="mb-2 text-sm font-medium text-slate-700">Quick picks</p>
          <div className="flex flex-wrap gap-2">
            {CUISINE_SUGGESTIONS.map((cuisine) => (
              <button
                key={cuisine}
                type="button"
                onClick={() => setValues((v) => ({ ...v, name: cuisine }))}
                className={`rounded-full border px-3 py-1 text-xs font-medium transition ${
                  values.name === cuisine
                    ? "border-brand-300 bg-brand-50 text-brand-800"
                    : "border-slate-200 bg-white text-slate-600 hover:border-brand-200"
                }`}
              >
                {cuisine}
              </button>
            ))}
          </div>
        </div>

        <div>
          <Label htmlFor="cuisineColor">Label color</Label>
          <div className="mt-2 flex flex-wrap gap-2">
            {RECIPE_CATEGORY_COLOR_PRESETS.map((preset) => (
              <button
                key={preset.hex}
                type="button"
                title={preset.name}
                onClick={() => setValues((v) => ({ ...v, color: preset.hex }))}
                className={`h-9 w-9 rounded-full border-2 transition ${
                  values.color.toUpperCase() === preset.hex.toUpperCase()
                    ? "border-slate-900 ring-2 ring-slate-300"
                    : "border-white shadow-sm"
                }`}
                style={{ backgroundColor: preset.hex }}
                aria-label={preset.name}
              />
            ))}
          </div>
          <Input
            id="cuisineColor"
            value={values.color}
            onChange={(event) => setValues((v) => ({ ...v, color: event.target.value }))}
            placeholder="#4CAF50"
            className="mt-3 font-mono text-sm"
            maxLength={7}
            required
          />
        </div>

        <div className="flex flex-col-reverse gap-3 border-t border-slate-100 pt-5 sm:flex-row sm:justify-end">
          <Button type="button" variant="secondary" onClick={onClose}>
            Cancel
          </Button>
          <Button type="button" isLoading={createCategory.isPending} onClick={() => handleSubmit()}>
            Create cuisine
          </Button>
        </div>
      </form>
    </Modal>
  );
}
