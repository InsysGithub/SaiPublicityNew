using Microsoft.AspNetCore.Mvc;
using SaiPublicity.Data;
using SaiPublicity.Models;

namespace SaiPublicity.Controllers
{
  
    public class AdminPanelController : Controller
    {
        private readonly AdminDAL _adminDAL;
        private readonly TestimonialDAL _testimonialDAL;
        private readonly VideoDAL _videoDAL;
        private readonly ProjectDAL _projectDAL;


        public AdminPanelController(AdminDAL adminDAL, TestimonialDAL testimonialDAL, VideoDAL videoDAL, ProjectDAL projectDAL)
        {
            _adminDAL = adminDAL;
            _testimonialDAL = testimonialDAL;
            _videoDAL = videoDAL;
            _projectDAL = projectDAL;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(AdminLoginModel model)
        {
            if (ModelState.IsValid)
            {
                bool isValidUser = _adminDAL.IsAdminValid(model);

                if (isValidUser)
                {
                    HttpContext.Session.SetString("UserName", model.UserName);
                    return RedirectToAction("Dashboard");
                }

                ModelState.AddModelError(string.Empty, "Invalid login credentials.");
            }
            ViewBag.Message = "Invalid login credentials.";
            return View(model);
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                TempData["ErrorMessage"] = "Please login first.";
                return RedirectToAction("Index");
            }

            ViewBag.TotalTestimonials = _testimonialDAL.GetTotalTestimonialsCount();
            ViewBag.TotalVideos = _videoDAL.GetTotalVideoCount();
            ViewBag.TotalProject = _projectDAL.GetTotalProjectCount();

            ViewData["Layout"] = "~/Views/Shared/_LayoutAdminPanel.cshtml";
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear all session data
            return RedirectToAction("Index", "Home");
        }
    }
}
