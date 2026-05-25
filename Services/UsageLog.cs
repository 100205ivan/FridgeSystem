using FridgeSystem.Models;

namespace FridgeSystem.Services;

public static class UsageLog
{
    // TODO: Mock usage records for frontend development. Backend should replace this with persisted usage logs.
    private static readonly List<UsageRecord> Records = new()
    {
        new()
        {
            Time = DateTime.Now.AddMinutes(-35),
            UserName = "system",
            Action = "系統啟動",
            Target = "智慧冰箱",
            Description = "載入預設食材資料"
        }
    };

    public static IReadOnlyList<UsageRecord> GetRecent(int count = 50)
    {
        return Records
            .OrderByDescending(record => record.Time)
            .Take(count)
            .ToList();
    }

    public static void Add(string action, string target, string description, string? userName = null)
    {
        Records.Add(new UsageRecord
        {
            Time = DateTime.Now,
            UserName = string.IsNullOrWhiteSpace(userName) ? "訪客" : userName,
            Action = action,
            Target = target,
            Description = description
        });
    }
}
