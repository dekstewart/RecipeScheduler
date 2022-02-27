
namespace RecipeScheduler.Services
{
    public interface IFileOperations
    {
        Task<TowerTrays> GetTowerTrayInput();
        Task WriteScheduleOutput(RecipeSchedules recipeSchedules);
    }
}