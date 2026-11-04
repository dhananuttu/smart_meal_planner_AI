import { useEffect, useMemo, useState } from "react";
import { EmptyState, LoadingSpinner, WidgetSkeleton } from "@/components/common";
import { Button, Label, Select } from "@/components/ui";
import {
  useAddGroceryItem,
  useCreateGroceryList,
  useDeleteGroceryItem,
  useGenerateGroceryList,
  useGroceryList,
  useGroceryLists,
  useMarkGroceryItemPurchased,
} from "@/hooks/useGrocery";
import type { AddGroceryItemFormValues, GroceryItem } from "@/types/grocery";
import { getApiErrorMessage } from "@/utils/apiError";
import { AddGroceryItemModal } from "./AddGroceryItemModal";
import { CreateGroceryListModal } from "./CreateGroceryListModal";
import { GenerateGroceryListModal } from "./GenerateGroceryListModal";
import { GroceryItemRow } from "./GroceryItemRow";
import { formatGroceryListLabel, sortGroceryLists } from "../helpers/groceryListLabel";

type ShowFilter = "all" | "remaining" | "purchased";

function dedupeItemsById(items: GroceryItem[]): GroceryItem[] {
  const seen = new Set<string>();
  return items.filter((item) => {
    if (seen.has(item.id)) {
      return false;
    }
    seen.add(item.id);
    return true;
  });
}

