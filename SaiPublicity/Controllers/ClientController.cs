using Microsoft.AspNetCore.Mvc;
using SaiPublicity.Areas.AdminPanel.Controllers;
using SaiPublicity.Data;

namespace SaiPublicity.Controllers
{
    public class ClientController : Controller
    {
        private readonly TestimonialDAL _testimonialDAL;

        public ClientController(TestimonialDAL testimonialDAL)
        {
            _testimonialDAL = testimonialDAL;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Route("testimonial")]
        public IActionResult ClientTestim()
        {
            var testimonials = _testimonialDAL.GetAllTestim();

            return View(testimonials);
        }
    }
}
