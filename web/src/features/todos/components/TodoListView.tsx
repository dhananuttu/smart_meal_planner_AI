import { useMemo, useState } from "react";
import { EmptyState, LoadingSpinner, WidgetSkeleton } from "@/components/common";
import { Button, Label, Select } from "@/components/ui";
import {
  useCreateTodo,
  useDeleteTodo,
  useMarkTodoComplete,
  useOverdueTodos,
  useTodayTodos,
  useTodos,
  useUpcomingTodos,
  useUpdateTodo,
} from "@/hooks/useTodos";
import type { Todo, TodoFormValues, TodoListQuery } from "@/types/todo";
import { TODO_CATEGORIES, TODO_PRIORITIES } from "@/types/todo";
import { formatDateIso } from "@/utils/dateUtils";
import { getApiErrorMessage } from "@/utils/apiError";
import { TodoFormModal } from "./TodoFormModal";
import { TodoItemRow } from "./TodoItemRow";

type ViewMode = "all" | "today" | "overdue" | "upcoming";
type StatusFilter = "all" | "pending" | "completed";

interface FormState {
  mode: "create" | "edit";
  task?: Todo;
}

function toRequestValues(values: TodoFormValues) {
  return {
    title: values.title.trim(),
    description: values.description.trim() || undefined,
    category: values.category,
    priority: values.priority,
    dueDate: values.dueDate || undefined,
    isRecurring: values.isRecurring,
    recurrenceType: values.isRecurring ? values.recurrenceType : undefined,
  };
}

function applyClientFilters(
  tasks: Todo[],
  category: string,
  priority: string,
  status: StatusFilter,
): Todo[] {
  return tasks.filter((task) => {
    if (category !== "all" && task.category !== category) {
      return false;
    }
    if (priority !== "all" && task.priority !== priority) {
      return false;
    }
    if (status === "pending" && task.isCompleted) {
      return false;
    }
    if (status === "completed" && !task.isCompleted) {
      return false;
    }
    return true;
  });
}

const viewLabels: Record<ViewMode, string> = {
  all: "All tasks",
  today: "Today",
  overdue: "Overdue",
  upcoming: "Upcoming",
};

