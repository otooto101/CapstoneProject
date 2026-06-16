using Microsoft.AspNetCore.Mvc;

namespace LifeAdvisor.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
