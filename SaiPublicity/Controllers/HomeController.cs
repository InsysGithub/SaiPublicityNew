using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SaiPublicity.Data;
using SaiPublicity.Models;

namespace SaiPublicity.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ProjectDAL _projectDAL;

    public HomeController(ILogger<HomeController> logger , ProjectDAL projectDAL)
    {
        _logger = logger;
        _projectDAL = projectDAL;
    }

    public IActionResult Index()
    {
        var latestProjects = _projectDAL
                         .GetLatestProjectsByCategory()
                         .OrderBy(x => Guid.NewGuid())   // Shuffle
                         .Take(3)
                         .ToList();

        return View(latestProjects);

    }

    [Route("about-us")]
    public IActionResult About()
    {
        return View();
    }
    public IActionResult OurTeam()
    {
        return View();
    }

    [Route("Services")]

    public IActionResult Services()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
