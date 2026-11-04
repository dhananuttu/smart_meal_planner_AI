import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import * as todosApi from "@/api/todosApi";
import type {
  CreateTodoRequest,
  MarkTodoCompleteRequest,
  TodoListQuery,
  UpdateTodoRequest,
} from "@/types/todo";

export const todoKeys = {
  all: ["todos"] as const,
  lists: () => [...todoKeys.all, "list"] as const,
  list: (query: TodoListQuery) => [...todoKeys.lists(), query] as const,
  today: () => [...todoKeys.all, "today"] as const,
  upcoming: () => [...todoKeys.all, "upcoming"] as const,
  overdue: () => [...todoKeys.all, "overdue"] as const,
  recurring: () => [...todoKeys.all, "recurring"] as const,
  detail: (id: string) => [...todoKeys.all, "detail", id] as const,
};

function invalidateAllTodos(queryClient: ReturnType<typeof useQueryClient>) {
  queryClient.invalidateQueries({ queryKey: todoKeys.all });
  queryClient.invalidateQueries({ queryKey: ["dashboard", "todos"] });
}

export function useTodos(query: TodoListQuery) {
  return useQuery({
    queryKey: todoKeys.list(query),
    queryFn: () => todosApi.getTodos(query),
  });
}

export function useTodayTodos() {
  return useQuery({
    queryKey: todoKeys.today(),
    queryFn: todosApi.getToday,
  });
}

export function useUpcomingTodos() {
  return useQuery({
    queryKey: todoKeys.upcoming(),
    queryFn: todosApi.getUpcoming,
  });
}

export function useOverdueTodos() {
  return useQuery({
    queryKey: todoKeys.overdue(),
    queryFn: todosApi.getOverdue,
  });
}

export function useCreateTodo() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateTodoRequest) => todosApi.createTodo(request),
    onSuccess: () => invalidateAllTodos(queryClient),
  });
}

export function useUpdateTodo() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, request }: { id: string; request: UpdateTodoRequest }) =>
      todosApi.updateTodo(id, request),
    onSuccess: () => invalidateAllTodos(queryClient),
  });
}

export function useMarkTodoComplete() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, request }: { id: string; request: MarkTodoCompleteRequest }) =>
      todosApi.markComplete(id, request),
    onSuccess: () => invalidateAllTodos(queryClient),
  });
}

export function useDeleteTodo() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => todosApi.deleteTodo(id),
    onSuccess: () => invalidateAllTodos(queryClient),
  });
}
