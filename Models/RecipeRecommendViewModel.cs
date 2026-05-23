namespace FridgeSystem.Models;

public class RecipeRecommendViewModel
{
    public List<FoodItem> InventoryFoods { get; set; } = new();
    public List<RecipeRecommendation> Recommendations { get; set; } = new();
}

public class RecipeRecommendation
{
    public Recipe Recipe { get; set; } = new();
    public List<string> MatchedIngredients { get; set; } = new();
    public List<string> MissingIngredients { get; set; } = new();
    public List<string> ExpiringIngredients { get; set; } = new();
    public string Reason { get; set; } = string.Empty;
    public int MatchScore => MatchedIngredients.Count;
    public bool UsesExpiringFood => ExpiringIngredients.Any();
}
