import { FormEvent, useEffect, useState } from "react";
import { Button, Input, Label, Modal, Select, Textarea } from "@/components/ui";
import type { AddGroceryItemFormValues } from "@/types/grocery";
import { PANTRY_UNITS } from "@/types/pantry";
import { emptyGroceryItemFormValues } from "@/types/grocery";
import { getApiErrorMessage } from "@/utils/apiError";

interface AddGroceryItemModalProps {
  open: boolean;
  isSubmitting: boolean;
  onClose: () => void;
  onSubmit: (values: AddGroceryItemFormValues) => Promise<void>;
}

export function AddGroceryItemModal({
  open,
  isSubmitting,
  onClose,
  onSubmit,
}: AddGroceryItemModalProps) {
  const [values, setValues] = useState<AddGroceryItemFormValues>(emptyGroceryItemFormValues);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (open) {
      setValues(emptyGroceryItemFormValues);
      setError(null);
    }
  }, [open]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    if (!values.name.trim()) {
      setError("Item name is required.");
      return;
    }

    if (!values.quantity || Number(values.quantity) <= 0) {
      setError("Quantity must be greater than zero.");
      return;
    }

    try {
      await onSubmit(values);
      onClose();
    } catch (err) {
      setError(getApiErrorMessage(err, "Unable to add item."));
    }
  }

  return (
    <Modal open={open} title="Add grocery item" onClose={onClose}>
      <form className="space-y-5" onSubmit={handleSubmit}>
        {error ? (
          <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
            {error}
          </div>
        ) : null}

        <div>
          <Label htmlFor="itemName">Name</Label>
          <Input
            id="itemName"
            value={values.name}
            onChange={(event) => setValues((v) => ({ ...v, name: event.target.value }))}
            placeholder="Milk, eggs, onions…"
            required
            maxLength={200}
          />
        </div>

        <div className="grid gap-4 sm:grid-cols-2">
          <div>
            <Label htmlFor="quantity">Quantity</Label>
            <Input
              id="quantity"
              type="number"
              min={0.01}
              step="0.01"
              value={values.quantity}
              onChange={(event) => setValues((v) => ({ ...v, quantity: event.target.value }))}
              required
            />
          </div>
          <div>
            <Label htmlFor="unit">Unit</Label>
            <Select
              id="unit"
              value={values.unit}
              onChange={(event) => setValues((v) => ({ ...v, unit: event.target.value }))}
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

        <div>
          <Label htmlFor="note">Note (optional)</Label>
          <Textarea
            id="note"
            value={values.note}
            onChange={(event) => setValues((v) => ({ ...v, note: event.target.value }))}
            rows={2}
            maxLength={500}
          />
        </div>

        <div className="flex flex-col-reverse gap-3 border-t border-slate-100 pt-5 sm:flex-row sm:justify-end">
          <Button type="button" variant="secondary" onClick={onClose}>
            Cancel
          </Button>
          <Button type="submit" isLoading={isSubmitting}>
            Add item
          </Button>
        </div>
      </form>
    </Modal>
  );
}
