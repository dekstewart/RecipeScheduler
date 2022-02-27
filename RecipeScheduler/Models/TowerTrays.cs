namespace RecipeScheduler.Models
{
    public class TowerTrays
    {
        [JsonProperty("input")]
        public IList<TowerTray> TowerTray { get; set; }
    }
}
