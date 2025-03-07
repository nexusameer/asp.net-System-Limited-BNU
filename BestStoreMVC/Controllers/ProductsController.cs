﻿using Microsoft.AspNetCore.Mvc;
using BestStoreMVC.Services;
using BestStoreMVC.Models;

namespace BestStoreMVC.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext context;
    private readonly IWebHostEnvironment environment;

    public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        this.context = context;
        this.environment = environment;
    }

    public IActionResult Index()
    {
        var products = context.Products.ToList();
        return View(products);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(ProductDto productDto)
    {
        if (productDto.Image == null)
        {
            ModelState.AddModelError("Image", "Image is required");
        }

        if (!ModelState.IsValid)
        {
            return View(productDto);
        }

        // Save the image file
        string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(productDto.Image.FileName);
        string productsDirectory = Path.Combine(environment.WebRootPath, "products");

        if (!Directory.Exists(productsDirectory))
        {
            Directory.CreateDirectory(productsDirectory);
        }

        string imageFullPath = Path.Combine(productsDirectory, newFileName);

        using (var stream = System.IO.File.Create(imageFullPath))
        {
            productDto.Image.CopyTo(stream);
        }

        // Save the product in the database
        Product product = new Product
        {
            Name = productDto.Name,
            Image = newFileName
        };

        context.Products.Add(product);
        context.SaveChanges();

        return RedirectToAction("Index");
    }
    public IActionResult Edit(int id)
    {
        var product = context.Products.Find(id);
        if (product == null)
        {
            return RedirectToAction("Index", "Products");
        }
        var productDto = new ProductDto()
        {
            Name = product.Name,
        };

        ViewData["ProductId"] = product.ProductId;
        ViewData["Image"] = product.Image;
        return View(productDto);
    }
    [HttpPost]
    public IActionResult Edit(int id, ProductDto productDto)
    {
        var product = context.Products.Find(id);
        if (product == null)
        {
            return RedirectToAction("Index", "Products");
        }
        if(!ModelState.IsValid)
        {
            ViewData["ProductId"] = product.ProductId;
            ViewData["Image"] = product.Image;
            return View(productDto);
        }
        string newFileName = product.Image;
        if (productDto.Image != null)
        {
            newFileName =  DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.Image.FileName);

            string imageFullPath = environment.WebRootPath + "/products/" + newFileName;

            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.Image.CopyTo(stream);
            }
            string oldImageFullPath = environment.WebRootPath + "/products/" + product.Image;
            System.IO.File.Delete(oldImageFullPath);
        }
        product.Name = productDto.Name;
        product.Image = newFileName;
        context.SaveChanges();
        return RedirectToAction("Index", "Products");
        
    }
    public IActionResult Delete(int id)
    {
        var product = context.Products.Find(id);
        if (product == null)
        {
            return RedirectToAction("Index", "Products");
        }
        string imageFullPath = environment.WebRootPath + "/products/" + product.Image;
        System.IO.File.Delete(imageFullPath);
        context.Products.Remove(product);
        context.SaveChanges();
        return RedirectToAction("Index", "Products");
    }
}
