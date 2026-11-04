import { useNavigate } from "react-router-dom";
import { LoadingSpinner } from "@/components/common";
import { RecipeForm } from "@/features/recipes/components/RecipeForm";
import { useRecipe, useUpdateRecipe } from "@/hooks/useRecipes";
import { formValuesToRequest, recipeToFormValues } from "@/types/recipe";
import { getApiErrorMessage } from "@/utils/apiError";

interface EditRecipeViewProps {
  recipeId: string;
}

export function EditRecipeView({ recipeId }: EditRecipeViewProps) {
  const navigate = useNavigate();
  const { data: recipe, isLoading, isError } = useRecipe(recipeId);
  const updateRecipe = useUpdateRecipe(recipeId);

  if (isLoading) {
    return <LoadingSpinner label="Loading recipe…" />;
  }

  if (isError || !recipe) {
    return (
      <div className="rounded-2xl border border-red-200 bg-red-50 p-8 text-center text-sm text-red-700">
        Unable to load recipe for editing.
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <p className="text-sm font-medium text-brand-700">Recipes</p>
        <h1 className="mt-1 text-2xl font-bold text-slate-900 sm:text-3xl">Edit recipe</h1>
        <p className="mt-2 text-sm text-slate-600">Update {recipe.name}.</p>
      </div>

      <section className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
        <RecipeForm
          key={recipe.id}
          initialValues={recipeToFormValues(recipe)}
          submitLabel="Save changes"
          isSubmitting={updateRecipe.isPending}
          onCancel={() => navigate(`/recipes/${recipe.id}`)}
          onSubmit={async (values) => {
            try {
              const updated = await updateRecipe.mutateAsync(formValuesToRequest(values));
              navigate(`/recipes/${updated.id}`, { replace: true });
            } catch (error) {
              throw new Error(getApiErrorMessage(error, "Unable to update recipe."));
            }
          }}
        />
      </section>
    </div>
  );
}
