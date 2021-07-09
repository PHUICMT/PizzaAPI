namespace WorkerService.Models
{
    public class Pizza
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public bool IsGlutenFree { get; set; }
    }

    public class DotPizza
    {
        public string Guid { get; set; }
        public string Information { get; set; }// Name + IsGlutenFree

    }
}