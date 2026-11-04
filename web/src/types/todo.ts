export interface Todo {
  id: string;
  title: string;
  description?: string;
  category: string;
  priority: string;
  dueDate?: string;
  isCompleted: boolean;
  isRecurring: boolean;
  recurrenceType?: string;
  mealPlanEntryId?: string;
  completedAt?: string;
  createdAt: string;
  updatedAt: string;
}

export interface TodoListQuery {
  page?: number;
  pageSize?: number;
  category?: string;
  priority?: string;
  isCompleted?: boolean;
  isRecurring?: boolean;
}

export interface CreateTodoRequest {
  title: string;
  description?: string;
  category: string;
  priority: string;
  dueDate?: string;
  isRecurring: boolean;
  recurrenceType?: string;
  mealPlanEntryId?: string;
}

export interface UpdateTodoRequest {
  title: string;
  description?: string;
  category: string;
  priority: string;
  dueDate?: string;
  isCompleted: boolean;
  isRecurring: boolean;
  recurrenceType?: string;
  mealPlanEntryId?: string;
}

export interface MarkTodoCompleteRequest {
  isCompleted: boolean;
}

export interface TodoFormValues {
  title: string;
  description: string;
  category: string;
  priority: string;
  dueDate: string;
  isRecurring: boolean;
  recurrenceType: string;
}

export const TODO_CATEGORIES = [
  "Meal Preparation",
  "Shopping",
  "Household",
  "Personal",
] as const;

export const TODO_PRIORITIES = ["Low", "Medium", "High", "Critical"] as const;

export const TODO_RECURRENCE_TYPES = ["Daily", "Weekly", "Monthly", "Yearly"] as const;

export const emptyTodoFormValues: TodoFormValues = {
  title: "",
  description: "",
  category: "Personal",
  priority: "Medium",
  dueDate: "",
  isRecurring: false,
  recurrenceType: "Weekly",
};

export function todoToFormValues(todo: Todo): TodoFormValues {
  return {
    title: todo.title,
    description: todo.description ?? "",
    category: todo.category,
    priority: todo.priority,
    dueDate: todo.dueDate ?? "",
    isRecurring: todo.isRecurring,
    recurrenceType: todo.recurrenceType ?? "Weekly",
  };
}
