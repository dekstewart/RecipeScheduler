namespace RecipeScheduler.Models
{
    public class Recipe
    {
        public string name { get; set; }
        public IList<LightingPhase> lightingPhases { get; set; }
        public IList<WateringPhase> wateringPhases { get; set; }
    }
}
