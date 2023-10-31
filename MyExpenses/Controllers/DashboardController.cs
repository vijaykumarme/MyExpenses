using Microsoft.AspNetCore.Mvc;

namespace MyExpenses.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
