export interface InventoryAlertPantryItem {
  id: string;
  ingredientName: string;
  quantity: number;
  unit: string;
  expirationDate?: string;
}

export interface InventoryAlert {
  id: string;
  alertType: string;
  message: string;
  severity: string;
  pantryItem: InventoryAlertPantryItem;
  isRead: boolean;
  isDismissed: boolean;
  createdAt: string;
  updatedAt: string;
}
