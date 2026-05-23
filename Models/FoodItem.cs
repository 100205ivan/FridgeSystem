using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models;

public class FoodItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "請輸入食材名稱")]
    [Display(Name = "食材名稱")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "請選擇分類")]
    [Display(Name = "分類")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "請輸入數量")]
    [Display(Name = "數量")]
    public string Quantity { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "放入日期")]
    public DateTime PutDate { get; set; } = DateTime.Today;

    [Display(Name = "保存位置")]
    public string? StoragePlace { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "有效期限")]
    public DateTime? ExpireDate { get; set; }

    [Display(Name = "狀態")]
    public string Status => GetStatus();

    [Display(Name = "備註")]
    public string? Note { get; set; }

    public string GetStatus(DateTime? today = null)
    {
        if (!ExpireDate.HasValue)
        {
            return "正常";
        }

        var currentDate = (today ?? DateTime.Today).Date;
        var expireDate = ExpireDate.Value.Date;
        var daysLeft = (expireDate - currentDate).Days;
        var keptDays = (currentDate - PutDate.Date).Days;

        if (expireDate < currentDate)
        {
            return "已過期";
        }

        if (expireDate == currentDate)
        {
            return "今天到期";
        }

        if (daysLeft <= 3)
        {
            return "快過期";
        }

        if (keptDays >= 30 && StoragePlace != "冷凍")
        {
            return "放太久";
        }

        return "正常";
    }
}
