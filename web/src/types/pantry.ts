export interface PantryIngredientSummary {
  id: string;
  name: string;
  category?: string;
}

export interface PantryItem {
  id: string;
  ingredientId: string;
  ingredient: PantryIngredientSummary;
  quantity: number;
  unit: string;
  expirationDate?: string;
  lowStockThreshold?: number;
  isLowStock: boolean;
  isExpired: boolean;
  isExpiringSoon: boolean;
  householdId: string;
  createdByUserId: string;
  createdAt: string;
  updatedAt: string;
}

export interface PantryInventoryQuery {
  page?: number;
  pageSize?: number;
  search?: string;
  lowStockOnly?: boolean;
  expiringSoonOnly?: boolean;
}

export interface CreatePantryItemRequest {
  ingredientId: string;
  quantity: number;
  unit: string;
  expirationDate?: string;
  lowStockThreshold?: number;
}

export interface UpdatePantryItemRequest {
  quantity: number;
  unit: string;
  expirationDate?: string;
  lowStockThreshold?: number;
}

export interface PantryItemFormValues {
  ingredientId: string;
  quantity: string;
  unit: string;
  expirationDate: string;
  lowStockThreshold: string;
}

export const PANTRY_UNITS = [
  "each",
  "g",
  "kg",
  "ml",
  "l",
  "cup",
  "tbsp",
  "tsp",
  "oz",
  "lb",
] as const;

export function pantryItemToFormValues(item: PantryItem): PantryItemFormValues {
  return {
    ingredientId: item.ingredientId,
    quantity: item.quantity.toString(),
    unit: item.unit,
    expirationDate: item.expirationDate ?? "",
    lowStockThreshold: item.lowStockThreshold?.toString() ?? "",
  };
}

export const emptyPantryFormValues: PantryItemFormValues = {
  ingredientId: "",
  quantity: "1",
  unit: "each",
  expirationDate: "",
  lowStockThreshold: "",
};
