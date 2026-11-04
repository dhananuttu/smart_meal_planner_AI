import { FormEvent, useEffect, useState } from "react";
import { Button, Input, Label, Modal, Select } from "@/components/ui";
import { useIngredients } from "@/hooks/useIngredients";
import type { Ingredient } from "@/types/ingredient";
import type { PantryItem, PantryItemFormValues } from "@/types/pantry";
import { PANTRY_UNITS } from "@/types/pantry";
import { getApiErrorMessage } from "@/utils/apiError";
import { AddIngredientModal } from "./AddIngredientModal";

interface PantryItemFormModalProps {
  open: boolean;
  mode: "create" | "edit";
  initialValues: PantryItemFormValues;
  item?: PantryItem;
  isSubmitting: boolean;
  onClose: () => void;
  onSubmit: (values: PantryItemFormValues) => Promise<void>;
}

export function PantryItemFormModal({
  open,
  mode,
  initialValues,
  item,
  isSubmitting,
  onClose,
  onSubmit,
}: PantryItemFormModalProps) {
  const [values, setValues] = useState<PantryItemFormValues>(initialValues);
  const [error, setError] = useState<string | null>(null);
  const [showAddIngredient, setShowAddIngredient] = useState(false);
  const { data: ingredientsData, isLoading: ingredientsLoading } = useIngredients();

  useEffect(() => {
    if (open) {
      setValues(initialValues);
      setError(null);
    }
  }, [open, initialValues]);

  function updateField<K extends keyof PantryItemFormValues>(
    field: K,
    value: PantryItemFormValues[K],
  ) {
    setValues((current) => ({ ...current, [field]: value }));
  }

  function handleIngredientCreated(ingredient: Ingredient) {
    setValues((current) => ({
      ...current,
      ingredientId: ingredient.id,
      unit: ingredient.defaultUnit ?? current.unit,
    }));
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    if (mode === "create" && !values.ingredientId) {
      setError("Please select an ingredient.");
      return;
    }

    if (!values.quantity || Number(values.quantity) < 0) {
      setError("Quantity must be zero or greater.");
      return;
    }

    try {
      await onSubmit(values);
      onClose();
    } catch (err) {
      setError(getApiErrorMessage(err, "Unable to save pantry item."));
    }
  }

  return (
    <Modal
      open={open}
      title={mode === "create" ? "Add pantry item" : `Edit ${item?.ingredient.name ?? "item"}`}
      onClose={onClose}
    >
      <form className="space-y-5" onSubmit={handleSubmit}>
        {error ? (
          <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
            {error}
          </div>
        ) : null}

        {mode === "create" ? (
          <div>
            <div className="mb-1.5 flex items-center justify-between gap-2">
              <Label htmlFor="ingredientId" className="mb-0">
                Ingredient
              </Label>
              <Button
                type="button"
                variant="ghost"
                className="h-auto px-2 py-1 text-sm text-brand-700"
                onClick={() => setShowAddIngredient(true)}
              >
                + New ingredient
              </Button>
            </div>
            <Select
              id="ingredientId"
              value={values.ingredientId}
              onChange={(event) => {
                const ingredientId = event.target.value;
                const ingredient = ingredientsData?.items.find((i) => i.id === ingredientId);
                updateField("ingredientId", ingredientId);
                if (ingredient?.defaultUnit) {
                  updateField("unit", ingredient.defaultUnit);
                }
              }}
              disabled={ingredientsLoading}
              required
            >
              <option value="">
                {ingredientsLoading
                  ? "Loading ingredients…"
                  : (ingredientsData?.items.length ?? 0) === 0
                    ? "No ingredients yet — add one above"
                    : "Select ingredient…"}
              </option>
              {(ingredientsData?.items ?? []).map((ingredient) => (
                <option key={ingredient.id} value={ingredient.id}>
                  {ingredient.name}
                  {ingredient.category ? ` (${ingredient.category})` : ""}
                </option>
              ))}
            </Select>
          </div>
        ) : (
          <div className="rounded-lg bg-slate-50 px-3 py-2 text-sm text-slate-700">
            <span className="font-medium">Ingredient:</span> {item?.ingredient.name}
          </div>
        )}

        <div className="grid gap-4 sm:grid-cols-2">
          <div>
            <Label htmlFor="quantity">Quantity</Label>
            <Input
              id="quantity"
              type="number"
              min={0}
              step="0.01"
              value={values.quantity}
              onChange={(event) => updateField("quantity", event.target.value)}
              required
            />
          </div>
          <div>
            <Label htmlFor="unit">Unit</Label>
            <Select
              id="unit"
              value={values.unit}
              onChange={(event) => updateField("unit", event.target.value)}
              required
            >
              {PANTRY_UNITS.map((unit) => (
                <option key={unit} value={unit}>
                  {unit}
                </option>
              ))}
            </Select>
          </div>
        </div>

        <div className="grid gap-4 sm:grid-cols-2">
          <div>
            <Label htmlFor="expirationDate">Expiration date</Label>
            <Input
              id="expirationDate"
              type="date"
              value={values.expirationDate}
              onChange={(event) => updateField("expirationDate", event.target.value)}
            />
          </div>
          <div>
            <Label htmlFor="lowStockThreshold">Low stock threshold</Label>
            <Input
              id="lowStockThreshold"
              type="number"
              min={0}
              step="0.01"
              value={values.lowStockThreshold}
              onChange={(event) => updateField("lowStockThreshold", event.target.value)}
              placeholder="Optional"
            />
          </div>
        </div>

        <div className="flex flex-col-reverse gap-3 border-t border-slate-100 pt-5 sm:flex-row sm:justify-end">
          <Button type="button" variant="secondary" onClick={onClose}>
            Cancel
          </Button>
          <Button type="submit" isLoading={isSubmitting}>
            {mode === "create" ? "Add item" : "Save changes"}
          </Button>
        </div>
      </form>

      <AddIngredientModal
        open={showAddIngredient}
        onClose={() => setShowAddIngredient(false)}
        onCreated={handleIngredientCreated}
      />
    </Modal>
  );
}
