using FridgeSystem.Data;
using FridgeSystem.Models;
using FridgeSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace FridgeSystem.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

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

        var user = _context.Users.FirstOrDefault(u => u.Username == model.UserName);

        if (user == null || user.PasswordHash != model.Password)
        {
            ModelState.AddModelError(string.Empty, "帳號或密碼錯誤");
            return View(model);
        }

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserName", user.Username);

        UsageLog.Add("登入", "帳號", $"{user.Username} 登入系統", user.Username);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var exists = _context.Users.Any(u => u.Username == model.UserName);

        if (exists)
        {
            ModelState.AddModelError(string.Empty, "此帳號已被註冊");
            return View(model);
        }

        var user = new User
        {
            Username = model.UserName,
            PasswordHash = model.Password,
            CreatedAt = DateTime.Now
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return RedirectToAction("Login");
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