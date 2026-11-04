import { FormEvent, useEffect, useState } from "react";
import { Button, Input, Label, Modal } from "@/components/ui";
import { getWeekStartMonday } from "@/utils/dateUtils";
import { getApiErrorMessage } from "@/utils/apiError";

interface GenerateGroceryListModalProps {
  open: boolean;
  isSubmitting: boolean;
  onClose: () => void;
  onGenerate: (weekStart: string, name: string) => Promise<void>;
}

export function GenerateGroceryListModal({
  open,
  isSubmitting,
  onClose,
  onGenerate,
}: GenerateGroceryListModalProps) {
  const [weekStart, setWeekStart] = useState(() => getWeekStartMonday(new Date()));
  const [name, setName] = useState("");
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (open) {
      setWeekStart(getWeekStartMonday(new Date()));
      setName("");
      setError(null);
    }
  }, [open]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    try {
      await onGenerate(weekStart, name.trim());
      onClose();
    } catch (err) {
      setError(getApiErrorMessage(err, "Unable to generate grocery list."));
    }
  }

  return (
    <Modal open={open} title="Generate from meal plan" onClose={onClose}>
      <form className="space-y-5" onSubmit={handleSubmit}>
        {error ? (
          <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
            {error}
          </div>
        ) : null}

        <p className="text-sm text-slate-600">
          Aggregates ingredients from all recipes in the selected week&apos;s meal plan. Each
          generate creates a <strong>new</strong> list — use a distinct name if you already have one
          for that week.
        </p>

        <div>
          <Label htmlFor="weekStart">Week starting (Monday)</Label>
          <Input
            id="weekStart"
            type="date"
            value={weekStart}
            onChange={(event) => setWeekStart(event.target.value)}
            required
          />
        </div>

        <div>
          <Label htmlFor="listName">List name (optional)</Label>
          <Input
            id="listName"
            value={name}
            onChange={(event) => setName(event.target.value)}
            placeholder="Grocery List – 2026-06-16"
            maxLength={200}
          />
        </div>

        <div className="flex flex-col-reverse gap-3 border-t border-slate-100 pt-5 sm:flex-row sm:justify-end">
          <Button type="button" variant="secondary" onClick={onClose}>
            Cancel
          </Button>
          <Button type="submit" isLoading={isSubmitting}>
            Generate list
          </Button>
        </div>
      </form>
    </Modal>
  );
}
