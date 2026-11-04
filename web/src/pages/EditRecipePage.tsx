import { useParams } from "react-router-dom";
import { EditRecipeView } from "@/features/recipes";

export function EditRecipePage() {
  const { id } = useParams<{ id: string }>();

  if (!id) {
    return null;
  }

  return <EditRecipeView recipeId={id} />;
}
