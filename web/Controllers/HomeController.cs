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
        PopulateChoreViewBag();

        ChoreViewModel cvm = new ChoreViewModel();
        cvm.People = new SelectList(_context.People.OrderBy(p => p.Name).ToList(), "Id", "Name");
        cvm.Chores = new SelectList(_context.Chores.OrderBy(c => c.Name).ToList(), "Id", "Name");

        return View(cvm);
    }
    
    [HttpPost]
    public IActionResult Chores(ChoreViewModel cvmUpdated)
    {
        if (!cvmUpdated.IsValid())
        {
            cvmUpdated.AdditionalItem = true;
            cvmUpdated.Success = false;
            PopulateChoreViewBag();
            cvmUpdated.People = new SelectList(_context.People.OrderBy(p => p.Name).ToList(), "Id", "Name", cvmUpdated.PersonId);
            cvmUpdated.Chores = new SelectList(_context.Chores.OrderBy(c => c.Name).ToList(), "Id", "Name", cvmUpdated.ChoreId);
            return View(cvmUpdated);
        }

        Person? person = _context.People.Find(cvmUpdated.PersonId);
        Chore? chore = _context.Chores.Find(cvmUpdated.ChoreId);

        switch(cvmUpdated.SelectedChoreType)
        {
            case "dailychore":
                DailyChore dailyChore = new DailyChore { Person = person, Chore = chore, priority = cvmUpdated.DailyPriority };
                dailyChore.Activate();
                _context.DailyChores.Add(dailyChore);
                _context.SaveChanges();
                break;
            case "weeklychore":
                WeeklyChore weeklyChore = new WeeklyChore { Person = person, Chore = chore, DayOfWeek = cvmUpdated.WeeklyDayOfWeek };
                weeklyChore.Activate();
                _context.WeeklyChores.Add(weeklyChore);
                _context.SaveChanges();
                break;
            case "monthlychore":
                MonthlyChore monthlyChore = new MonthlyChore { Person = person, Chore = chore, DayOfMonth = cvmUpdated.MonthlyDayOfMonth };
                monthlyChore.Activate();
                _context.MonthlyChores.Add(monthlyChore);
                _context.SaveChanges();
                break;
            default:
                RedirectToAction("Error");
                break;
        }

        ChoreViewModel cvmFresh = new ChoreViewModel();
        cvmFresh.People = new SelectList(_context.People.OrderBy(p => p.Name).ToList(), "Id", "Name");
        cvmFresh.Chores = new SelectList(_context.Chores.OrderBy(c => c.Name).ToList(), "Id", "Name");
        cvmFresh.AdditionalItem = true;
        cvmFresh.Success = true;

        PopulateChoreViewBag();

        return View(cvmFresh);
    }

    [HttpPost]
    public IActionResult DeletePersonChore(int id)
    {
        PersonChore? personChore = _context.PersonChores.Find(id);
        if (personChore != null)
        {
            _context.PersonChores.Remove(personChore);
            _context.SaveChanges();
        }

        return RedirectToAction("Chores");
    }

    /// <summary>
    /// Loads the current daily/weekly/monthly chore assignments (with their Person and Chore
    /// navigation properties) into ViewBag for display in the Chores view.
    /// </summary>
    private void PopulateChoreViewBag()
    {
        ViewBag.WeeklyChores = _context.WeeklyChores
            .Include(wc => wc.Person)
            .Include(wc => wc.Chore)
            .Where(wc => wc.IsActive)
            .ToList();

        ViewBag.DailyChores = _context.DailyChores
            .Include(dc => dc.Person)
            .Include(dc => dc.Chore)
            .Where(dc => dc.IsActive)
            .ToList();

        ViewBag.MonthlyChores = _context.MonthlyChores
            .Include(mc => mc.Person)
            .Include(mc => mc.Chore)
            .Where(mc => mc.IsActive)
            .ToList();
    }

    [HttpGet]
    public IActionResult ManageChores()
    {
        ManageChoresViewModel vm = new ManageChoresViewModel
        {
            ExistingChores = _context.Chores.OrderBy(c => c.Name).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    public IActionResult ManageChores(ManageChoresViewModel vm)
    {
        if (string.IsNullOrWhiteSpace(vm.NewChore?.Name))
        {
            vm.Success = false;
            vm.ExistingChores = _context.Chores.OrderBy(c => c.Name).ToList();
            return View(vm);
        }

        _context.Chores.Add(new Chore
        {
            Name = vm.NewChore.Name,
            Description = vm.NewChore.Description,
            Notes = vm.NewChore.Notes
        });
        _context.SaveChanges();

        return RedirectToAction("ManageChores");
    }

    [HttpGet]
    public IActionResult EditChore(int id)
    {
        Chore? chore = _context.Chores.Find(id);
        if (chore == null)
        {
            return RedirectToAction("ManageChores");
        }

        return View(chore);
    }

    [HttpPost]
    public IActionResult EditChore(Chore chore)
    {
        Chore? existing = _context.Chores.Find(chore.Id);
        if (existing == null)
        {
            return RedirectToAction("ManageChores");
        }

        existing.Name = chore.Name;
        existing.Description = chore.Description;
        existing.Notes = chore.Notes;
        _context.SaveChanges();

        return RedirectToAction("ManageChores");
    }

    [HttpPost]
    public IActionResult DeleteChore(int id)
    {
        bool inUse = _context.PersonChores.Any(pc => pc.Chore != null && pc.Chore.Id == id);
        if (!inUse)
        {
            Chore? chore = _context.Chores.Find(id);
            if (chore != null)
            {
                _context.Chores.Remove(chore);
                _context.SaveChanges();
            }
        }

        return RedirectToAction("ManageChores");
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
