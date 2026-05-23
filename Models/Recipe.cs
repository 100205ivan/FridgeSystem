namespace FridgeSystem.Models;

public class Recipe
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<RecipeIngredient> Ingredients { get; set; } = new();
}
