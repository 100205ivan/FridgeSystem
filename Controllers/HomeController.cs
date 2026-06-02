using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FridgeSystem.Models;
using FridgeSystem.Services;
using FridgeSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace FridgeSystem.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var foods = _context.Foods.ToList();

        var model = new DashboardViewModel
        {
            TotalCount = foods.Count,
            ExpiringCount = foods.Count(food => food.Status is "快過期" or "今天到期"),
            ExpiredCount = foods.Count(food => food.Status == "已過期"),
            NoExpireDateCount = foods.Count(food => !food.ExpireDate.HasValue),
            StorageStats = BuildStats(foods, food => food.StoragePlace ?? "未分類"),
            CategoryStats = BuildStats(foods, food => food.Category),
            StatusStats = BuildStats(foods, food => food.Status),
            ExpireMonthStats = BuildExpireMonthStats(foods),
            PriorityFoods = foods
                .Where(food => food.Status is "已過期" or "今天到期" or "快過期")
                .OrderBy(food => food.ExpireDate ?? DateTime.MaxValue)
                .Take(6)
                .ToList(),
            RecentRecords = UsageLog.GetRecent(5).ToList()
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }

    private static List<ChartItem> BuildStats(IEnumerable<FoodItem> foods, Func<FoodItem, string> selector)
    {
        var foodList = foods.ToList();
        var total = foodList.Count;

        return foodList
            .GroupBy(selector)
            .Select(group => new ChartItem
            {
                Label = group.Key,
                Count = group.Count(),
                Percentage = total == 0
                    ? 0
                    : Math.Round(group.Count() * 100m / total, 1)
            })
            .OrderByDescending(item => item.Count)
            .ThenBy(item => item.Label)
            .ToList();
    }

    private static List<ChartItem> BuildExpireMonthStats(IEnumerable<FoodItem> foods)
    {
        var foodList = foods.Where(food => food.ExpireDate.HasValue).ToList();
        var total = foodList.Count;

        return foodList
            .GroupBy(food => food.ExpireDate!.Value.ToString("yyyy/MM"))
            .Select(group => new ChartItem
            {
                Label = group.Key,
                Count = group.Count(),
                Percentage = total == 0
                    ? 0
                    : Math.Round(group.Count() * 100m / total, 1)
            })
            .OrderBy(item => item.Label)
            .ToList();
    }
}