
namespace RecipeScheduler.Services
{
    public interface IRecipeScheduleService
    {
        List<LightingSchedule> GetLightingSchedules(Recipe recipeToUse, string trayStartDate);
        List<WateringSchedule> GetWateringSchedules(Recipe recipeToUse, string trayStartDate);
        Task Run();
    }
}