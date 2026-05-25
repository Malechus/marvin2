using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using web.Models;
using System.Net;
using System.Net.Http;
using data.Services;
using System.Threading.Tasks;
using marvin2.Models.PiModels;
using marvin2.Models;
using marvin2.Models.WebModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace web.Controllers;

public class HomeController : Controller
{
    private readonly IConfigurationRoot _config;
    private readonly ILogger<HomeController> _logger;
    private readonly PiService _piService;
    private readonly ChoreContext _context;

    public HomeController(ILogger<HomeController> logger, IConfigurationRoot configurationRoot, PiService piService)
    {
        _logger = logger;
        _config = configurationRoot;
        _piService = piService;
        DbContextOptions<ChoreContext> options = new DbContextOptionsBuilder<ChoreContext>()
            .UseMySql(_config["Database:ConnectionString"], ServerVersion.AutoDetect(_config["Database:ConnectionString"]))
            .Options;
        _context = new ChoreContext(options);
    }

    public IActionResult Index()
    {
        ViewBag.Blocking = _piService.IsBlocking();
        ViewBag.TopClients = _piService.GetTopClients();
        ViewBag.TopBlocked = _piService.GetTopBlockedClients();
        
        // Get today's chores
        List<PersonChore> todaysChores = new List<PersonChore>();
        try
        {
            string todayName = DateTime.Now.DayOfWeek.ToString();
            int todayDate = DateTime.Now.Day;
            
            var dailyChores = _context.DailyChores
                .Include(pc => pc.Person)
                .Include(pc => pc.Chore)
                .Where(dc => dc.IsActive)
                .ToList();
            
            var weeklyChores = _context.WeeklyChores
                .Include(pc => pc.Person)
                .Include(pc => pc.Chore)
                .Where(wc => wc.IsActive && wc.DayOfWeek.ToLower() == todayName.ToLower())
                .ToList();
            
            var monthlyChores = _context.MonthlyChores
                .Include(pc => pc.Person)
                .Include(pc => pc.Chore)
                .Where(mc => mc.IsActive && mc.DayOfMonth == todayDate)
                .ToList();
            
            todaysChores.AddRange(dailyChores);
            todaysChores.AddRange(weeklyChores);
            todaysChores.AddRange(monthlyChores);
        }
        catch
        {
            // If chore retrieval fails, continue with empty list
        }
        
        ViewBag.TodaysChores = todaysChores;
        return View();
    }
    
    [HttpGet]
    public IActionResult Chores()
    {
        ViewBag.WeeklyChores = _context.WeeklyChores
            .Where(wc => wc.IsActive)
            .ToList();

        ViewBag.DailyChores = _context.DailyChores
            .Where(dc => dc.IsActive)
            .ToList();

        ViewBag.MonthlyChores = _context.MonthlyChores
            .Where(mc => mc.IsActive)
            .ToList();

        ChoreViewModel cvm = new ChoreViewModel();
        cvm.ChoreTypes.Where(s => s.Value == "dailychore").First().Selected = true;

        return View(cvm);
    }
    
    [HttpPost]
    public IActionResult Chores(ChoreViewModel cvmUpdated)
    {
        if (!cvmUpdated.IsValid())
        {
            cvmUpdated.AdditionalItem = true;
            cvmUpdated.Success = false;
            return View(cvmUpdated);
        }
        
        switch(cvmUpdated.ChoreTypes.SelectedValue)
        {
            case "dailychore":
                _context.DailyChores.Add(cvmUpdated.DailyChore);
                _context.SaveChanges();
                break;
            case "weeklychore":
                _context.WeeklyChores.Add(cvmUpdated.WeeklyChore);
                _context.SaveChanges();
                break;
            case "monthlychore":
                _context.MonthlyChores.Add(cvmUpdated.MonthlyChore);
                _context.SaveChanges();
                break;
            default:
                RedirectToAction("Error");
                break;
        }

        ChoreViewModel cvmFresh = new ChoreViewModel();
        cvmFresh.ChoreTypes.Where(s => s.Value == "dailychore").First().Selected = true;
        cvmFresh.AdditionalItem = true;
        cvmFresh.Success = true;

        return View(cvmFresh);
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
