using Cricket.Models;
using Cricket.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cricket.Controllers;

public class TeamController : Controller
{
    private readonly IWebHostEnvironment _environment;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TeamController> _logger;

    public TeamController(IWebHostEnvironment environment, ApplicationDbContext context, ILogger<TeamController> logger)
    {
        _environment = environment;
        _context = context;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var teams = _context.Teams.ToList(); // Get all teams from the database

        return View(teams); // Pass the teams to the view
    }

    [HttpGet]
    // GET: Create Team
    public IActionResult Create()
    {
        return View();
    }

    // POST: Create Team
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(TeamDto teamDto)
    {
        if (!ModelState.IsValid)
        {
            return View(teamDto);
        }

        string filePath = null;

        // Handle the logo file upload
        if (teamDto.Logo != null && teamDto.Logo.Length > 0)
        {
            // Generate a unique file name for the uploaded logo
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(teamDto.Logo.FileName);
            string logosDirectory = Path.Combine(_environment.WebRootPath, "images", "logos");

            // Ensure the directory exists
            if (!Directory.Exists(logosDirectory))
            {
                Directory.CreateDirectory(logosDirectory);
            }

            // Full path where the logo will be saved
            string logoFullPath = Path.Combine(logosDirectory, newFileName);

            // Save the uploaded file to the server
            using (var stream = System.IO.File.Create(logoFullPath))
            {
                teamDto.Logo.CopyTo(stream);
            }

            // Store the file path for the database
            filePath = Path.Combine("images", "logos", newFileName);
        }

        // Create a new team object
        var team = new Team
        {
            Name = teamDto.Name,
            Logo = filePath,
            Ranking = teamDto.Ranking,
            Region = teamDto.Region
        };

        // Save the team to the database
        _context.Teams.Add(team);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }


    public IActionResult Edit(int id)
    {
        var team = _context.Teams.FirstOrDefault(t => t.Id == id);
        if (team == null)
        {
            return NotFound();
        }

        var teamDto = new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            Logo = null,  // Don't send the logo file itself to the view
            Ranking = team.Ranking,
            Region = team.Region
        };

        return View(teamDto);
    }



    // POST: Edit Team
    [HttpPost]
    public IActionResult Edit(TeamDto teamDto)
    {
        string filePath = null;  // Declare filePath outside the if block

        if (teamDto.Logo != null && teamDto.Logo.Length > 0)
        {
            // Generate a new unique file name for the uploaded logo
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(teamDto.Logo.FileName);
            string logosDirectory = Path.Combine(_environment.WebRootPath, "images", "logos");

            // Ensure the directory exists
            if (!Directory.Exists(logosDirectory))
            {
                Directory.CreateDirectory(logosDirectory);
            }

            // Full path where the logo will be saved
            string logoFullPath = Path.Combine(logosDirectory, newFileName);

            // Save the uploaded file to the server
            using (var stream = System.IO.File.Create(logoFullPath))
            {
                teamDto.Logo.CopyTo(stream);
            }

            // Store the file path as a string
            filePath = Path.Combine("images", "logos", newFileName);
        }

        // Retrieve the team from the database
        var team = _context.Teams.FirstOrDefault(t => t.Id == teamDto.Id);
        if (team == null)
        {
            return NotFound();
        }

        // Update the team's properties
        team.Name = teamDto.Name;
        team.Ranking = teamDto.Ranking;
        team.Region = teamDto.Region;

        // If a new logo was uploaded, update the Logo property with the file path
        if (!string.IsNullOrEmpty(filePath))
        {
            team.Logo = filePath; // Assign the file path to the Logo property
        }

        // Save the changes to the database
        _context.Teams.Update(team);
        _context.SaveChanges();

        // Redirect to the index or the appropriate view after updating
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var team = _context.Teams.FirstOrDefault(t => t.Id == id);
        if (team == null)
        {
            return NotFound();
        }

        // Delete the logo file from the server
        if (!string.IsNullOrEmpty(team.Logo))
        {
            string logoPath = Path.Combine(_environment.WebRootPath, team.Logo);
            if (System.IO.File.Exists(logoPath))
            {
                System.IO.File.Delete(logoPath);
            }
        }

        // Remove the team from the database
        _context.Teams.Remove(team);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }


    // POST: Delete Team
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var team = _context.Teams.FirstOrDefault(t => t.Id == id);
        if (team == null)
        {
            return NotFound();
        }

        // Delete the logo file from the server
        if (!string.IsNullOrEmpty(team.Logo))
        {
            string logoPath = Path.Combine(_environment.WebRootPath, team.Logo);
            if (System.IO.File.Exists(logoPath))
            {
                System.IO.File.Delete(logoPath);
            }
        }

        // Remove the team from the database
        _context.Teams.Remove(team);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }


}
