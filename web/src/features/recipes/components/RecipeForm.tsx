import { FormEvent, useMemo, useState } from "react";
import { Button, Input, Label, Select, Textarea } from "@/components/ui";
import { useRecipeCategories } from "@/hooks/useRecipes";
import type { RecipeCategory } from "@/types/recipeCategory";
import type { RecipeFormValues } from "@/types/recipe";
import { AddRecipeCategoryModal } from "./AddRecipeCategoryModal";

interface RecipeFormProps {
  initialValues: RecipeFormValues;
  submitLabel: string;
  isSubmitting: boolean;
  onSubmit: (values: RecipeFormValues) => Promise<void>;
  onCancel: () => void;
}

export function RecipeForm({
  initialValues,
  submitLabel,
  isSubmitting,
  onSubmit,
  onCancel,
}: RecipeFormProps) {
  const [values, setValues] = useState<RecipeFormValues>(initialValues);
  const [error, setError] = useState<string | null>(null);
  const [showAddCategory, setShowAddCategory] = useState(false);
  const [createdCategory, setCreatedCategory] = useState<RecipeCategory | null>(null);
  const {
    data: categoriesData,
    isLoading: categoriesLoading,
    isError: categoriesError,
    refetch: refetchCategories,
  } = useRecipeCategories();

  const categories = useMemo(() => {
    const items = categoriesData?.items ?? [];
    if (createdCategory && !items.some((category) => category.id === createdCategory.id)) {
      return [...items, createdCategory].sort((a, b) => a.name.localeCompare(b.name));
    }
    return items;
  }, [categoriesData?.items, createdCategory]);

  function updateField<K extends keyof RecipeFormValues>(field: K, value: RecipeFormValues[K]) {
    setValues((current) => ({ ...current, [field]: value }));
  }

  function handleCategoryCreated(category: RecipeCategory) {
    setCreatedCategory(category);
    updateField("categoryId", category.id);
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    if (!values.name.trim()) {
      setError("Recipe name is required.");
      return;
    }

    if (!values.instructions.trim()) {
      setError("Instructions are required.");
      return;
    }

    try {
      await onSubmit(values);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to save recipe.");
    }
  }

  return (
    <>
    <form className="space-y-6" onSubmit={handleSubmit}>
      {error ? (
        <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
          {error}
        </div>
      ) : null}

      <div className="grid gap-6 lg:grid-cols-2">
        <div className="space-y-5">
          <div>
            <Label htmlFor="name">Name</Label>
            <Input
              id="name"
              value={values.name}
              onChange={(event) => updateField("name", event.target.value)}
              placeholder="Chicken tikka masala"
              required
              maxLength={200}
            />
          </div>

          <div>
            <Label htmlFor="description">Description</Label>
            <Textarea
              id="description"
              value={values.description}
              onChange={(event) => updateField("description", event.target.value)}
              placeholder="A short summary of the dish"
              rows={3}
              maxLength={5000}
            />
          </div>

          <div>
            <div className="mb-1.5 flex items-center justify-between gap-2">
              <Label htmlFor="categoryId" className="mb-0">
                Cuisine
              </Label>
              <Button
                type="button"
                variant="ghost"
                className="h-auto px-2 py-1 text-sm text-brand-700"
                onClick={() => setShowAddCategory(true)}
              >
                + New cuisine
              </Button>
            </div>
            <Select
              id="categoryId"
              value={values.categoryId}
              onChange={(event) => updateField("categoryId", event.target.value)}
              disabled={categoriesLoading}
            >
              <option value="">
                {categoriesLoading ? "Loading cuisines…" : "No cuisine"}
              </option>
              {categories.map((category) => (
                <option key={category.id} value={category.id}>
                  {category.name}
                </option>
              ))}
            </Select>
            {categoriesError ? (
              <p className="mt-1.5 text-xs text-red-600">
                Unable to load cuisines. Sign out and sign back in, or{" "}
                <button
                  type="button"
                  className="font-medium underline"
                  onClick={() => refetchCategories()}
                >
                  retry
                </button>
                .
              </p>
            ) : !categoriesLoading && categories.length === 0 ? (
              <p className="mt-1.5 text-xs text-amber-700">
                No cuisines yet — click <strong>+ New cuisine</strong> to add North Indian, Italian,
                etc.
              </p>
            ) : (
              <p className="mt-1.5 text-xs text-slate-500">
                e.g. Italian, Mediterranean, North Indian — not the same as pantry ingredient types.
              </p>
            )}
          </div>
        </div>

        <div className="space-y-5">
          <div className="grid gap-4 sm:grid-cols-3">
            <div>
              <Label htmlFor="prepTime">Prep (min)</Label>
              <Input
                id="prepTime"
                type="number"
                min={0}
                max={1440}
                value={values.prepTime}
                onChange={(event) => updateField("prepTime", event.target.value)}
                placeholder="15"
              />
            </div>
            <div>
              <Label htmlFor="cookTime">Cook (min)</Label>
              <Input
                id="cookTime"
                type="number"
                min={0}
                max={1440}
                value={values.cookTime}
                onChange={(event) => updateField("cookTime", event.target.value)}
                placeholder="30"
              />
            </div>
            <div>
              <Label htmlFor="servings">Servings</Label>
              <Input
                id="servings"
                type="number"
                min={1}
                max={50}
                value={values.servings}
                onChange={(event) => updateField("servings", event.target.value)}
                required
              />
            </div>
          </div>

          <div>
            <Label htmlFor="instructions">Instructions</Label>
            <Textarea
              id="instructions"
              value={values.instructions}
              onChange={(event) => updateField("instructions", event.target.value)}
              placeholder="Step-by-step cooking instructions"
              rows={12}
              required
              maxLength={10000}
            />
          </div>
        </div>
      </div>

      <div className="flex flex-col-reverse gap-3 border-t border-slate-100 pt-6 sm:flex-row sm:justify-end">
        <Button type="button" variant="secondary" onClick={onCancel}>
          Cancel
        </Button>
        <Button type="submit" isLoading={isSubmitting}>
          {submitLabel}
        </Button>
      </div>
    </form>

    <AddRecipeCategoryModal
      open={showAddCategory}
      onClose={() => setShowAddCategory(false)}
      onCreated={handleCategoryCreated}
    />
  </>
  );
}

export const emptyRecipeFormValues: RecipeFormValues = {
  name: "",
  description: "",
  instructions: "",
  prepTime: "",
  cookTime: "",
  servings: "4",
  categoryId: "",
};
