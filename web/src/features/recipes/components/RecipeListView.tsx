import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { EmptyState, WidgetSkeleton } from "@/components/common";
import { Button, Input, Select } from "@/components/ui";
import { useRecipeCategories, useRecipes } from "@/hooks/useRecipes";
import { RecipeCard } from "./RecipeCard";

export function RecipeListView() {
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [page, setPage] = useState(1);
  const pageSize = 12;

  useEffect(() => {
    const timer = window.setTimeout(() => {
      setSearch(searchInput.trim());
      setPage(1);
    }, 300);
    return () => window.clearTimeout(timer);
  }, [searchInput]);

  const { data, isLoading, isError } = useRecipes({
    page,
    pageSize,
    search: search || undefined,
    categoryId: categoryId || undefined,
  });

  const { data: categoriesData } = useRecipeCategories();
  const categories = categoriesData?.items ?? [];

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <p className="text-sm font-medium text-brand-700">Recipes</p>
          <h1 className="mt-1 text-2xl font-bold text-slate-900 sm:text-3xl">
            Household recipes
          </h1>
          <p className="mt-2 text-sm text-slate-600">
            Browse, create, and manage your family&apos;s recipe collection.
          </p>
        </div>
        <Link to="/recipes/new">
          <Button className="w-full sm:w-auto">New recipe</Button>
        </Link>
      </div>

      <div className="grid gap-4 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm sm:grid-cols-[1fr_220px]">
        <Input
          value={searchInput}
          onChange={(event) => setSearchInput(event.target.value)}
          placeholder="Search recipes…"
          aria-label="Search recipes"
        />
        <Select
          value={categoryId}
          onChange={(event) => {
            setCategoryId(event.target.value);
            setPage(1);
          }}
          aria-label="Filter by cuisine"
        >
          <option value="">All cuisines</option>
          {categories.map((category) => (
            <option key={category.id} value={category.id}>
              {category.name}
            </option>
          ))}
        </Select>
      </div>

      {isLoading ? (
        <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
          {Array.from({ length: 6 }).map((_, index) => (
            <div key={index} className="rounded-2xl border border-slate-200 bg-white p-5">
              <WidgetSkeleton rows={4} />
            </div>
          ))}
        </div>
      ) : isError ? (
        <div className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          Unable to load recipes. Please try again.
        </div>
      ) : !data || data.items.length === 0 ? (
        <div className="rounded-2xl border border-slate-200 bg-white shadow-sm">
          <EmptyState
            message={search || categoryId ? "No recipes match your filters." : "No recipes yet."}
            action={
              <Link to="/recipes/new">
                <Button>Create your first recipe</Button>
              </Link>
            }
          />
        </div>
      ) : (
        <>
          <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
            {data.items.map((recipe) => (
              <RecipeCard key={recipe.id} recipe={recipe} />
            ))}
          </div>

          {data.totalPages > 1 ? (
            <div className="flex items-center justify-between rounded-xl border border-slate-200 bg-white px-4 py-3">
              <p className="text-sm text-slate-600">
                Page {data.page} of {data.totalPages} · {data.totalCount} recipes
              </p>
              <div className="flex gap-2">
                <Button
                  variant="secondary"
                  disabled={!data.hasPreviousPage}
                  onClick={() => setPage((current) => Math.max(current - 1, 1))}
                >
                  Previous
                </Button>
                <Button
                  variant="secondary"
                  disabled={!data.hasNextPage}
                  onClick={() => setPage((current) => current + 1)}
                >
                  Next
                </Button>
              </div>
            </div>
          ) : null}
        </>
      )}
    </div>
  );
}
