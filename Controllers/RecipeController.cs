using FridgeSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace FridgeSystem.Controllers;

public class RecipeController : Controller
{
    private static readonly List<Recipe> Recipes = new()
    {
        new() { Id = 1, Name = "番茄炒蛋", Ingredients = Ingredients("番茄", "雞蛋") },
        new() { Id = 2, Name = "蛋炒飯", Ingredients = Ingredients("雞蛋", "白飯") },
        new() { Id = 3, Name = "雞肉咖哩", Ingredients = Ingredients("雞胸肉", "馬鈴薯", "紅蘿蔔") },
        new() { Id = 4, Name = "蔬菜湯", Ingredients = Ingredients("高麗菜", "紅蘿蔔", "洋蔥") },
        new() { Id = 5, Name = "水果優格", Ingredients = Ingredients("水果", "優格") }
    };

    public IActionResult Recommend()
    {
        var foods = FoodController.Foods;
        var recommendations = Recipes
            .Select(recipe => BuildRecommendation(recipe, foods))
            .OrderByDescending(recommendation => recommendation.UsesExpiringFood)
            .ThenByDescending(recommendation => recommendation.MatchScore)
            .ThenBy(recommendation => recommendation.MissingIngredients.Count)
            .ToList();

        return View(new RecipeRecommendViewModel
        {
            InventoryFoods = foods,
            Recommendations = recommendations
        });
    }

    private static RecipeRecommendation BuildRecommendation(Recipe recipe, List<FoodItem> foods)
    {
        var foodNames = foods.Select(food => food.Name).ToList();
        var expiringFoodNames = foods
            .Where(food => food.Status is "快過期" or "今天到期" or "已過期")
            .Select(food => food.Name)
            .ToList();

        var requiredIngredients = recipe.Ingredients.Select(ingredient => ingredient.Name).ToList();
        var matchedIngredients = requiredIngredients
            .Where(ingredient => HasIngredient(foodNames, ingredient))
            .ToList();
        var missingIngredients = requiredIngredients
            .Where(ingredient => !HasIngredient(foodNames, ingredient))
            .ToList();
        var expiringIngredients = matchedIngredients
            .Where(ingredient => HasIngredient(expiringFoodNames, ingredient))
            .ToList();

        var reason = expiringIngredients.Any()
            ? $"可優先使用即將到期食材：{string.Join("、", expiringIngredients)}"
            : matchedIngredients.Any()
                ? $"已具備 {matchedIngredients.Count} 項食材，準備成本較低"
                : "目前缺少主要食材，可作為採買參考";

        return new RecipeRecommendation
        {
            Recipe = recipe,
            MatchedIngredients = matchedIngredients,
            MissingIngredients = missingIngredients,
            ExpiringIngredients = expiringIngredients,
            Reason = reason
        };
    }

    private static bool HasIngredient(IEnumerable<string> foodNames, string ingredient)
    {
        if (ingredient == "水果")
        {
            return FoodController.Foods.Any(food => food.Category == "水果");
        }

        return foodNames.Any(foodName =>
            foodName.Contains(ingredient, StringComparison.OrdinalIgnoreCase) ||
            ingredient.Contains(foodName, StringComparison.OrdinalIgnoreCase));
    }

    private static List<RecipeIngredient> Ingredients(params string[] names)
    {
        return names.Select(name => new RecipeIngredient { Name = name }).ToList();
    }
}
