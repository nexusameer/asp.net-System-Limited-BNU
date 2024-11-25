using Cricket.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Cricket.Controllers;

public class TeamController : Controller
{
    private readonly ApplicationDbContext context;
    private readonly ILogger<TeamController> _logger;

    public TeamController(ApplicationDbContext context, ILogger<TeamController> logger)
    {
        this.context = context;
        _logger = logger;
    }

    public IActionResult Index()
    {
        try
        {
            var teams = context.Teams.ToList();
            _logger.LogInformation($"Retrieved {teams.Count} teams from database");
            return View(teams);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving teams: {ex.Message}");
            return View(new List<Cricket.Models.Team>());
        }
    }
    public IActionResult Create()
    {
        return View();
    }
}