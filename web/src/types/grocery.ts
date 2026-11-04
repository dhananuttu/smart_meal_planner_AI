export interface GroceryListSummary {
  id: string;
  name: string;
  mealPlanId?: string;
  isArchived: boolean;
  itemCount: number;
  purchasedItemCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface GroceryItem {
  id: string;
  groceryListId: string;
  name: string;
  quantity: number;
  unit: string;
  isPurchased: boolean;
  sourceRecipeId?: string;
  note?: string;
  sortOrder: number;
  createdAt: string;
  updatedAt: string;
}

export interface GroceryListDetail {
  id: string;
  name: string;
  mealPlanId?: string;
  isArchived: boolean;
  items: GroceryItem[];
  createdAt: string;
  updatedAt: string;
}

export interface CreateGroceryListRequest {
  name?: string;
}

export interface GenerateGroceryListFromMealPlanRequest {
  mealPlanId?: string;
  weekStart?: string;
  name?: string;
}

export interface AddGroceryItemRequest {
  name: string;
  quantity: number;
  unit: string;
  note?: string;
  sourceRecipeId?: string;
}

export interface MarkGroceryItemPurchasedRequest {
  isPurchased: boolean;
}

export interface AddGroceryItemFormValues {
  name: string;
  quantity: string;
  unit: string;
  note: string;
}

export const emptyGroceryItemFormValues: AddGroceryItemFormValues = {
  name: "",
  quantity: "1",
  unit: "each",
  note: "",
};
