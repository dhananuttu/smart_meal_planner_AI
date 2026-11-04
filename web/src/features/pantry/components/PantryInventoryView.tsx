import { useEffect, useState } from "react";
import { useQueries } from "@tanstack/react-query";
import { EmptyState, WidgetSkeleton } from "@/components/common";
import { Button, Input } from "@/components/ui";
import * as pantryApi from "@/api/pantryApi";
import {
  useCreatePantryItem,
  useDeletePantryItem,
  usePantryInventory,
  useUpdatePantryItem,
} from "@/hooks/usePantry";
import type { PantryItem, PantryItemFormValues } from "@/types/pantry";
import {
  emptyPantryFormValues,
  pantryItemToFormValues,
} from "@/types/pantry";
import { getApiErrorMessage } from "@/utils/apiError";
import { PantryItemFormModal } from "./PantryItemFormModal";
import { PantryTable } from "./PantryTable";

type FilterMode = "all" | "low-stock" | "expiring";

interface FormState {
  mode: "create" | "edit";
  item?: PantryItem;
  values: PantryItemFormValues;
}

function toRequestValues(values: PantryItemFormValues) {
  return {
    quantity: Number(values.quantity),
    unit: values.unit,
    expirationDate: values.expirationDate || undefined,
    lowStockThreshold: values.lowStockThreshold
      ? Number(values.lowStockThreshold)
      : undefined,
  };
}

