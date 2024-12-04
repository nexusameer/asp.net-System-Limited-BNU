using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cycling.Models;
using Cycling.Services;

namespace Cycling.Controllers
{
    public class CycleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CycleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Display all cycles
        public IActionResult Index()
        {
            var cycles = _context.Cycles.ToList(); // Fetch all cycles from the database
            return View(cycles); // Pass the data to the view
        }

        // GET: Cycle/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cycle/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CycleDto cycleDto)
        {
            if (ModelState.IsValid)
            {
                string logoFileName = null;

                // Handle file upload if Logo is provided
                if (cycleDto.Logo != null && cycleDto.Logo.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/logos");
                    logoFileName = $"{Guid.NewGuid()}_{cycleDto.Logo.FileName}";
                    var filePath = Path.Combine(uploadsFolder, logoFileName);

                    // Ensure the directory exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await cycleDto.Logo.CopyToAsync(stream);
                    }
                }

                // Create a Cycle entity to save into the database
                var cycle = new Cycle
                {
                    Name = cycleDto.Name,
                    Gears = cycleDto.Gears,
                    Company = cycleDto.Company,
                    Logo = logoFileName // Save the file name in the database
                };

                _context.Cycles.Add(cycle);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cycleDto);
        }
        // GET: Cycle/Edit/5
        public IActionResult Edit(int id)
        {
            var cycle = _context.Cycles.FirstOrDefault(c => c.Id == id);
            if (cycle == null)
            {
                return NotFound();
            }

            var cycleDto = new CycleDto
            {
                Id = cycle.Id,
                Name = cycle.Name,
                Gears = cycle.Gears,
                Company = cycle.Company
            };
            return View(cycleDto);
        }

        // POST: Cycle/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CycleDto cycleDto)
        {
            if (id != cycleDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var cycle = _context.Cycles.FirstOrDefault(c => c.Id == id);
                if (cycle == null)
                {
                    return NotFound();
                }

                string logoFileName = cycle.Logo;

                // Handle file upload if a new Logo is provided
                if (cycleDto.Logo != null && cycleDto.Logo.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/logos");
                    logoFileName = $"{Guid.NewGuid()}_{cycleDto.Logo.FileName}";
                    var filePath = Path.Combine(uploadsFolder, logoFileName);

                    // Ensure the directory exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Delete the old file if it exists
                    if (!string.IsNullOrEmpty(cycle.Logo))
                    {
                        var oldFilePath = Path.Combine(uploadsFolder, cycle.Logo);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await cycleDto.Logo.CopyToAsync(stream);
                    }
                }

                // Update cycle entity
                cycle.Name = cycleDto.Name;
                cycle.Gears = cycleDto.Gears;
                cycle.Company = cycleDto.Company;
                cycle.Logo = logoFileName;

                _context.Cycles.Update(cycle);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cycleDto);
        }
        // GET: Cycle/Delete/5
        public IActionResult Delete(int id)
        {
            var cycle = _context.Cycles.FirstOrDefault(c => c.Id == id);
            if (cycle == null)
            {
                return NotFound();
            }
            return View(cycle);
        }

        // POST: Cycle/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var cycle = _context.Cycles.FirstOrDefault(c => c.Id == id);
            if (cycle != null)
            {
                _context.Cycles.Remove(cycle);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
