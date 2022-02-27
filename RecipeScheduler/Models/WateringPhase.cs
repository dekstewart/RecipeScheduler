namespace RecipeScheduler.Models
{
    public class WateringPhase
    {
        public int amount { get; set; }
        public string name { get; set; }
        public int order { get; set; }
        public int hours { get; set; }
        public int minutes { get; set; }
        public int repetitions { get; set; }
    }
}
