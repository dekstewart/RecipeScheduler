namespace RecipeScheduler.Services
{
    public class FileOperations : IFileOperations
    {
        private readonly ILogger<FileOperations> _logger;
        private readonly FileOperationSettings _fileOperationSettings;

        public FileOperations(ILogger<FileOperations> logger, IOptions<FileOperationSettings> fileOperationSettings)
        {
            _logger = logger;
            _fileOperationSettings = fileOperationSettings.Value;
        }

        public async Task<TowerTrays> GetTowerTrayInput()
        {
            string jsonInputFile = Environment.CurrentDirectory + _fileOperationSettings.recipeInputFilePath;

            using (StreamReader jsonStreamReader = new StreamReader(jsonInputFile))
            {
                string jsonOutput = await jsonStreamReader.ReadToEndAsync();
                var result = JsonConvert.DeserializeObject<TowerTrays>(jsonOutput);

                return result;
            }    
        }

        public async Task WriteScheduleOutput(RecipeSchedules recipeSchedules)
        {
            string json = JsonConvert.SerializeObject(recipeSchedules, Formatting.Indented);
            string outputDir = Environment.CurrentDirectory + _fileOperationSettings.scheduleOutputPath;

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            await File.WriteAllTextAsync(outputDir + "schedule.json", json);
        }
    }
}
