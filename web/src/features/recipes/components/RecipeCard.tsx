import { Link } from "react-router-dom";
import type { Recipe } from "@/types/recipe";

interface RecipeCardProps {
  recipe: Recipe;
}

function formatMinutes(minutes?: number): string | null {
  if (minutes === undefined || minutes === null) {
    return null;
  }
  return `${minutes} min`;
}

export function RecipeCard({ recipe }: RecipeCardProps) {
  const totalTime =
    recipe.prepTime || recipe.cookTime
      ? [formatMinutes(recipe.prepTime), formatMinutes(recipe.cookTime)]
          .filter(Boolean)
          .join(" + ")
      : null;

  return (
    <Link
      to={`/recipes/${recipe.id}`}
      className="group flex h-full flex-col rounded-2xl border border-slate-200 bg-white p-5 shadow-sm transition hover:border-brand-200 hover:shadow-md"
    >
      <div className="flex items-start justify-between gap-3">
        <h3 className="text-lg font-semibold text-slate-900 group-hover:text-brand-700">
          {recipe.name}
        </h3>
        {recipe.category ? (
          <span
            className="shrink-0 rounded-full px-2.5 py-1 text-xs font-medium text-white"
            style={{ backgroundColor: recipe.category.color }}
          >
            {recipe.category.name}
          </span>
        ) : null}
      </div>

      {recipe.description ? (
        <p className="mt-3 line-clamp-2 text-sm text-slate-600">{recipe.description}</p>
      ) : (
        <p className="mt-3 line-clamp-2 text-sm text-slate-400">No description</p>
      )}

      <div className="mt-auto flex flex-wrap gap-3 pt-4 text-xs text-slate-500">
        <span>{recipe.servings} servings</span>
        {totalTime ? <span>{totalTime}</span> : null}
      </div>
    </Link>
  );
}
