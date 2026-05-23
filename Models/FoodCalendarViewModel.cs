namespace FridgeSystem.Models;

public class FoodCalendarViewModel
{
    public DateTime Month { get; set; }

    public List<FoodCalendarDay> Days { get; set; } = new();
}

public class FoodCalendarDay
{
    public DateTime Date { get; set; }

    public bool IsCurrentMonth { get; set; }

    public bool IsToday { get; set; }

    public List<FoodItem> Foods { get; set; } = new();
}
