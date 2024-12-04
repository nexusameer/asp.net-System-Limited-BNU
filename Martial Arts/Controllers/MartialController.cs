using MartialArts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting; // Required for IWebHostEnvironment

namespace MartialArts.Controllers
{
    public class MartialController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public MartialController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // Index action to display all records
        public IActionResult Index()
        {
            var martialArts = _context.Martials.ToList();
            return View(martialArts);
        }

        // Details action to display a single martial art
        public IActionResult Details(int id)
        {
            var martialArt = _context.Martials.FirstOrDefault(m => m.Id == id);
            if (martialArt == null)
            {
                return NotFound();
            }

            return View(martialArt);
        }

        // GET: Martial/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Martial/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Martial martial, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    martial.ImagePath = "/images/" + uniqueFileName;
                }

                _context.Martials.Add(martial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(martial);
        }

        // GET: Martial/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var martialArt = await _context.Martials.FindAsync(id);
            if (martialArt == null)
            {
                return NotFound();
            }

            return View(martialArt);
        }

        // POST: Martial/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Martial martial, IFormFile imageFile)
        {
            if (id != martial.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(martial.ImagePath))
                        {
                            string oldImagePath = Path.Combine(_environment.WebRootPath, martial.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        string uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        martial.ImagePath = "/images/" + uniqueFileName;
                    }

                    _context.Update(martial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Martials.Any(m => m.Id == martial.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(martial);
        }

        // GET: Martial/Delete/5
        public IActionResult Delete(int id)
        {
            var martialArt = _context.Martials.FirstOrDefault(m => m.Id == id);
            if (martialArt == null)
            {
                return NotFound();
            }

            return View(martialArt);
        }

        // POST: Martial/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var martialArt = _context.Martials.FirstOrDefault(m => m.Id == id);
            if (martialArt == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(martialArt.ImagePath))
            {
                string imagePath = Path.Combine(_environment.WebRootPath, martialArt.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Martials.Remove(martialArt);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Martial art deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
