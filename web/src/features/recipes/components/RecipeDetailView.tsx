import { Link, useNavigate } from "react-router-dom";
import { LoadingSpinner } from "@/components/common";
import { Button } from "@/components/ui";
import { useDeleteRecipe, useRecipe } from "@/hooks/useRecipes";
import { getApiErrorMessage } from "@/utils/apiError";

interface RecipeDetailViewProps {
  recipeId: string;
}

function formatMinutes(label: string, minutes?: number) {
  if (minutes === undefined || minutes === null) {
    return null;
  }

  return (
    <div className="rounded-xl bg-slate-50 px-4 py-3 text-center">
      <p className="text-2xl font-bold text-slate-900">{minutes}</p>
      <p className="text-xs text-slate-500">{label}</p>
    </div>
  );
}

export function RecipeDetailView({ recipeId }: RecipeDetailViewProps) {
  const navigate = useNavigate();
  const { data: recipe, isLoading, isError } = useRecipe(recipeId);
  const deleteRecipe = useDeleteRecipe();

  async function handleDelete() {
    if (!recipe) {
      return;
    }

    const confirmed = window.confirm(`Delete "${recipe.name}"? This cannot be undone.`);
    if (!confirmed) {
      return;
    }

    try {
      await deleteRecipe.mutateAsync(recipe.id);
      navigate("/recipes", { replace: true });
    } catch (error) {
      window.alert(getApiErrorMessage(error, "Unable to delete recipe."));
    }
  }

  if (isLoading) {
    return <LoadingSpinner label="Loading recipe…" />;
  }

  if (isError || !recipe) {
    return (
      <div className="rounded-2xl border border-red-200 bg-red-50 p-8 text-center">
        <h1 className="text-lg font-semibold text-red-800">Recipe not found</h1>
        <p className="mt-2 text-sm text-red-700">This recipe may have been removed.</p>
        <Link to="/recipes" className="mt-4 inline-block text-sm font-medium text-brand-700">
          Back to recipes
        </Link>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
        <div>
          <Link to="/recipes" className="text-sm font-medium text-brand-700 hover:text-brand-600">
            ← Back to recipes
          </Link>
          <div className="mt-3 flex flex-wrap items-center gap-3">
            <h1 className="text-3xl font-bold text-slate-900">{recipe.name}</h1>
            {recipe.category ? (
              <span
                className="rounded-full px-3 py-1 text-sm font-medium text-white"
                style={{ backgroundColor: recipe.category.color }}
              >
                {recipe.category.name}
              </span>
            ) : null}
          </div>
          {recipe.description ? (
            <p className="mt-3 max-w-3xl text-base text-slate-600">{recipe.description}</p>
          ) : null}
        </div>

        <div className="flex gap-3">
          <Link to={`/recipes/${recipe.id}/edit`}>
            <Button variant="secondary">Edit</Button>
          </Link>
          <Button
            variant="ghost"
            className="text-red-600 hover:bg-red-50 hover:text-red-700"
            isLoading={deleteRecipe.isPending}
            onClick={handleDelete}
          >
            Delete
          </Button>
        </div>
      </div>

      <div className="grid gap-4 sm:grid-cols-3">
        {formatMinutes("Prep min", recipe.prepTime)}
        {formatMinutes("Cook min", recipe.cookTime)}
        <div className="rounded-xl bg-brand-50 px-4 py-3 text-center">
          <p className="text-2xl font-bold text-brand-700">{recipe.servings}</p>
          <p className="text-xs text-brand-700/80">Servings</p>
        </div>
      </div>

      <section className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-slate-900">Instructions</h2>
        <div className="mt-4 whitespace-pre-wrap text-sm leading-7 text-slate-700">
          {recipe.instructions}
        </div>
      </section>
    </div>
  );
}