export function PantryInventoryView() {
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [filter, setFilter] = useState<FilterMode>("all");
  const [page, setPage] = useState(1);
  const [form, setForm] = useState<FormState | null>(null);
  const pageSize = 20;

  useEffect(() => {
    const timer = window.setTimeout(() => {
      setSearch(searchInput.trim());
      setPage(1);
    }, 300);
    return () => window.clearTimeout(timer);
  }, [searchInput]);

  const query = {
    page,
    pageSize,
    search: search || undefined,
    lowStockOnly: filter === "low-stock" ? true : undefined,
    expiringSoonOnly: filter === "expiring" ? true : undefined,
  };

  const { data, isLoading, isError } = usePantryInventory(query);
  const [lowStockStats, expiringStats, allStats] = useQueries({
    queries: [
      {
        queryKey: ["pantry", "stats", "low-stock"],
        queryFn: () => pantryApi.getInventory({ lowStockOnly: true, pageSize: 1, page: 1 }),
      },
      {
        queryKey: ["pantry", "stats", "expiring"],
        queryFn: () => pantryApi.getInventory({ expiringSoonOnly: true, pageSize: 1, page: 1 }),
      },
      {
        queryKey: ["pantry", "stats", "all"],
        queryFn: () => pantryApi.getInventory({ pageSize: 100, page: 1 }),
      },
    ],
  });

  const summary = {
    lowStock: lowStockStats.data?.totalCount ?? 0,
    expiring: expiringStats.data?.totalCount ?? 0,
    expired: (allStats.data?.items ?? []).filter((item) => item.isExpired).length,
  };

  const createItem = useCreatePantryItem();
  const updateItem = useUpdatePantryItem();
  const deleteItem = useDeletePantryItem();

  const isSubmitting = createItem.isPending || updateItem.isPending;

  async function handleFormSubmit(values: PantryItemFormValues) {
    const payload = toRequestValues(values);

    if (form?.mode === "create") {
      await createItem.mutateAsync({
        ingredientId: values.ingredientId,
        ...payload,
      });
      return;
    }

    if (form?.item) {
      await updateItem.mutateAsync({ id: form.item.id, request: payload });
    }
  }

  async function handleDelete(item: PantryItem) {
    const confirmed = window.confirm(
      `Remove ${item.ingredient.name} from the pantry?`,
    );
    if (!confirmed) {
      return;
    }

    try {
      await deleteItem.mutateAsync(item.id);
    } catch (error) {
      window.alert(getApiErrorMessage(error, "Unable to delete pantry item."));
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <p className="text-sm font-medium text-brand-700">Pantry</p>
          <h1 className="mt-1 text-2xl font-bold text-slate-900 sm:text-3xl">
            Inventory
          </h1>
          <p className="mt-2 text-sm text-slate-600">
            Track quantities, expiration dates, and low-stock levels.
          </p>
        </div>
        <Button
          className="w-full sm:w-auto"
          onClick={() =>
            setForm({ mode: "create", values: emptyPantryFormValues })
          }
        >
          Add item
        </Button>
      </div>

      {/* Alert-style summary cards (dashboard-style) */}
      <div className="grid gap-4 sm:grid-cols-3">
        <button
          type="button"
          onClick={() => setFilter(filter === "low-stock" ? "all" : "low-stock")}
          className={`rounded-2xl border p-4 text-left transition ${
            filter === "low-stock"
              ? "border-orange-300 bg-orange-50"
              : "border-slate-200 bg-white hover:border-orange-200"
          }`}
        >
          <p className="text-sm font-medium text-orange-800">Low stock</p>
          <p className="mt-2 text-3xl font-bold text-orange-900">{summary.lowStock}</p>
          <p className="mt-1 text-xs text-orange-700">Items at or below threshold</p>
        </button>

        <button
          type="button"
          onClick={() => setFilter(filter === "expiring" ? "all" : "expiring")}
          className={`rounded-2xl border p-4 text-left transition ${
            filter === "expiring"
              ? "border-amber-300 bg-amber-50"
              : "border-slate-200 bg-white hover:border-amber-200"
          }`}
        >
          <p className="text-sm font-medium text-amber-800">Expiring soon</p>
          <p className="mt-2 text-3xl font-bold text-amber-900">{summary.expiring}</p>
          <p className="mt-1 text-xs text-amber-700">Within the next 7 days</p>
        </button>

        <div className="rounded-2xl border border-red-200 bg-red-50 p-4">
          <p className="text-sm font-medium text-red-800">Expired</p>
          <p className="mt-2 text-3xl font-bold text-red-900">{summary.expired}</p>
          <p className="mt-1 text-xs text-red-700">Past expiration date</p>
        </div>
      </div>

      <div className="flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm sm:flex-row sm:items-center">
        <Input
          value={searchInput}
          onChange={(event) => setSearchInput(event.target.value)}
          placeholder="Search ingredients…"
          aria-label="Search pantry"
          className="sm:flex-1"
        />
        {filter !== "all" ? (
          <Button variant="secondary" onClick={() => setFilter("all")}>
            Clear filter
          </Button>
        ) : null}
      </div>

      <section className="rounded-2xl border border-slate-200 bg-white shadow-sm">
        {isLoading ? (
          <div className="p-6">
            <WidgetSkeleton rows={6} />
          </div>
        ) : isError ? (
          <div className="p-6 text-sm text-red-700">Unable to load pantry inventory.</div>
        ) : !data || data.items.length === 0 ? (
          <EmptyState
            message={
              search || filter !== "all"
                ? "No items match your filters."
                : "Your pantry is empty."
            }
            action={
              <Button onClick={() => setForm({ mode: "create", values: emptyPantryFormValues })}>
                Add your first item
              </Button>
            }
          />
        ) : (
          <>
            <PantryTable
              items={data.items}
              onEdit={(item) =>
                setForm({
                  mode: "edit",
                  item,
                  values: pantryItemToFormValues(item),
                })
              }
              onDelete={handleDelete}
              isDeleting={deleteItem.isPending}
            />

            {data.totalPages > 1 ? (
              <div className="flex items-center justify-between border-t border-slate-100 px-4 py-3">
                <p className="text-sm text-slate-600">
                  Page {data.page} of {data.totalPages} · {data.totalCount} items
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
      </section>

      {form ? (
        <PantryItemFormModal
          open
          mode={form.mode}
          initialValues={form.values}
          item={form.item}
          isSubmitting={isSubmitting}
          onClose={() => setForm(null)}
          onSubmit={handleFormSubmit}
        />
      ) : null}
    </div>
  );
}
