using Microsoft.AspNetCore.Mvc;

namespace InterViewV2.Controllers
{
    public class VacancyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
