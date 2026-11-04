import { useParams } from "react-router-dom";
import { RecipeDetailView } from "@/features/recipes";

export function RecipeDetailPage() {
  const { id } = useParams<{ id: string }>();

  if (!id) {
    return null;
  }

  return <RecipeDetailView recipeId={id} />;
}
