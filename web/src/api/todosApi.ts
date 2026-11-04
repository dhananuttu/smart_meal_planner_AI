import { apiClient } from "./client";
import type { ApiResult, PaginatedList } from "@/types/api";
import type {
  CreateTodoRequest,
  MarkTodoCompleteRequest,
  Todo,
  TodoListQuery,
  UpdateTodoRequest,
} from "@/types/todo";

export async function getTodos(query: TodoListQuery = {}): Promise<PaginatedList<Todo>> {
  const { data } = await apiClient.get<ApiResult<PaginatedList<Todo>>>("/api/todos", {
    params: query,
  });
  return data.data;
}

export async function getToday(): Promise<Todo[]> {
  const { data } = await apiClient.get<ApiResult<Todo[]>>("/api/todos/today");
  return data.data;
}

export async function getUpcoming(): Promise<Todo[]> {
  const { data } = await apiClient.get<ApiResult<Todo[]>>("/api/todos/upcoming");
  return data.data;
}

export async function getOverdue(): Promise<Todo[]> {
  const { data } = await apiClient.get<ApiResult<Todo[]>>("/api/todos/overdue");
  return data.data;
}

export async function getRecurring(): Promise<Todo[]> {
  const { data } = await apiClient.get<ApiResult<Todo[]>>("/api/todos/recurring");
  return data.data;
}

export async function getById(id: string): Promise<Todo> {
  const { data } = await apiClient.get<ApiResult<Todo>>(`/api/todos/${id}`);
  return data.data;
}

export async function createTodo(request: CreateTodoRequest): Promise<Todo> {
  const { data } = await apiClient.post<ApiResult<Todo>>("/api/todos", request);
  return data.data;
}

export async function updateTodo(id: string, request: UpdateTodoRequest): Promise<Todo> {
  const { data } = await apiClient.put<ApiResult<Todo>>(`/api/todos/${id}`, request);
  return data.data;
}

export async function markComplete(id: string, request: MarkTodoCompleteRequest): Promise<Todo> {
  const { data } = await apiClient.patch<ApiResult<Todo>>(
    `/api/todos/${id}/complete`,
    request,
  );
  return data.data;
}

export async function deleteTodo(id: string): Promise<void> {
  await apiClient.delete(`/api/todos/${id}`);
}
