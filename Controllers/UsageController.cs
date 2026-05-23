using FridgeSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace FridgeSystem.Controllers;

public class UsageController : Controller
{
    public IActionResult Index()
    {
        return View(UsageLog.GetRecent(100));
    }
}
