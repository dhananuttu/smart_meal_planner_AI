import { Button } from "@/components/ui";
import type { Todo } from "@/types/todo";
import { formatShortDay } from "@/utils/dateUtils";

interface TodoItemRowProps {
  task: Todo;
  isUpdating: boolean;
  isDeleting: boolean;
  showDueDate?: boolean;
  isOverdue?: boolean;
  onToggleComplete: (task: Todo, isCompleted: boolean) => void;
  onEdit: (task: Todo) => void;
  onDelete: (task: Todo) => void;
}

const priorityStyles: Record<string, string> = {
  Low: "bg-slate-100 text-slate-600",
  Medium: "bg-blue-50 text-blue-700",
  High: "bg-orange-50 text-orange-700",
  Critical: "bg-red-50 text-red-700",
};

export function TodoItemRow({
  task,
  isUpdating,
  isDeleting,
  showDueDate = true,
  isOverdue = false,
  onToggleComplete,
  onEdit,
  onDelete,
}: TodoItemRowProps) {
  return (
    <li
      className={`flex flex-col gap-3 rounded-xl border px-4 py-3 transition sm:flex-row sm:items-center ${
        task.isCompleted
          ? "border-slate-200 bg-slate-50"
          : isOverdue
            ? "border-red-200 bg-red-50/40 hover:border-red-300"
            : "border-slate-200 bg-white hover:border-brand-200"
      }`}
    >
      <div className="flex min-w-0 flex-1 items-start gap-3">
        <input
          type="checkbox"
          checked={task.isCompleted}
          disabled={isUpdating || isDeleting}
          onChange={(event) => onToggleComplete(task, event.target.checked)}
          className="mt-1 h-4 w-4 shrink-0 rounded border-slate-300 text-brand-600 focus:ring-brand-500"
          aria-label={`Mark ${task.title} as complete`}
        />

        <button
          type="button"
          onClick={() => onEdit(task)}
          className="min-w-0 flex-1 text-left"
        >
          <p
            className={`font-medium ${
              task.isCompleted ? "text-slate-400 line-through" : "text-slate-900"
            }`}
          >
            {task.title}
          </p>
          {task.description ? (
            <p
              className={`mt-0.5 line-clamp-2 text-sm ${
                task.isCompleted ? "text-slate-400" : "text-slate-600"
              }`}
            >
              {task.description}
            </p>
          ) : null}
          <div className="mt-2 flex flex-wrap items-center gap-2">
            <span className="rounded-full bg-slate-100 px-2 py-0.5 text-xs font-medium text-slate-600">
              {task.category}
            </span>
            <span
              className={`rounded-full px-2 py-0.5 text-xs font-semibold ${
                priorityStyles[task.priority] ?? "bg-slate-100 text-slate-600"
              }`}
            >
              {task.priority}
            </span>
            {task.isRecurring && task.recurrenceType ? (
              <span className="rounded-full bg-violet-50 px-2 py-0.5 text-xs font-medium text-violet-700">
                {task.recurrenceType}
              </span>
            ) : null}
            {showDueDate && task.dueDate ? (
              <span
                className={`text-xs font-medium ${
                  isOverdue && !task.isCompleted ? "text-red-600" : "text-slate-500"
                }`}
              >
                {formatShortDay(task.dueDate)}
              </span>
            ) : null}
          </div>
        </button>
      </div>

      <div className="flex shrink-0 items-center gap-2 self-end sm:self-center">
        <Button variant="ghost" disabled={isDeleting} onClick={() => onEdit(task)}>
          Edit
        </Button>
        <Button
          variant="ghost"
          className="text-red-600 hover:bg-red-50"
          disabled={isDeleting}
          onClick={() => onDelete(task)}
        >
          Delete
        </Button>
      </div>
    </li>
  );
}
