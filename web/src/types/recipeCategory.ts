export interface RecipeCategory {
  id: string;
  name: string;
  color: string;
  householdId: string;
  createdByUserId: string;
  createdAt: string;
  updatedAt: string;
}

export interface RecipeCategoryListQuery {
  page?: number;
  pageSize?: number;
  search?: string;
}

export interface CreateRecipeCategoryRequest {
  name: string;
  color: string;
}

export interface RecipeCategoryFormValues {
  name: string;
  color: string;
}

/** Cuisine / regional labels for recipe organization (not pantry ingredient types). */
export const CUISINE_SUGGESTIONS = [
  "Italian",
  "Mediterranean",
  "North Indian",
  "South Indian",
  "Korean",
  "Chinese",
  "Japanese",
  "Thai",
  "Swedish",
  "German",
  "French",
  "Mexican",
  "Spanish",
  "Greek",
  "Middle Eastern",
  "American",
  "British",
  "Vietnamese",
  "Turkish",
  "Caribbean",
] as const;

export const RECIPE_CATEGORY_COLOR_PRESETS = [
  { name: "Tomato", hex: "#E53935" },
  { name: "Saffron", hex: "#F9A825" },
  { name: "Olive", hex: "#558B2F" },
  { name: "Basil", hex: "#43A047" },
  { name: "Ocean", hex: "#1E88E5" },
  { name: "Plum", hex: "#8E24AA" },
  { name: "Charcoal", hex: "#546E7A" },
  { name: "Coral", hex: "#FF7043" },
] as const;

export const emptyRecipeCategoryFormValues: RecipeCategoryFormValues = {
  name: "",
  color: RECIPE_CATEGORY_COLOR_PRESETS[0].hex,
};

export function isValidRecipeCategoryColor(color: string): boolean {
  return /^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$/.test(color.trim());
}