export function GroceryListView() {
  const [selectedListId, setSelectedListId] = useState<string | null>(null);
  const [showFilter, setShowFilter] = useState<ShowFilter>("remaining");
  const [showGenerate, setShowGenerate] = useState(false);
  const [showCreateList, setShowCreateList] = useState(false);
  const [showAddItem, setShowAddItem] = useState(false);

  const { data: lists, isLoading: listsLoading, isError: listsError } = useGroceryLists();
  const {
    data: list,
    isLoading: listLoading,
    isFetching: listFetching,
    isError: listError,
  } = useGroceryList(selectedListId);

  const sortedLists = useMemo(() => (lists ? sortGroceryLists(lists) : []), [lists]);
  const activeList = list && list.id === selectedListId ? list : undefined;

  const createList = useCreateGroceryList();
  const generateList = useGenerateGroceryList();
  const addItem = useAddGroceryItem(selectedListId ?? "");
  const markPurchased = useMarkGroceryItemPurchased(selectedListId ?? "");
  const deleteItem = useDeleteGroceryItem(selectedListId ?? "");

  useEffect(() => {
    if (!lists || lists.length === 0) {
      setSelectedListId(null);
      return;
    }

    if (!selectedListId || !lists.some((entry) => entry.id === selectedListId)) {
      setSelectedListId(sortGroceryLists(lists)[0].id);
    }
  }, [lists, selectedListId]);

  const filteredItems = useMemo(() => {
    const items = dedupeItemsById(activeList?.items ?? []);
    if (showFilter === "remaining") {
      return items.filter((item) => !item.isPurchased);
    }
    if (showFilter === "purchased") {
      return items.filter((item) => item.isPurchased);
    }
    return items;
  }, [activeList?.items, showFilter]);

  const progress = activeList
    ? activeList.items.length > 0
      ? Math.round(
          (activeList.items.filter((item) => item.isPurchased).length / activeList.items.length) *
            100,
        )
      : 0
    : 0;

  async function handleCreateEmptyList(name: string) {
    const created = await createList.mutateAsync({ name });
    setSelectedListId(created.id);
  }

  async function handleGenerate(weekStart: string, name: string) {
    const created = await generateList.mutateAsync({
      weekStart,
      name: name || undefined,
    });
    setSelectedListId(created.id);
  }

  async function handleAddItem(values: AddGroceryItemFormValues) {
    if (!selectedListId) {
      return;
    }

    await addItem.mutateAsync({
      name: values.name.trim(),
      quantity: Number(values.quantity),
      unit: values.unit,
      note: values.note.trim() || undefined,
    });
  }

  async function handleTogglePurchased(item: GroceryItem, isPurchased: boolean) {
    try {
      await markPurchased.mutateAsync({ itemId: item.id, request: { isPurchased } });
    } catch (error) {
      window.alert(getApiErrorMessage(error, "Unable to update item."));
    }
  }

  async function handleDelete(item: GroceryItem) {
    const confirmed = window.confirm(`Remove "${item.name}" from the list?`);
    if (!confirmed) {
      return;
    }

    try {
      await deleteItem.mutateAsync(item.id);
    } catch (error) {
      window.alert(getApiErrorMessage(error, "Unable to delete item."));
    }
  }

  if (listsLoading) {
    return <LoadingSpinner label="Loading grocery lists…" />;
  }

  if (listsError) {
    return (
      <div className="rounded-2xl border border-red-200 bg-red-50 p-8 text-center text-sm text-red-700">
        Unable to load grocery lists.
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div>
          <p className="text-sm font-medium text-brand-700">Grocery</p>
          <h1 className="mt-1 text-2xl font-bold text-slate-900 sm:text-3xl">Shopping lists</h1>
          <p className="mt-2 text-sm text-slate-600">
            Generate from meal plans or add items manually while you shop.
          </p>
        </div>

        <div className="flex flex-col gap-2 sm:flex-row">
          <Button variant="secondary" onClick={() => setShowGenerate(true)}>
            Generate list
          </Button>
          <Button variant="secondary" onClick={() => setShowCreateList(true)}>
            New shopping list
          </Button>
          <Button
            onClick={() => setShowAddItem(true)}
            disabled={!selectedListId}
          >
            Add item
          </Button>
        </div>
      </div>

      {!lists || lists.length === 0 ? (
        <div className="rounded-2xl border border-slate-200 bg-white shadow-sm">
          <EmptyState
            message="No grocery lists yet."
            action={
              <div className="flex flex-col gap-2 sm:flex-row">
                <Button onClick={() => setShowGenerate(true)}>Generate from meal plan</Button>
                <Button variant="secondary" onClick={() => setShowCreateList(true)}>
                  New shopping list
                </Button>
              </div>
            }
          />
        </div>
      ) : (
        <>
          <div className="grid gap-4 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm lg:grid-cols-[1fr_auto] lg:items-center">
            <div>
              <Label htmlFor="listSelect" className="mb-1.5">
                Active list
              </Label>
              <Select
                id="listSelect"
                value={selectedListId ?? ""}
                onChange={(event) => setSelectedListId(event.target.value)}
              >
                {sortedLists.map((entry) => (
                  <option key={entry.id} value={entry.id}>
                    {formatGroceryListLabel(entry)}
                  </option>
                ))}
              </Select>
            </div>

            {activeList ? (
              <div className="min-w-[200px]">
                <p className="text-xs font-medium uppercase tracking-wide text-slate-500">
                  Progress
                </p>
                <p className="mt-1 text-2xl font-bold text-brand-700">{progress}%</p>
                <div className="mt-2 h-2 overflow-hidden rounded-full bg-slate-100">
                  <div
                    className="h-full rounded-full bg-brand-600 transition-all"
                    style={{ width: `${progress}%` }}
                  />
                </div>
              </div>
            ) : null}
          </div>

          <div className="flex flex-wrap gap-2">
            {(["remaining", "all", "purchased"] as ShowFilter[]).map((filter) => (
              <Button
                key={filter}
                variant={showFilter === filter ? "primary" : "secondary"}
                onClick={() => setShowFilter(filter)}
              >
                {filter === "remaining" ? "To buy" : filter === "purchased" ? "Purchased" : "All"}
              </Button>
            ))}
          </div>

          <section className="rounded-2xl border border-slate-200 bg-white shadow-sm">
            {listLoading || (listFetching && !activeList) ? (
              <div className="p-6">
                <WidgetSkeleton rows={5} />
              </div>
            ) : listError || !activeList ? (
              <div className="p-6 text-sm text-red-700">Unable to load list details.</div>
            ) : filteredItems.length === 0 ? (
              <EmptyState
                message={
                  showFilter === "remaining"
                    ? activeList.items.length > 0
                      ? "Everything is purchased!"
                      : "This list is empty."
                    : "No items in this view."
                }
                action={
                  showFilter === "remaining" && activeList.items.length === 0 ? (
                    <Button onClick={() => setShowAddItem(true)}>Add item</Button>
                  ) : undefined
                }
              />
            ) : (
              <ul className="space-y-2 p-4">
                {filteredItems.map((item) => (
                  <GroceryItemRow
                    key={item.id}
                    item={item}
                    isUpdating={markPurchased.isPending}
                    isDeleting={deleteItem.isPending}
                    onTogglePurchased={handleTogglePurchased}
                    onDelete={handleDelete}
                  />
                ))}
              </ul>
            )}
          </section>
        </>
      )}

      <CreateGroceryListModal
        open={showCreateList}
        isSubmitting={createList.isPending}
        onClose={() => setShowCreateList(false)}
        onCreate={handleCreateEmptyList}
      />

      <GenerateGroceryListModal
        open={showGenerate}
        isSubmitting={generateList.isPending}
        onClose={() => setShowGenerate(false)}
        onGenerate={handleGenerate}
      />

      {selectedListId ? (
        <AddGroceryItemModal
          open={showAddItem}
          isSubmitting={addItem.isPending}
          onClose={() => setShowAddItem(false)}
          onSubmit={handleAddItem}
        />
      ) : null}
    </div>
  );
}