export function TodoListView() {
  const [view, setView] = useState<ViewMode>("all");
  const [page, setPage] = useState(1);
  const [categoryFilter, setCategoryFilter] = useState("all");
  const [priorityFilter, setPriorityFilter] = useState("all");
  const [statusFilter, setStatusFilter] = useState<StatusFilter>("pending");
  const [recurringOnly, setRecurringOnly] = useState(false);
  const [form, setForm] = useState<FormState | null>(null);

  const pageSize = 20;
  const todayIso = formatDateIso(new Date());

  const listQuery: TodoListQuery = {
    page,
    pageSize,
    category: categoryFilter !== "all" ? categoryFilter : undefined,
    priority: priorityFilter !== "all" ? priorityFilter : undefined,
    isCompleted:
      statusFilter === "pending" ? false : statusFilter === "completed" ? true : undefined,
    isRecurring: recurringOnly ? true : undefined,
  };

  const allQuery = useTodos(listQuery);
  const todayQuery = useTodayTodos();
  const overdueQuery = useOverdueTodos();
  const upcomingQuery = useUpcomingTodos();

  const createTodo = useCreateTodo();
  const updateTodo = useUpdateTodo();
  const markComplete = useMarkTodoComplete();
  const deleteTodo = useDeleteTodo();

  const activeQuery =
    view === "today"
      ? todayQuery
      : view === "overdue"
        ? overdueQuery
        : view === "upcoming"
          ? upcomingQuery
          : allQuery;

  const tasks = useMemo(() => {
    if (view === "all") {
      return allQuery.data?.items ?? [];
    }

    const source =
      view === "today"
        ? todayQuery.data ?? []
        : view === "overdue"
          ? overdueQuery.data ?? []
          : upcomingQuery.data ?? [];

    return applyClientFilters(source, categoryFilter, priorityFilter, statusFilter).filter(
      (task) => !recurringOnly || task.isRecurring,
    );
  }, [
    view,
    allQuery.data,
    todayQuery.data,
    overdueQuery.data,
    upcomingQuery.data,
    categoryFilter,
    priorityFilter,
    statusFilter,
    recurringOnly,
  ]);

  const counts = {
    today: todayQuery.data?.filter((task) => !task.isCompleted).length ?? 0,
    overdue: overdueQuery.data?.length ?? 0,
    upcoming: upcomingQuery.data?.length ?? 0,
  };

  const isSubmitting = createTodo.isPending || updateTodo.isPending;

  function handleViewChange(nextView: ViewMode) {
    setView(nextView);
    setPage(1);
  }

  function handleFilterChange() {
    setPage(1);
  }

  async function handleFormSubmit(values: TodoFormValues) {
    const payload = toRequestValues(values);

    if (form?.mode === "create") {
      await createTodo.mutateAsync(payload);
      return;
    }

    if (form?.task) {
      await updateTodo.mutateAsync({
        id: form.task.id,
        request: {
          ...payload,
          isCompleted: form.task.isCompleted,
        },
      });
    }
  }

  async function handleToggleComplete(task: Todo, isCompleted: boolean) {
    try {
      await markComplete.mutateAsync({ id: task.id, request: { isCompleted } });
    } catch (error) {
      window.alert(getApiErrorMessage(error, "Unable to update task."));
    }
  }

  async function handleDelete(task: Todo) {
    const confirmed = window.confirm(`Delete "${task.title}"?`);
    if (!confirmed) {
      return;
    }

    try {
      await deleteTodo.mutateAsync(task.id);
    } catch (error) {
      window.alert(getApiErrorMessage(error, "Unable to delete task."));
    }
  }

  if (activeQuery.isLoading && !activeQuery.data) {
    return <LoadingSpinner label="Loading tasks…" />;
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div>
          <p className="text-sm font-medium text-brand-700">Tasks</p>
          <h1 className="mt-1 text-2xl font-bold text-slate-900 sm:text-3xl">Todo list</h1>
          <p className="mt-2 text-sm text-slate-600">
            Track meal prep, shopping, and household chores in one place.
          </p>
        </div>
        <Button onClick={() => setForm({ mode: "create" })}>New task</Button>
      </div>

      <div className="-mx-1 flex gap-2 overflow-x-auto px-1 pb-1">
        {(["all", "today", "overdue", "upcoming"] as ViewMode[]).map((mode) => (
          <Button
            key={mode}
            variant={view === mode ? "primary" : "secondary"}
            className="shrink-0"
            onClick={() => handleViewChange(mode)}
          >
            {viewLabels[mode]}
            {mode === "today" && counts.today > 0 ? ` (${counts.today})` : ""}
            {mode === "overdue" && counts.overdue > 0 ? ` (${counts.overdue})` : ""}
            {mode === "upcoming" && counts.upcoming > 0 ? ` (${counts.upcoming})` : ""}
          </Button>
        ))}
      </div>

      <div className="grid gap-4 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm sm:grid-cols-2 lg:grid-cols-4">
        <div>
          <Label htmlFor="categoryFilter">Category</Label>
          <Select
            id="categoryFilter"
            value={categoryFilter}
            onChange={(event) => {
              setCategoryFilter(event.target.value);
              handleFilterChange();
            }}
          >
            <option value="all">All categories</option>
            {TODO_CATEGORIES.map((category) => (
              <option key={category} value={category}>
                {category}
              </option>
            ))}
          </Select>
        </div>

        <div>
          <Label htmlFor="priorityFilter">Priority</Label>
          <Select
            id="priorityFilter"
            value={priorityFilter}
            onChange={(event) => {
              setPriorityFilter(event.target.value);
              handleFilterChange();
            }}
          >
            <option value="all">All priorities</option>
            {TODO_PRIORITIES.map((priority) => (
              <option key={priority} value={priority}>
                {priority}
              </option>
            ))}
          </Select>
        </div>

        <div>
          <Label htmlFor="statusFilter">Status</Label>
          <Select
            id="statusFilter"
            value={statusFilter}
            onChange={(event) => {
              setStatusFilter(event.target.value as StatusFilter);
              handleFilterChange();
            }}
          >
            <option value="pending">Pending</option>
            <option value="completed">Completed</option>
            <option value="all">All</option>
          </Select>
        </div>

        <div className="flex items-end">
          <label className="flex w-full items-center gap-2 rounded-xl border border-slate-200 bg-slate-50 px-3 py-2.5 text-sm text-slate-700">
            <input
              type="checkbox"
              checked={recurringOnly}
              onChange={(event) => {
                setRecurringOnly(event.target.checked);
                handleFilterChange();
              }}
              className="h-4 w-4 rounded border-slate-300 text-brand-600 focus:ring-brand-500"
            />
            Recurring only
          </label>
        </div>
      </div>

      {view === "overdue" && counts.overdue > 0 ? (
        <div className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          You have {counts.overdue} overdue {counts.overdue === 1 ? "task" : "tasks"}.
        </div>
      ) : null}

      <section className="rounded-2xl border border-slate-200 bg-white shadow-sm">
        {activeQuery.isLoading ? (
          <div className="p-6">
            <WidgetSkeleton rows={5} />
          </div>
        ) : activeQuery.isError ? (
          <div className="p-6 text-sm text-red-700">Unable to load tasks.</div>
        ) : tasks.length === 0 ? (
          <EmptyState
            message={
              view === "overdue"
                ? "No overdue tasks — you're caught up!"
                : view === "upcoming"
                  ? "No upcoming tasks scheduled."
                  : view === "today"
                    ? "Nothing due today."
                    : "No tasks match your filters."
            }
            action={
              <Button onClick={() => setForm({ mode: "create" })}>Create a task</Button>
            }
          />
        ) : (
          <ul className="space-y-2 p-4">
            {tasks.map((task) => (
              <TodoItemRow
                key={task.id}
                task={task}
                isUpdating={markComplete.isPending}
                isDeleting={deleteTodo.isPending}
                showDueDate={view !== "today"}
                isOverdue={Boolean(
                  task.dueDate && task.dueDate < todayIso && !task.isCompleted,
                )}
                onToggleComplete={handleToggleComplete}
                onEdit={(item) => setForm({ mode: "edit", task: item })}
                onDelete={handleDelete}
              />
            ))}
          </ul>
        )}
      </section>

      {view === "all" && allQuery.data && allQuery.data.totalPages > 1 ? (
        <div className="flex flex-col items-center justify-between gap-3 sm:flex-row">
          <p className="text-sm text-slate-600">
            Page {allQuery.data.page} of {allQuery.data.totalPages} · {allQuery.data.totalCount}{" "}
            tasks
          </p>
          <div className="flex gap-2">
            <Button
              variant="secondary"
              disabled={!allQuery.data.hasPreviousPage}
              onClick={() => setPage((p) => Math.max(1, p - 1))}
            >
              Previous
            </Button>
            <Button
              variant="secondary"
              disabled={!allQuery.data.hasNextPage}
              onClick={() => setPage((p) => p + 1)}
            >
              Next
            </Button>
          </div>
        </div>
      ) : null}

      <TodoFormModal
        open={form !== null}
        mode={form?.mode ?? "create"}
        task={form?.task}
        isSubmitting={isSubmitting}
        onClose={() => setForm(null)}
        onSubmit={handleFormSubmit}
      />
    </div>
  );
}
