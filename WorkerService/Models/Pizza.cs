namespace WorkerService.Models
{
    public class Pizza
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsGlutenFree { get; set; }
    }

    public class DotPizza
    {
        public int Id { get; set; }
        public string Information { get; set; }// Name + IsGlutenFree

    }
}