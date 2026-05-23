using FridgeSystem.Models;
using FridgeSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace FridgeSystem.Controllers;

public class AccountController : Controller
{
    private static readonly Dictionary<string, string> Users = new()
    {
        ["admin"] = "1234",
        ["user"] = "1234"
    };

    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!Users.TryGetValue(model.UserName, out var password) || password != model.Password)
        {
            ModelState.AddModelError(string.Empty, "帳號或密碼錯誤");
            return View(model);
        }

        HttpContext.Session.SetString("UserName", model.UserName);
        UsageLog.Add("登入", "帳號", $"{model.UserName} 登入系統", model.UserName);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        var userName = HttpContext.Session.GetString("UserName") ?? "訪客";
        UsageLog.Add("登出", "帳號", $"{userName} 登出系統", userName);
        HttpContext.Session.Clear();

        return RedirectToAction("Login");
    }
}
