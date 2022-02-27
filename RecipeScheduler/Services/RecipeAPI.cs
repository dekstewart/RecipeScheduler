namespace RecipeScheduler.Services
{
    public class RecipeAPI : IRecipeAPI
    {
        private readonly ILogger<RecipeScheduleService> _logger;
        private readonly RecipeAPISettings _recipeAPISettings;
        private readonly HttpClient _httpClient;
        private readonly string _recipeEndpoint;

        public RecipeAPI(ILogger<RecipeScheduleService> logger, IOptions<RecipeAPISettings> recipeAPISettings)
        {
            _logger = logger;
            _recipeAPISettings = recipeAPISettings.Value;

            _httpClient = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true });
            _recipeEndpoint = $"{_recipeAPISettings.BaseURL}{_recipeAPISettings.RecipeEndpoint}";
        }

        public async Task<Recipes> GetRecipes()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_recipeEndpoint)
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var strResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Recipes>(strResponse);
        }


    }
}
