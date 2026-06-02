
using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "請輸入帳號")]
    [Display(Name = "帳號")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "請輸入密碼")]
    [DataType(DataType.Password)]
    [Display(Name = "密碼")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "請再次輸入密碼")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "兩次密碼不一致")]
    [Display(Name = "確認密碼")]
    public string ConfirmPassword { get; set; } = string.Empty;
}