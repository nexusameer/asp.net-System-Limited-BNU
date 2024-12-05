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
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
            return View(products);
        }

        // GET: Product/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error loading categories: " + ex.Message);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View(product);
            }

            try
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "products");

                    Directory.CreateDirectory(uploadsFolder);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    product.ImagePath = Path.Combine("uploads", "products", uniqueFileName);
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating product: " + ex.Message);
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View(product);
            }
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            try
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error loading categories: " + ex.Message);
            }

            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? ImageFile)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View(product);
            }

            try
            {
                // If a new image is uploaded, replace the existing one
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(product.ImagePath))
                    {
                        string oldFilePath = Path.Combine(_env.WebRootPath, product.ImagePath);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Save the new image
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "products");

                    Directory.CreateDirectory(uploadsFolder);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    product.ImagePath = Path.Combine("uploads", "products", uniqueFileName);
                }

                // Update the product in the database
                _context.Update(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating product: " + ex.Message);
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View(product);
            }
        }
        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product); // Show delete confirmation view
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                // Delete associated image from server
                if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    string filePath = Path.Combine(_env.WebRootPath, product.ImagePath);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Products.Remove(product); // Remove product from database
                await _context.SaveChangesAsync(); // Commit changes
            }

            return RedirectToAction(nameof(Index)); // Redirect to product list
        }
        public IActionResult About()
        {
            return View("StaticPage");
        }

    }
}
