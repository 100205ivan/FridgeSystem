namespace FridgeSystem.Models;

public class UsageRecord
{
    public DateTime Time { get; set; } = DateTime.Now;

    public string UserName { get; set; } = "訪客";

    public string Action { get; set; } = string.Empty;

    public string Target { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
