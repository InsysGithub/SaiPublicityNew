using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SaiPublicity.Data;
using SaiPublicity.Models;

namespace SaiPublicity.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class ProjectController : Controller
    {
        private readonly ProjectDAL _projectDal;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ProjectController(ProjectDAL projectDal, IWebHostEnvironment webHostEnvironment)
        {
            _projectDal = projectDal;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var categories = _projectDal.GetProjectCategoryList();
            return View(categories);
        }

        // Show selected category page with form and project list
        public IActionResult List(int id)
        {
            var model = new ProjectPageViewModel
            {
                CategoryId = id,
                Category = _projectDal.GetProjectCategoryById(id),
                Projects = _projectDal.GetProjectsByCategory(id)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProject(ProjectModel model)
        {
            // Handle file upload
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "project");
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                string extension = Path.GetExtension(model.ImageFile.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                string fileName = $"project_{timestamp}{extension}";

                string filePath = Path.Combine(uploadsFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(fileStream);
                }

                model.ProjectImage = fileName;
            }

            // If ProjectDate not provided, set to today
            if (model.ProjectDate == default(DateTime) || model.ProjectDate == DateTime.MinValue)
            {
                model.ProjectDate = DateTime.Now;
            }

            // Add project to database
            _projectDal.AddProject(model);

            // Redirect back to the list page of the same category
            return RedirectToAction("List", new { id = model.ProjectCategoryId });
        }

        // GET: Show Edit form
        public IActionResult EditProject(int id)
        {
            var project = _projectDal.GetProjectById(id);
            if (project == null)
                return NotFound();

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProject(ProjectModel model)
        {
            var existingProject = _projectDal.GetProjectById(model.ProjectId);

            if (existingProject == null)
                return NotFound();

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "project");
            Directory.CreateDirectory(uploadsFolder);

            // Handle new image upload
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string extension = Path.GetExtension(model.ImageFile.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                string fileName = $"project_{timestamp}{extension}";

                string filePath = Path.Combine(uploadsFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(fileStream);
                }

                // Delete old image if exists
                if (!string.IsNullOrEmpty(existingProject.ProjectImage))
                {
                    string oldImagePath = Path.Combine(uploadsFolder, existingProject.ProjectImage);
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                model.ProjectImage = fileName;
            }
            else
            {
                // Retain old image if no new image uploaded
                model.ProjectImage = existingProject.ProjectImage;
            }

            // Update project in database
            _projectDal.UpdateProject(model);

            return RedirectToAction("List", new { id = model.ProjectCategoryId });
        }

        public IActionResult DeleteProject(int id, int categoryId)
        {
            _projectDal.DeleteProject(id);
            return RedirectToAction("List", new { id = categoryId });
        }
    }
}
