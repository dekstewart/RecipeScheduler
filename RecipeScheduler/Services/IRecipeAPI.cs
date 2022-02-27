
namespace RecipeScheduler.Services
{
    public interface IRecipeAPI
    {
        Task<Recipes> GetRecipes();
    }
}