/**
 * Cross-cutting React hooks.
 * Feature-specific data hooks may live in feature folders or here when shared.
 */

export { useAuth, AuthProvider } from "./useAuth";
export {
  useRecipes,
  useRecipe,
  useRecipeCategories,
  useCreateRecipeCategory,
  useCreateRecipe,
  useUpdateRecipe,
  useDeleteRecipe,
  recipeKeys,
  recipeCategoryKeys,
} from "./useRecipes";
export {
  useWeeklyMealPlan,
  useCreateMeal,
  useUpdateMeal,
  useDeleteMeal,
  mealPlanKeys,
} from "./useMealPlan";
export {
  usePantryInventory,
  useCreatePantryItem,
  useUpdatePantryItem,
  useDeletePantryItem,
  pantryKeys,
} from "./usePantry";
export { useIngredients, useCreateIngredient, ingredientKeys } from "./useIngredients";
export {
  useGroceryLists,
  useGroceryList,
  useCreateGroceryList,
  useGenerateGroceryList,
  useAddGroceryItem,
  useMarkGroceryItemPurchased,
  useDeleteGroceryItem,
  groceryKeys,
} from "./useGrocery";
export {
  useTodos,
  useTodayTodos,
  useUpcomingTodos,
  useOverdueTodos,
  useCreateTodo,
  useUpdateTodo,
  useMarkTodoComplete,
  useDeleteTodo,
  todoKeys,
} from "./useTodos";
