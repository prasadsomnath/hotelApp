using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
