using FridgeSystem.Data;
using FridgeSystem.Models;
using FridgeSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace FridgeSystem.Controllers;

public class FoodController : Controller
{
    private readonly AppDbContext _context;
    

    public FoodController(AppDbContext context)
    {
        _context = context;
       
    }

    public IActionResult Index(string? keyword, string? storagePlace, string? status)
    {
        var userId = CurrentUserId();

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var allFoods = _context.Foods
            .Where(food => food.UserId == userId.Value)
            .ToList();

        IEnumerable<FoodItem> foods = allFoods;

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            foods = foods.Where(food =>
                food.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
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
        ViewBag.TotalCount = allFoods.Count;
        ViewBag.FridgeCount = allFoods.Count(food => food.StoragePlace == "冷藏");
        ViewBag.FreezerCount = allFoods.Count(food => food.StoragePlace == "冷凍");
        ViewBag.ExpiringCount = allFoods.Count(food =>
            food.Status == "快過期" || food.Status == "今天到期");
        ViewBag.ExpiredCount = allFoods.Count(food => food.Status == "已過期");

        return View(foods
            .OrderBy(food => food.ExpireDate ?? DateTime.MaxValue)
            .ToList());
    }

    public IActionResult Calendar(int? year, int? month)
    {
        var userId = CurrentUserId();

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var allFoods = _context.Foods
            .Where(food => food.UserId == userId.Value)
            .ToList();

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
                    Foods = allFoods
                        .Where(food =>
                            food.ExpireDate.HasValue &&
                            food.ExpireDate.Value.Date == date.Date)
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
        var userId = CurrentUserId();

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var food = _context.Foods
            .FirstOrDefault(food =>
                food.Id == id &&
                food.UserId == userId.Value);

        return food == null ? NotFound() : View(food);
    }

    public IActionResult Create()
    {
        var userId = CurrentUserId();

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        SetFoodOptions();

        return View(new FoodItem
        {
            PutDate = DateTime.Today
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(FoodItem foodItem)
    {
        var userId = CurrentUserId();

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        FoodRule.ApplyDefaults(foodItem);

        if (!ModelState.IsValid)
        {
            SetFoodOptions(foodItem.StoragePlace);
            return View(foodItem);
        }

        foodItem.UserId = userId.Value;
        

        _context.Foods.Add(foodItem);
        _context.SaveChanges();

        UsageLog.Add(
            "新增食材",
            foodItem.Name,
            $"新增 {foodItem.Quantity}，保存位置：{foodItem.StoragePlace}",
            CurrentUserName());

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var userId = CurrentUserId();

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var food = _context.Foods
            .FirstOrDefault(food =>
                food.Id == id &&
                food.UserId == userId.Value);

        if (food == null)
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
        var userId = CurrentUserId();

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

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

        var existingFood = _context.Foods
            .FirstOrDefault(food =>
                food.Id == id &&
                food.UserId == userId.Value);

        if (existingFood == null)
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

        _context.SaveChanges();

        UsageLog.Add(
            "修改食材",
            existingFood.Name,
            $"更新數量為 {existingFood.Quantity}",
            CurrentUserName());

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var userId = CurrentUserId();

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var food = _context.Foods
            .FirstOrDefault(food =>
                food.Id == id &&
                food.UserId == userId.Value);

        return food == null ? NotFound() : View(food);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = CurrentUserId();

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var food = _context.Foods
            .FirstOrDefault(food =>
                food.Id == id &&
                food.UserId == userId.Value);

        if (food != null)
        {
            _context.Foods.Remove(food);
            _context.SaveChanges();

            UsageLog.Add(
                "刪除食材",
                food.Name,
                $"刪除 {food.Quantity}",
                CurrentUserName());
        }

        return RedirectToAction(nameof(Index));
    }

    private void SetFoodOptions(string? selectedStoragePlace = null, string? selectedStatus = null)
    {
        ViewBag.CategoryOptions = new SelectList(FoodRule.Categories);
        ViewBag.StoragePlaceOptions = new SelectList(FoodRule.StoragePlaces, selectedStoragePlace);

        ViewBag.StatusOptions = new SelectList(
            new[]
            {
                "正常",
                "快過期",
                "今天到期",
                "已過期",
                "放太久"
            },
            selectedStatus);
    }

    private int? CurrentUserId()
    {
        return HttpContext.Session.GetInt32("UserId");
    }

    private string CurrentUserName()
    {
        return HttpContext.Session.GetString("UserName") ?? "訪客";
    }
}