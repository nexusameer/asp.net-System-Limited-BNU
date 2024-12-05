using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Willingo.Models;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using System;
using Willingo.Services;

namespace Willingo.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CategoryController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }
        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category != null)
            {
                if (!string.IsNullOrEmpty(category.ImagePath))
                {
                    string filePath = Path.Combine(_env.WebRootPath, category.ImagePath);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, IFormFile? ImageFile)
        {
            Console.WriteLine("Create method called");

            // Check ModelState validity
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is not valid");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                }
                return View(category);
            }

            // Check if an image file is provided
            if (ImageFile == null || ImageFile.Length == 0)
            {
                Console.WriteLine("No image file provided.");
                ModelState.AddModelError("ImageFile", "Please upload an image file.");
                return View(category);
            }

            try
            {
                // Process the uploaded file
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "categories");
                Directory.CreateDirectory(uploadsFolder); // Ensure directory exists

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                category.ImagePath = Path.Combine("uploads", "categories", uniqueFileName);

                // Add category to the database
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                Console.WriteLine("Category successfully created.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while creating the category. Please try again.");
                return View(category);
            }


        }
        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category, IFormFile? ImageFile)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            try
            {
                // If a new image is uploaded, handle the file
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(category.ImagePath))
                    {
                        string oldFilePath = Path.Combine(_env.WebRootPath, category.ImagePath);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Save the new image
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "categories");
                    Directory.CreateDirectory(uploadsFolder); // Ensure the folder exists

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    category.ImagePath = Path.Combine("uploads", "categories", uniqueFileName);
                }

                // Update the category in the database
                _context.Update(category);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the category. Please try again. " + ex.Message);
                return View(category);
            }
        }


    }
}
