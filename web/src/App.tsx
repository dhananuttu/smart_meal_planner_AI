import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { GuestRoute, ProtectedRoute } from "@/components/routes/AuthRoutes";
import { AuthProvider } from "@/contexts/AuthContext";
import { AppLayout, AuthLayout } from "@/layouts";
import {
  AlertsPage,
  CreateRecipePage,
  DashboardPage,
  EditRecipePage,
  GroceryListPage,
  LoginPage,
  MealPlannerPage,
  PantryPage,
  ProfilePage,
  RecipeDetailPage,
  RecipesPage,
  RegisterPage,
  TodosPage,
} from "@/pages";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000,
      retry: 1,
    },
  },
});

export function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <BrowserRouter>
          <Routes>
            <Route element={<GuestRoute />}>
              <Route element={<AuthLayout />}>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
              </Route>
            </Route>

            <Route element={<ProtectedRoute />}>
              <Route element={<AppLayout />}>
                <Route path="/" element={<Navigate to="/dashboard" replace />} />
                <Route path="/dashboard" element={<DashboardPage />} />
                <Route path="/profile" element={<ProfilePage />} />
                <Route path="/recipes" element={<RecipesPage />} />
                <Route path="/recipes/new" element={<CreateRecipePage />} />
                <Route path="/recipes/:id/edit" element={<EditRecipePage />} />
                <Route path="/recipes/:id" element={<RecipeDetailPage />} />
                <Route path="/meal-planner" element={<MealPlannerPage />} />
                <Route path="/pantry" element={<PantryPage />} />
                <Route path="/grocery" element={<GroceryListPage />} />
                <Route path="/todos" element={<TodosPage />} />
                <Route path="/alerts" element={<AlertsPage />} />
              </Route>
            </Route>

            <Route path="*" element={<Navigate to="/dashboard" replace />} />
          </Routes>
        </BrowserRouter>
      </AuthProvider>
    </QueryClientProvider>
  );
}
