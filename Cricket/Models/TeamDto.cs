namespace Cricket.Models;
using System.ComponentModel.DataAnnotations;

public class TeamDto
{
    [Required]
    public string Name { get; set; } = "";
    public IFormFile? Logo { get; set; }

    public int Ranking { get; set; }

    public string Region { get; set; } = "";
}
