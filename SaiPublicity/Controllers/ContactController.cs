using Microsoft.AspNetCore.Mvc;

namespace SaiPublicity.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
