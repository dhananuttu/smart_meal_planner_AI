export interface Ingredient {
  id: string;
  name: string;
  defaultUnit?: string;
  category?: string;
  notes?: string;
  householdId: string;
  createdByUserId: string;
  createdAt: string;
  updatedAt: string;
}

export interface IngredientListQuery {
  page?: number;
  pageSize?: number;
  search?: string;
  category?: string;
}

export interface CreateIngredientRequest {
  name: string;
  defaultUnit?: string;
  category?: string;
  notes?: string;
}

export interface IngredientFormValues {
  name: string;
  defaultUnit: string;
  category: string;
  notes: string;
}

export const INGREDIENT_CATEGORIES = [
  "Produce",
  "Protein",
  "Dairy",
  "Grain",
  "Spice",
  "Beverage",
  "Other",
] as const;

export const emptyIngredientFormValues: IngredientFormValues = {
  name: "",
  defaultUnit: "each",
  category: "",
  notes: "",
};
