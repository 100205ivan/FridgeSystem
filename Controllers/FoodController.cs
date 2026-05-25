using FridgeSystem.Models;
using FridgeSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FridgeSystem.Controllers;

public class FoodController : Controller
{
    // TODO: Mock data for frontend development. Backend should replace this with database access.
    internal static readonly List<FoodItem> Foods = new()
    {
        new() { Id = 1, Name = "牛奶", Category = "乳製品", Quantity = "1 瓶", PutDate = DateTime.Today.AddDays(-4), StoragePlace = "冷藏", ExpireDate = DateTime.Today, Note = "早餐用" },
        new() { Id = 2, Name = "雞蛋", Category = "蛋類", Quantity = "8 顆", PutDate = DateTime.Today.AddDays(-6), StoragePlace = "冷藏", ExpireDate = DateTime.Today.AddDays(2) },
        new() { Id = 3, Name = "雞胸肉", Category = "肉類", Quantity = "2 片", PutDate = DateTime.Today.AddDays(-8), StoragePlace = "冷凍", ExpireDate = DateTime.Today.AddDays(20) },
        new() { Id = 4, Name = "番茄", Category = "蔬菜", Quantity = "3 顆", PutDate = DateTime.Today.AddDays(-3), StoragePlace = "冷藏", ExpireDate = DateTime.Today.AddDays(1) },
        new() { Id = 5, Name = "白飯", Category = "熟食", Quantity = "1 碗", PutDate = DateTime.Today.AddDays(-1), StoragePlace = "冷藏", ExpireDate = DateTime.Today.AddDays(2) },
        new() { Id = 6, Name = "冷凍水餃", Category = "加工食品", Quantity = "1 包", PutDate = DateTime.Today.AddDays(-12), StoragePlace = "冷凍", ExpireDate = DateTime.Today.AddDays(18) },
        new() { Id = 7, Name = "優格", Category = "乳製品", Quantity = "2 盒", PutDate = DateTime.Today.AddDays(-2), StoragePlace = "冷藏", ExpireDate = DateTime.Today.AddDays(5) },
        new() { Id = 8, Name = "高麗菜", Category = "蔬菜", Quantity = "半顆", PutDate = DateTime.Today.AddDays(-7), StoragePlace = "冷藏", ExpireDate = DateTime.Today.AddDays(-1) }
    };

    public IActionResult Index(string? keyword, string? storagePlace, string? status)
    {
        var foods = Foods.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            foods = foods.Where(food => food.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(storagePlace))
        {
            foods = foods.Where(food => food.StoragePlace == storagePlace);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            foods = foods.Where(food => food.Status == status);
        }

        SetFoodOptions(storagePlace, status);
        ViewBag.Keyword = keyword;
        ViewBag.TotalCount = Foods.Count;
        ViewBag.FridgeCount = Foods.Count(food => food.StoragePlace == "冷藏");
        ViewBag.FreezerCount = Foods.Count(food => food.StoragePlace == "冷凍");
        ViewBag.ExpiringCount = Foods.Count(food => food.Status is "快過期" or "今天到期");
        ViewBag.ExpiredCount = Foods.Count(food => food.Status == "已過期");

        return View(foods.OrderBy(food => food.ExpireDate ?? DateTime.MaxValue).ToList());
    }

    public IActionResult Calendar(int? year, int? month)
    {
        var today = DateTime.Today;
        var selectedMonth = new DateTime(year ?? today.Year, month ?? today.Month, 1);
        var start = selectedMonth.AddDays(-(int)selectedMonth.DayOfWeek);
        var days = Enumerable.Range(0, 42)
            .Select(offset =>
            {
                var date = start.AddDays(offset);
                return new FoodCalendarDay
                {
                    Date = date,
                    IsCurrentMonth = date.Month == selectedMonth.Month,
                    IsToday = date.Date == today,
                    Foods = Foods
                        .Where(food => food.ExpireDate.HasValue && food.ExpireDate.Value.Date == date.Date)
                        .OrderBy(food => food.Name)
                        .ToList()
                };
            })
            .ToList();

        return View(new FoodCalendarViewModel
        {
            Month = selectedMonth,
            Days = days
        });
    }

    public IActionResult Details(int id)
    {
        var food = FindFood(id);
        return food is null ? NotFound() : View(food);
    }

    public IActionResult Create()
    {
        SetFoodOptions();
        return View(new FoodItem { PutDate = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(FoodItem foodItem)
    {
        FoodRule.ApplyDefaults(foodItem);

        if (!ModelState.IsValid)
        {
            SetFoodOptions(foodItem.StoragePlace);
            return View(foodItem);
        }

        foodItem.Id = Foods.Any() ? Foods.Max(food => food.Id) + 1 : 1;
        Foods.Add(foodItem);
        UsageLog.Add("新增食材", foodItem.Name, $"新增 {foodItem.Quantity}，保存位置：{foodItem.StoragePlace}", CurrentUserName());

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var food = FindFood(id);
        if (food is null)
        {
            return NotFound();
        }

        SetFoodOptions(food.StoragePlace);
        return View(food);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, FoodItem foodItem)
    {
        if (id != foodItem.Id)
        {
            return BadRequest();
        }

        FoodRule.ApplyDefaults(foodItem);

        if (!ModelState.IsValid)
        {
            SetFoodOptions(foodItem.StoragePlace);
            return View(foodItem);
        }

        var existingFood = FindFood(id);
        if (existingFood is null)
        {
            return NotFound();
        }

        existingFood.Name = foodItem.Name;
        existingFood.Category = foodItem.Category;
        existingFood.Quantity = foodItem.Quantity;
        existingFood.PutDate = foodItem.PutDate;
        existingFood.StoragePlace = foodItem.StoragePlace;
        existingFood.ExpireDate = foodItem.ExpireDate;
        existingFood.Note = foodItem.Note;
        UsageLog.Add("修改食材", existingFood.Name, $"更新數量為 {existingFood.Quantity}，狀態：{existingFood.Status}", CurrentUserName());

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var food = FindFood(id);
        return food is null ? NotFound() : View(food);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var food = FindFood(id);
        if (food is not null)
        {
            Foods.Remove(food);
            UsageLog.Add("刪除食材", food.Name, $"刪除 {food.Quantity}", CurrentUserName());
        }

        return RedirectToAction(nameof(Index));
    }

    private static FoodItem? FindFood(int id)
    {
        return Foods.FirstOrDefault(food => food.Id == id);
    }

    private void SetFoodOptions(string? selectedStoragePlace = null, string? selectedStatus = null)
    {
        ViewBag.CategoryOptions = new SelectList(FoodRule.Categories);
        ViewBag.StoragePlaceOptions = new SelectList(FoodRule.StoragePlaces, selectedStoragePlace);
        ViewBag.StatusOptions = new SelectList(new[] { "正常", "快過期", "今天到期", "已過期", "放太久" }, selectedStatus);
    }

    private string CurrentUserName()
    {
        return HttpContext.Session.GetString("UserName") ?? "訪客";
    }
}
