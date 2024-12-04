using MartialArts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MartialArts.Controllers
{
    public class MartialController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MartialController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Index action to display all records
        public IActionResult Index()
        {
            var martialArts = _context.Martials.ToList();
            if (martialArts == null)
            {
                // Log or debug statement
                Console.WriteLine("No martial arts found.");
            }
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
                // Save image file if provided
                if (imageFile != null && imageFile.Length > 0)
                {
                    try
                    {
                        // Create the "wwwroot/images" folder if it doesn't exist
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Save the file to the path
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        martial.ImagePath = "/images/" + uniqueFileName;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, $"Image upload failed: {ex.Message}");
                        return View(martial);
                    }
                }

                // Save martial art to the database
                _context.Martials.Add(martial);
                await _context.SaveChangesAsync();

                // Redirect to Index after successful creation
                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid, return to the same view with the model
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
                    // If a new image is uploaded, update the image path
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Delete the old image if it's being replaced
                        if (!string.IsNullOrEmpty(martial.ImagePath))
                        {
                            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", martial.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Save the new image
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        martial.ImagePath = "/images/" + uniqueFileName;
                    }

                    // Update the martial art record in the database
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

                // Redirect to Index after successful edit
                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid, return to the same view with the model
            return View(martial);
        }

        // GET: Martial/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var martialArt = await _context.Martials.FindAsync(id);
            if (martialArt == null)
            {
                return NotFound();
            }
            return View(martialArt);
        }

        // POST: Martial/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var martialArt = await _context.Martials.FindAsync(id);
            if (martialArt != null)
            {
                // Delete logic here...
                _context.Martials.Remove(martialArt);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
