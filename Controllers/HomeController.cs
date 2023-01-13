using Microsoft.AspNetCore.Mvc;

namespace Bingo.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}