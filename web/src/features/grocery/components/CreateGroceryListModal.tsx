import { FormEvent, useEffect, useState } from "react";
import { Button, Input, Label, Modal } from "@/components/ui";
import { formatDateIso } from "@/utils/dateUtils";
import { getApiErrorMessage } from "@/utils/apiError";

function defaultShoppingListName(): string {
  const now = new Date();
  const time = now.toLocaleTimeString(undefined, {
    hour: "numeric",
    minute: "2-digit",
  });
  return `Shopping list – ${formatDateIso(now)} ${time}`;
}

interface CreateGroceryListModalProps {
  open: boolean;
  isSubmitting: boolean;
  onClose: () => void;
  onCreate: (name: string) => Promise<void>;
}

export function CreateGroceryListModal({
  open,
  isSubmitting,
  onClose,
  onCreate,
}: CreateGroceryListModalProps) {
  const [name, setName] = useState(defaultShoppingListName);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (open) {
      setName(defaultShoppingListName());
      setError(null);
    }
  }, [open]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    if (!name.trim()) {
      setError("List name is required.");
      return;
    }

    try {
      await onCreate(name.trim());
      onClose();
    } catch (err) {
      setError(getApiErrorMessage(err, "Unable to create grocery list."));
    }
  }

  return (
    <Modal open={open} title="New shopping list" onClose={onClose}>
      <form className="space-y-5" onSubmit={handleSubmit}>
        {error ? (
          <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
            {error}
          </div>
        ) : null}

        <p className="text-sm text-slate-600">
          Start a list you build yourself — add items as you shop. You can edit the suggested name
          below.
        </p>

        <div>
          <Label htmlFor="shoppingListName">List name</Label>
          <Input
            id="shoppingListName"
            value={name}
            onChange={(event) => setName(event.target.value)}
            required
            maxLength={200}
            autoFocus
          />
        </div>

        <div className="flex flex-col-reverse gap-3 border-t border-slate-100 pt-5 sm:flex-row sm:justify-end">
          <Button type="button" variant="secondary" onClick={onClose}>
            Cancel
          </Button>
          <Button type="submit" isLoading={isSubmitting}>
            Create shopping list
          </Button>
        </div>
      </form>
    </Modal>
  );
}
