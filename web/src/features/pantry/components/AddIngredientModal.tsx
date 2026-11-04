import { FormEvent, useEffect, useState } from "react";
import { Button, Input, Label, Modal, Select, Textarea } from "@/components/ui";
import { useCreateIngredient } from "@/hooks/useIngredients";
import type { Ingredient, IngredientFormValues } from "@/types/ingredient";
import { INGREDIENT_CATEGORIES, emptyIngredientFormValues } from "@/types/ingredient";
import { PANTRY_UNITS } from "@/types/pantry";
import { getApiErrorMessage } from "@/utils/apiError";

interface AddIngredientModalProps {
  open: boolean;
  onClose: () => void;
  onCreated: (ingredient: Ingredient) => void;
}

export function AddIngredientModal({ open, onClose, onCreated }: AddIngredientModalProps) {
  const [values, setValues] = useState<IngredientFormValues>(emptyIngredientFormValues);
  const [error, setError] = useState<string | null>(null);
  const createIngredient = useCreateIngredient();

  useEffect(() => {
    if (open) {
      setValues(emptyIngredientFormValues);
      setError(null);
    }
  }, [open]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    if (!values.name.trim()) {
      setError("Ingredient name is required.");
      return;
    }

    try {
      const ingredient = await createIngredient.mutateAsync({
        name: values.name.trim(),
        defaultUnit: values.defaultUnit || undefined,
        category: values.category || undefined,
        notes: values.notes.trim() || undefined,
      });
      onCreated(ingredient);
      onClose();
    } catch (err) {
      setError(getApiErrorMessage(err, "Unable to create ingredient."));
    }
  }

  return (
    <Modal open={open} title="New ingredient" onClose={onClose}>
      <form className="space-y-5" onSubmit={handleSubmit}>
        {error ? (
          <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
            {error}
          </div>
        ) : null}

        <div>
          <Label htmlFor="ingredientName">Name</Label>
          <Input
            id="ingredientName"
            value={values.name}
            onChange={(event) => setValues((v) => ({ ...v, name: event.target.value }))}
            placeholder="Milk, rice, olive oil…"
            required
            maxLength={200}
            autoFocus
          />
        </div>

        <div className="grid gap-4 sm:grid-cols-2">
          <div>
            <Label htmlFor="ingredientCategory">Category</Label>
            <Select
              id="ingredientCategory"
              value={values.category}
              onChange={(event) => setValues((v) => ({ ...v, category: event.target.value }))}
            >
              <option value="">No category</option>
              {INGREDIENT_CATEGORIES.map((category) => (
                <option key={category} value={category}>
                  {category}
                </option>
              ))}
            </Select>
          </div>
          <div>
            <Label htmlFor="ingredientDefaultUnit">Default unit</Label>
            <Select
              id="ingredientDefaultUnit"
              value={values.defaultUnit}
              onChange={(event) => setValues((v) => ({ ...v, defaultUnit: event.target.value }))}
            >
              {PANTRY_UNITS.map((unit) => (
                <option key={unit} value={unit}>
                  {unit}
                </option>
              ))}
            </Select>
          </div>
        </div>

        <div>
          <Label htmlFor="ingredientNotes">Notes (optional)</Label>
          <Textarea
            id="ingredientNotes"
            value={values.notes}
            onChange={(event) => setValues((v) => ({ ...v, notes: event.target.value }))}
            rows={2}
            maxLength={500}
            placeholder="Brand preference, storage tips…"
          />
        </div>

        <div className="flex flex-col-reverse gap-3 border-t border-slate-100 pt-5 sm:flex-row sm:justify-end">
          <Button type="button" variant="secondary" onClick={onClose}>
            Cancel
          </Button>
          <Button type="submit" isLoading={createIngredient.isPending}>
            Create ingredient
          </Button>
        </div>
      </form>
    </Modal>
  );
}
