using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FridgeSystem.Data;
using FridgeSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace FridgeSystem.Controllers;

public class RecipeController : Controller
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public RecipeController(
        AppDbContext context,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<IActionResult> Recommend()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var foods = _context.Foods
            .Where(food => food.UserId == userId.Value)
            .ToList();

        if (!foods.Any())
        {
            return View(new RecipeRecommendViewModel
            {
                InventoryFoods = foods,
                Recommendations = new List<RecipeRecommendation>()
            });
        }

        var apiKey = _configuration["Spoonacular:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            ViewBag.ErrorMessage = "Spoonacular API Key is not configured.";

            return View(new RecipeRecommendViewModel
            {
                InventoryFoods = foods,
                Recommendations = new List<RecipeRecommendation>()
            });
        }

        var translatedIngredients = new List<string>();

        foreach (var food in foods)
        {
            var englishName = await TranslateToEnglish(food.Name);
            translatedIngredients.Add(englishName);
        }

        var ingredientText = string.Join(",", translatedIngredients.Distinct());

        var url =
            "https://api.spoonacular.com/recipes/findByIngredients" +
            $"?ingredients={Uri.EscapeDataString(ingredientText)}" +
            "&number=10" +
            "&ranking=1" +
            "&ignorePantry=true" +
            $"&apiKey={apiKey}";

        var client = _httpClientFactory.CreateClient();

        List<SpoonacularRecipe> apiRecipes;

        try
        {
            apiRecipes =
                await client.GetFromJsonAsync<List<SpoonacularRecipe>>(url)
                ?? new List<SpoonacularRecipe>();
        }
        catch
        {
            ViewBag.ErrorMessage = "Unable to fetch recipes from Spoonacular.";

            return View(new RecipeRecommendViewModel
            {
                InventoryFoods = foods,
                Recommendations = new List<RecipeRecommendation>()
            });
        }

        var recommendations = apiRecipes
            .Select(ConvertToRecommendation)
            .OrderByDescending(recommendation => recommendation.MatchScore)
            .ThenBy(recommendation => recommendation.MissingIngredients.Count)
            .ToList();

        return View(new RecipeRecommendViewModel
        {
            InventoryFoods = foods,
            Recommendations = recommendations
        });
    }

    private static RecipeRecommendation ConvertToRecommendation(SpoonacularRecipe apiRecipe)
    {
        var matched = apiRecipe.UsedIngredients
            .Select(ingredient => ingredient.Name)
            .ToList();

        var missing = apiRecipe.MissedIngredients
            .Select(ingredient => ingredient.Name)
            .ToList();

        var allIngredients = matched
            .Concat(missing)
            .Select(name => new RecipeIngredient
            {
                Name = name
            })
            .ToList();

        return new RecipeRecommendation
        {
            Recipe = new Recipe
            {
                Id = apiRecipe.Id,
                Name = apiRecipe.Title,
                Ingredients = allIngredients
            },
            MatchedIngredients = matched,
            MissingIngredients = missing,
            ExpiringIngredients = new List<string>(),
            Reason = matched.Any()
                ? $"Matched {matched.Count} ingredient(s), missing {missing.Count} ingredient(s)."
                : "No matching ingredient found. You can use this as a shopping reference."
        };
    }

    private async Task<string> TranslateToEnglish(string text)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();

            var url =
                "https://translate.googleapis.com/translate_a/single" +
                "?client=gtx" +
                "&sl=zh-TW" +
                "&tl=en" +
                "&dt=t" +
                $"&q={Uri.EscapeDataString(text)}";

            var response = await client.GetStringAsync(url);

            using var json = JsonDocument.Parse(response);

            return json.RootElement[0][0][0].GetString() ?? text;
        }
        catch
        {
            return text;
        }
    }

    private class SpoonacularRecipe
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = "";

        [JsonPropertyName("usedIngredients")]
        public List<SpoonacularIngredient> UsedIngredients { get; set; } = new();

        [JsonPropertyName("missedIngredients")]
        public List<SpoonacularIngredient> MissedIngredients { get; set; } = new();
    }

    private class SpoonacularIngredient
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    }
}