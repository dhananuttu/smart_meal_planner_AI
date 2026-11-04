import { FormEvent, useEffect, useState } from "react";
import { Button, Input, Label, Modal, Select, Textarea } from "@/components/ui";
import type { Todo, TodoFormValues } from "@/types/todo";
import {
  TODO_CATEGORIES,
  TODO_PRIORITIES,
  TODO_RECURRENCE_TYPES,
  emptyTodoFormValues,
  todoToFormValues,
} from "@/types/todo";
import { getApiErrorMessage } from "@/utils/apiError";

interface TodoFormModalProps {
  open: boolean;
  mode: "create" | "edit";
  task?: Todo;
  isSubmitting: boolean;
  onClose: () => void;
  onSubmit: (values: TodoFormValues) => Promise<void>;
}

export function TodoFormModal({
  open,
  mode,
  task,
  isSubmitting,
  onClose,
  onSubmit,
}: TodoFormModalProps) {
  const [values, setValues] = useState<TodoFormValues>(emptyTodoFormValues);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (open) {
      setValues(mode === "edit" && task ? todoToFormValues(task) : emptyTodoFormValues);
      setError(null);
    }
  }, [open, mode, task]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    if (!values.title.trim()) {
      setError("Title is required.");
      return;
    }

    if (values.isRecurring && !values.recurrenceType) {
      setError("Select a recurrence type.");
      return;
    }

    try {
      await onSubmit(values);
      onClose();
    } catch (err) {
      setError(getApiErrorMessage(err, "Unable to save task."));
    }
  }

  return (
    <Modal
      open={open}
      title={mode === "create" ? "New task" : "Edit task"}
      onClose={onClose}
    >
      <form className="space-y-5" onSubmit={handleSubmit}>
        {error ? (
          <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
            {error}
          </div>
        ) : null}

        <div>
          <Label htmlFor="todoTitle">Title</Label>
          <Input
            id="todoTitle"
            value={values.title}
            onChange={(event) => setValues((v) => ({ ...v, title: event.target.value }))}
            placeholder="Prep vegetables, buy milk…"
            required
            maxLength={200}
          />
        </div>

        <div>
          <Label htmlFor="todoDescription">Description (optional)</Label>
          <Textarea
            id="todoDescription"
            value={values.description}
            onChange={(event) => setValues((v) => ({ ...v, description: event.target.value }))}
            rows={3}
            maxLength={1000}
          />
        </div>

        <div className="grid gap-4 sm:grid-cols-2">
          <div>
            <Label htmlFor="todoCategory">Category</Label>
            <Select
              id="todoCategory"
              value={values.category}
              onChange={(event) => setValues((v) => ({ ...v, category: event.target.value }))}
            >
              {TODO_CATEGORIES.map((category) => (
                <option key={category} value={category}>
                  {category}
                </option>
              ))}
            </Select>
          </div>
          <div>
            <Label htmlFor="todoPriority">Priority</Label>
            <Select
              id="todoPriority"
              value={values.priority}
              onChange={(event) => setValues((v) => ({ ...v, priority: event.target.value }))}
            >
              {TODO_PRIORITIES.map((priority) => (
                <option key={priority} value={priority}>
                  {priority}
                </option>
              ))}
            </Select>
          </div>
        </div>

        <div>
          <Label htmlFor="todoDueDate">Due date (optional)</Label>
          <Input
            id="todoDueDate"
            type="date"
            value={values.dueDate}
            onChange={(event) => setValues((v) => ({ ...v, dueDate: event.target.value }))}
          />
        </div>

        <div className="rounded-xl border border-slate-200 bg-slate-50 p-4">
          <label className="flex items-center gap-2 text-sm font-medium text-slate-700">
            <input
              type="checkbox"
              checked={values.isRecurring}
              onChange={(event) =>
                setValues((v) => ({ ...v, isRecurring: event.target.checked }))
              }
              className="h-4 w-4 rounded border-slate-300 text-brand-600 focus:ring-brand-500"
            />
            Recurring task
          </label>

          {values.isRecurring ? (
            <div className="mt-3">
              <Label htmlFor="todoRecurrence">Repeats</Label>
              <Select
                id="todoRecurrence"
                value={values.recurrenceType}
                onChange={(event) =>
                  setValues((v) => ({ ...v, recurrenceType: event.target.value }))
                }
              >
                {TODO_RECURRENCE_TYPES.map((type) => (
                  <option key={type} value={type}>
                    {type}
                  </option>
                ))}
              </Select>
            </div>
          ) : null}
        </div>

        <div className="flex flex-col-reverse gap-3 border-t border-slate-100 pt-5 sm:flex-row sm:justify-end">
          <Button type="button" variant="secondary" onClick={onClose}>
            Cancel
          </Button>
          <Button type="submit" isLoading={isSubmitting}>
            {mode === "create" ? "Create task" : "Save changes"}
          </Button>
        </div>
      </form>
    </Modal>
  );
}
