using Microsoft.AspNetCore.Http; // For IFormFile
using System.ComponentModel.DataAnnotations;

namespace MartialArts.Models
{
    public class Martial
    {
        public int Id { get; set; } // Primary key

        public string Name { get; set; } = string.Empty; // Name of the martial art

        public string Description { get; set; } = string.Empty; // Short description of the martial art

        public string Origin { get; set; } = string.Empty; // Country or region of origin

        public int Popularity { get; set; } // Popularity score or ranking

        public string ImagePath { get; set; } = string.Empty; // File path for the image
    }
}
