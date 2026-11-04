/**
 * Shared TypeScript types aligned with backend DTOs.
 * One file per domain; re-export from index.ts.
 */

export type { ApiResult, ApiProblemDetails, PaginatedList } from "./api";
export type {
  AuthResponse,
  AuthSession,
  AuthUser,
  LoginRequest,
  LogoutRequest,
  RefreshTokenRequest,
  RegisterRequest,
} from "./auth";
export type {
  Meal,
  MealRecipeSummary,
  DailyMeals,
  WeeklyMealPlan,
  CreateMealRequest,
  UpdateMealRequest,
  MealSlotType,
} from "./mealPlan";
export { MEAL_SLOTS } from "./mealPlan";
export type {
  Recipe,
  RecipeCategorySummary,
  RecipeListQuery,
  CreateRecipeRequest,
  UpdateRecipeRequest,
  RecipeFormValues,
} from "./recipe";
export type { RecipeCategory, RecipeCategoryListQuery, CreateRecipeCategoryRequest, RecipeCategoryFormValues } from "./recipeCategory";
export {
  CUISINE_SUGGESTIONS,
  RECIPE_CATEGORY_COLOR_PRESETS,
  emptyRecipeCategoryFormValues,
} from "./recipeCategory";
export type {
  PantryItem,
  PantryIngredientSummary,
  PantryInventoryQuery,
  CreatePantryItemRequest,
  UpdatePantryItemRequest,
  PantryItemFormValues,
} from "./pantry";
export { PANTRY_UNITS } from "./pantry";
export type {
  Ingredient,
  IngredientListQuery,
  CreateIngredientRequest,
  IngredientFormValues,
} from "./ingredient";
export { INGREDIENT_CATEGORIES, emptyIngredientFormValues } from "./ingredient";
export type {
  GroceryListSummary,
  GroceryListDetail,
  GroceryItem,
  CreateGroceryListRequest,
  GenerateGroceryListFromMealPlanRequest,
  AddGroceryItemRequest,
  MarkGroceryItemPurchasedRequest,
  AddGroceryItemFormValues,
} from "./grocery";
export { emptyGroceryItemFormValues } from "./grocery";
export type {
  Todo,
  TodoListQuery,
  CreateTodoRequest,
  UpdateTodoRequest,
  MarkTodoCompleteRequest,
  TodoFormValues,
} from "./todo";
export {
  TODO_CATEGORIES,
  TODO_PRIORITIES,
  TODO_RECURRENCE_TYPES,
  emptyTodoFormValues,
  todoToFormValues,
} from "./todo";
export type { InventoryAlert, InventoryAlertPantryItem } from "./inventoryAlert";
