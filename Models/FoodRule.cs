namespace FridgeSystem.Models;

public class FoodRule
{
    public string Category { get; set; } = string.Empty;
    public string DefaultStoragePlace { get; set; } = string.Empty;
    public int DefaultExpireDays { get; set; }

    public static readonly IReadOnlyList<FoodRule> Rules = new List<FoodRule>
    {
        new() { Category = "肉類", DefaultStoragePlace = "冷凍", DefaultExpireDays = 30 },
        new() { Category = "海鮮", DefaultStoragePlace = "冷凍", DefaultExpireDays = 14 },
        new() { Category = "蔬菜", DefaultStoragePlace = "冷藏", DefaultExpireDays = 5 },
        new() { Category = "水果", DefaultStoragePlace = "冷藏", DefaultExpireDays = 7 },
        new() { Category = "蛋類", DefaultStoragePlace = "冷藏", DefaultExpireDays = 14 },
        new() { Category = "乳製品", DefaultStoragePlace = "冷藏", DefaultExpireDays = 7 },
        new() { Category = "熟食", DefaultStoragePlace = "冷藏", DefaultExpireDays = 3 },
        new() { Category = "乾貨", DefaultStoragePlace = "常溫", DefaultExpireDays = 180 },
        new() { Category = "加工食品", DefaultStoragePlace = "冷藏", DefaultExpireDays = 30 }
    };

    public static readonly IReadOnlyList<string> Categories = Rules.Select(rule => rule.Category).ToList();

    public static readonly IReadOnlyList<string> StoragePlaces = new List<string> { "冷藏", "冷凍", "常溫" };

    public static FoodRule? FindByCategory(string? category)
    {
        return Rules.FirstOrDefault(rule => rule.Category == category);
    }

    public static void ApplyDefaults(FoodItem item)
    {
        var rule = FindByCategory(item.Category);
        if (rule is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(item.StoragePlace))
        {
            item.StoragePlace = rule.DefaultStoragePlace;
        }

        if (!item.ExpireDate.HasValue)
        {
            item.ExpireDate = item.PutDate.Date.AddDays(rule.DefaultExpireDays);
        }
    }
}
