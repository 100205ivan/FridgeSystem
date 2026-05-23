namespace FridgeSystem.Models;

public class DashboardViewModel
{
    public int TotalCount { get; set; }

    public int ExpiringCount { get; set; }

    public int ExpiredCount { get; set; }

    public int NoExpireDateCount { get; set; }

    public List<ChartItem> StorageStats { get; set; } = new();

    public List<ChartItem> CategoryStats { get; set; } = new();

    public List<ChartItem> StatusStats { get; set; } = new();

    public List<ChartItem> ExpireMonthStats { get; set; } = new();

    public List<FoodItem> PriorityFoods { get; set; } = new();

    public List<UsageRecord> RecentRecords { get; set; } = new();
}
