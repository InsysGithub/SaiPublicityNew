using SaiPublicity.Data;
using SaiPublicity.Models;
using Microsoft.AspNetCore.Mvc;

namespace SaiPublicity.Areas.AdminPanel.Controllers
{

    [Area("AdminPanel")]
    public class TestimonialController : Controller
    {
        private readonly TestimonialDAL _testimonialDAL;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TestimonialController(TestimonialDAL testimonialDAL, IWebHostEnvironment webHostEnvironment)
        {
            _testimonialDAL = testimonialDAL;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /AdminPanel/Testimonial
        public IActionResult Index()
        {
            var testimonials = _testimonialDAL.GetAllTestim();
            return View(testimonials);
        }

        // GET: AddTestim
        public IActionResult AddTestim()
        {
            return View();
        }

        // POST: AddTestim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTestim(TestimonialModel testim)
        {
            if (testim.ImageFile != null && testim.ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "testimonials");
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                string extension = Path.GetExtension(testim.ImageFile.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                string fileName = $"testim_{timestamp}{extension}";



                string filePath = Path.Combine(uploadsFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    testim.ImageFile.CopyTo(fileStream);
                }

                testim.TestimProfile = fileName;
            }
            else
            {
                // If no image uploaded, set default "nophoto" image
                testim.TestimProfile = "nophoto.png";
            }

            testim.TestId = _testimonialDAL.NextId("Testimonials", "TestId");

            // Handle date: if user didn’t select, use today’s date
            if (testim.TestDate == default(DateTime) || testim.TestDate == DateTime.MinValue)
            {
                testim.TestDate = DateTime.Now;
            }

            _testimonialDAL.AddTestim(testim);
            return RedirectToAction("Index", "Testimonial", new { area = "AdminPanel" });
        }

        // GET: EditTestim
        public IActionResult EditTestim(int id)
        {
            var testimonial = _testimonialDAL.GetTestimById(id);
            if (testimonial == null)
            {
                return NotFound();
            }
            return View(testimonial);
        }


        // POST: EditTestim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTestim(TestimonialModel testim)
        {
            var existingTestim = _testimonialDAL.GetTestimById(testim.TestId);

            if (existingTestim == null)
            {
                return NotFound();
            }

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "testimonials");

            // Handle Remove Image Request
            if (testim.RemoveImage)
            {
                if (!string.IsNullOrEmpty(existingTestim.TestimProfile) && existingTestim.TestimProfile != "/uploads/testimonials/nophoto.png")
                {
                    string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingTestim.TestimProfile.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }


                // Assign default image path (make sure the default image exists)
                testim.TestimProfile = "nophoto.png";
            }

            // Handle new image upload
            if (testim.ImageFile != null && testim.ImageFile.Length > 0)
            {
                string extension = Path.GetExtension(testim.ImageFile.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                string fileName = $"testim_{timestamp}{extension}";



                string filePath = Path.Combine(uploadsFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    testim.ImageFile.CopyTo(fileStream);
                }

                testim.TestimProfile = fileName;
            }
            else if (!testim.RemoveImage)
            {
                // Retain the old image only if not removed
                testim.TestimProfile = existingTestim.TestimProfile;
            }

            // Update the record
            _testimonialDAL.UpdateTestim(testim);

            return RedirectToAction("Index", "Testimonial", new { area = "AdminPanel" });

        }

        // POST: DeleteTestim (Soft Delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTestim(int id)
        {
            try
            {
                _testimonialDAL.SoftDeleteTestim(id);
                return RedirectToAction("Index", "Testimonial", new { area = "AdminPanel" });

            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting testimonial.";
                return RedirectToAction("Index", "Testimonial", new { area = "AdminPanel" });

            }
        }
    }
}
