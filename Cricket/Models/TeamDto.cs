﻿namespace Cricket.Models;
using System.ComponentModel.DataAnnotations;

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Ranking { get; set; }
    public string Region { get; set; }
    public IFormFile Logo { get; set; } // This will be used to upload the new logo
}