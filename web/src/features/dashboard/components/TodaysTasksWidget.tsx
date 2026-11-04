import { Link } from "react-router-dom";
import type { Todo } from "@/types/todo";
import { DashboardWidget } from "./DashboardWidget";

interface TodaysTasksWidgetProps {
  tasks: Todo[];
  isLoading: boolean;
  isError: boolean;
}

const priorityStyles: Record<string, string> = {
  Low: "text-slate-500",
  Medium: "text-slate-700",
  High: "text-orange-600",
  Critical: "text-red-600",
};

export function TodaysTasksWidget({ tasks, isLoading, isError }: TodaysTasksWidgetProps) {
  const pending = tasks.filter((task) => !task.isCompleted);

  return (
    <DashboardWidget
      title="Today's Tasks"
      href="/todos"
      isLoading={isLoading}
      isError={isError}
      isEmpty={pending.length === 0}
      emptyMessage={tasks.length > 0 ? "All tasks done for today!" : "No tasks due today."}
      emptyAction={
        <Link
          to="/todos"
          className="text-sm font-medium text-brand-700 hover:text-brand-600"
        >
          Add a task
        </Link>
      }
    >
      <ul className="space-y-2">
        {pending.slice(0, 5).map((task) => (
          <li
            key={task.id}
            className="flex items-center justify-between gap-3 rounded-lg border border-slate-100 px-3 py-2.5"
          >
            <div className="min-w-0">
              <p className="truncate text-sm font-medium text-slate-900">{task.title}</p>
              <p className="truncate text-xs text-slate-500">{task.category}</p>
            </div>
            <span
              className={`shrink-0 text-xs font-semibold ${
                priorityStyles[task.priority] ?? "text-slate-600"
              }`}
            >
              {task.priority}
            </span>
          </li>
        ))}
      </ul>
      {pending.length > 5 ? (
        <p className="mt-3 text-xs text-slate-500">+{pending.length - 5} more tasks</p>
      ) : null}
    </DashboardWidget>
  );
}
