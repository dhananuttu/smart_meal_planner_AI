export interface RecipeCategorySummary {
  id: string;
  name: string;
  color: string;
}

export interface Recipe {
  id: string;
  name: string;
  description?: string;
  instructions: string;
  prepTime?: number;
  cookTime?: number;
  servings: number;
  categoryId?: string;
  category?: RecipeCategorySummary;
  householdId: string;
  createdByUserId: string;
  createdAt: string;
  updatedAt: string;
}

export interface RecipeListQuery {
  page?: number;
  pageSize?: number;
  search?: string;
  categoryId?: string;
}

export interface CreateRecipeRequest {
  name: string;
  description?: string;
  instructions: string;
  prepTime?: number;
  cookTime?: number;
  servings: number;
  categoryId?: string;
}

export interface UpdateRecipeRequest extends CreateRecipeRequest {}

export interface RecipeFormValues {
  name: string;
  description: string;
  instructions: string;
  prepTime: string;
  cookTime: string;
  servings: string;
  categoryId: string;
}

export function recipeToFormValues(recipe: Recipe): RecipeFormValues {
  return {
    name: recipe.name,
    description: recipe.description ?? "",
    instructions: recipe.instructions,
    prepTime: recipe.prepTime?.toString() ?? "",
    cookTime: recipe.cookTime?.toString() ?? "",
    servings: recipe.servings.toString(),
    categoryId: recipe.categoryId ?? "",
  };
}

export function formValuesToRequest(values: RecipeFormValues): CreateRecipeRequest {
  return {
    name: values.name.trim(),
    description: values.description.trim() || undefined,
    instructions: values.instructions.trim(),
    prepTime: values.prepTime ? Number(values.prepTime) : undefined,
    cookTime: values.cookTime ? Number(values.cookTime) : undefined,
    servings: Number(values.servings) || 4,
    categoryId: values.categoryId || undefined,
  };
}
