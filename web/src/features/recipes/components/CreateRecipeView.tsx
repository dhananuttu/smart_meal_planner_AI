import { useNavigate } from "react-router-dom";
import {
  RecipeForm,
  emptyRecipeFormValues,
} from "@/features/recipes/components/RecipeForm";
import { useCreateRecipe } from "@/hooks/useRecipes";
import { formValuesToRequest } from "@/types/recipe";
import { getApiErrorMessage } from "@/utils/apiError";

export function CreateRecipeView() {
  const navigate = useNavigate();
  const createRecipe = useCreateRecipe();

  return (
    <div className="space-y-6">
      <div>
        <p className="text-sm font-medium text-brand-700">Recipes</p>
        <h1 className="mt-1 text-2xl font-bold text-slate-900 sm:text-3xl">Create recipe</h1>
        <p className="mt-2 text-sm text-slate-600">Add a new recipe to your household collection.</p>
      </div>

      <section className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
        <RecipeForm
          initialValues={emptyRecipeFormValues}
          submitLabel="Create recipe"
          isSubmitting={createRecipe.isPending}
          onCancel={() => navigate("/recipes")}
          onSubmit={async (values) => {
            try {
              const recipe = await createRecipe.mutateAsync(formValuesToRequest(values));
              navigate(`/recipes/${recipe.id}`, { replace: true });
            } catch (error) {
              throw new Error(getApiErrorMessage(error, "Unable to create recipe."));
            }
          }}
        />
      </section>
    </div>
  );
}
