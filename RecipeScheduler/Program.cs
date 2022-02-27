IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        services.Configure<RecipeAPISettings>(configuration.GetSection("RecipeAPISettings"));
        services.Configure<FileOperationSettings>(configuration.GetSection("FileOperationSettings"));

        services.AddHostedService<Worker>();
        services.AddSingleton<IRecipeScheduleService, RecipeScheduleService>();
        services.AddSingleton<IRecipeAPI, RecipeAPI>();
        services.AddSingleton<IFileOperations, FileOperations>();
    })
    .Build();

await host.RunAsync();
