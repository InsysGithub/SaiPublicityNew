using SaiPublicity.Data;
using SaiPublicity.Models;
using Microsoft.AspNetCore.Mvc;

namespace SaiPublicity.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class VideoController : Controller
    {

        private readonly VideoDAL _videoDAL;

        public VideoController(VideoDAL videoDAL)
        {
            _videoDAL = videoDAL;
        }

        public IActionResult Index()
        {
            var video = _videoDAL.GetAllVideo();
            return View(video);
        }

        [HttpGet]
        public IActionResult AddVideo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddVideo(VideoModel video)
        {
            if (ModelState.IsValid)
            {
                _videoDAL.AddVideo(video);
                return RedirectToAction("Index", "Video", new { area = "AdminPanel" });
            }

            return View(video);
        }

        [HttpGet]
        public IActionResult EditVideo(int id)
        {
            var video = _videoDAL.GetVideoById(id);
            if(video == null)
            {
                return NotFound();
            }
            return View(video);
        }

        [HttpPost]

        public IActionResult EditVideo(VideoModel video)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _videoDAL.UpdateVideo(video);
                    return RedirectToAction("Index","Video", new { area = "AdminPanel"});
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while updating the video.");
                }
            }
            return View(video);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteVideo(int id)
        {
            try
            {
                _videoDAL.SoftDeleteVideo(id);
                return RedirectToAction("Index", "Video", new { area = "AdminPanel" });

            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting news.";
                return RedirectToAction("Index", "Video", new { area = "AdminPanel" });

            }
        }
    }
}
