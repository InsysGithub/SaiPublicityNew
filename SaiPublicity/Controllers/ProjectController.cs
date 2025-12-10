using Microsoft.AspNetCore.Mvc;
using SaiPublicity.Data;

namespace SaiPublicity.Controllers
{
    public class ProjectController : Controller
    {
        private readonly VideoDAL _dataAccess;
        private readonly ProjectDAL _projectDAL;

        public ProjectController(VideoDAL dataAccess, ProjectDAL projectDAL)
        {
            _dataAccess = dataAccess;
            _projectDAL = projectDAL;
        }
        public IActionResult Index()
        {
            var latestProjects = _projectDAL.GetLatestProjectsByCategory();
            return View(latestProjects);
        }
        [Route("projects/{slug}-{categoryId}")]

        public IActionResult ProjectDetail(int categoryId , string slug)
        {
            var projects = _projectDAL.GetAllProjectsByCategory(categoryId);
            if (projects == null || !projects.Any())
            {
                return NotFound();
            }
            ViewBag.CategoryName = projects.First().ProjectCategory ?? "Category";
            return View(projects);
        }

        [Route("videos")]
        public IActionResult ProjectVideo()
        {
            var video = _dataAccess.GetAllVideo();
            return View(video);
        }
    }
}
