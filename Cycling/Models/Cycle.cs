namespace Cycling.Models
{
    public class Cycle
    {
        public int Id { get; set; }  // Primary Key by convention
        public string Name { get; set; } = "";
        public string Logo { get; set; } = "";
        public int Gears { get; set; }
        public string Company { get; set; } = "";
    }
}
