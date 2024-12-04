namespace Cycling.Models
{
    public class CycleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Gears { get; set; }
        public string Company { get; set; }
        public IFormFile Logo { get; set; }
    }
}

