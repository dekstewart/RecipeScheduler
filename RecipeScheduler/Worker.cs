namespace RecipeScheduler
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHost _host;
        private readonly IRecipeScheduleService _recipeScheduleService;

        public Worker(ILogger<Worker> logger, IHost host, IRecipeScheduleService recipeScheduleService)
        {
            _logger = logger;
            _host = host;
            _recipeScheduleService = recipeScheduleService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _recipeScheduleService.Run();
            await _host.StopAsync();
        }
    }
}