using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using web.Models;
using System.Net;
using System.Net.Http;
using data.Services;

namespace web.Controllers;

public class HomeController : Controller
{
    private readonly IConfigurationRoot _config;
    private readonly ILogger<HomeController> _logger;
    private readonly IPiService _piService;

    public HomeController(ILogger<HomeController> logger, IConfigurationRoot configurationRoot, IPiService piService)
    {
        _logger = logger;
        _config = configurationRoot;
        _piService = piService;
    }

    public IActionResult Index()
    {
        ViewBag.Blocking = _piService.IsBlockingAsync();
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
