using Newtonsoft.Json.Converters;
using RecipeScheduler.Utilites;
using System.Globalization;

namespace RecipeScheduler.Services
{
    public class RecipeScheduleService : IRecipeScheduleService
    {
        private readonly ILogger<RecipeScheduleService> _logger;
        private readonly IRecipeAPI _recipeAPI;
        private readonly IFileOperations _fileOperations;

        public RecipeScheduleService(ILogger<RecipeScheduleService> logger, IRecipeAPI recipeAPI, IFileOperations fileOperations)
        {
            _logger = logger;
            _recipeAPI = recipeAPI;
            _fileOperations = fileOperations;
        }

        public async Task Run()
        {
            RecipeSchedules recipeSchedules = new RecipeSchedules();

            //Get input json from file
            var towerTrays = await _fileOperations.GetTowerTrayInput();
            _logger.LogInformation($"tray count {towerTrays.TowerTray.Count.ToString()}");

            //Call API to get recipes
            var apiRecipes = await _recipeAPI.GetRecipes();
            _logger.LogInformation($"api recipes count {apiRecipes.recipes.Count}");

            //loop through input matching on recipes
            foreach (TowerTray tray in towerTrays.TowerTray)
            {
                //find recipe from API call
                var recipeToUse = apiRecipes.recipes.FirstOrDefault(r => r.name == tray.recipeName);

                if (recipeToUse != null)
                {
                    _logger.LogInformation($"recipe to use {recipeToUse.name} start date {tray.startDate}");
                    var recipeSchedule = new RecipeSchedule
                    {
                        name = tray.recipeName,
                        trayNumber = tray.trayNumber
                    };

                    recipeSchedule.lightingSchedule = GetLightingSchedules(recipeToUse, tray.startDate);
                    recipeSchedule.wateringSchedule = GetWateringSchedules(recipeToUse, tray.startDate);

                    recipeSchedules.recipeSchedules.Add(recipeSchedule);
                }
            }

            //produce json string and save to file
            _logger.LogInformation($"recipeSchedules {JsonConvert.SerializeObject(recipeSchedules).ToString()}");
            await _fileOperations.WriteScheduleOutput(recipeSchedules);

            await Task.CompletedTask;
        }

        public List<LightingSchedule> GetLightingSchedules(Recipe recipeToUse, string trayStartDate)
        {
            List<LightingSchedule> lightingSchedules = new List<LightingSchedule>();

            //ensure order is correct from recipe API for lighting phases
            var orderedLightPhases = recipeToUse.lightingPhases.OrderBy(r => r.order).ToList();
            var repetitionStartDate = DateTimeUtils.ParseInputDate(trayStartDate);

            foreach (var phase in orderedLightPhases)
            {
                for (int i = 0; i < phase.repetitions; i++)
                {
                    var repetitionEndDate = repetitionStartDate.AddHours(phase.hours).AddMinutes(phase.minutes);
                    var trackingOperationDate = repetitionStartDate;

                    for (int j = 0; j < phase.operations.Count; j++)
                    {
                        var scheduleToAdd = new LightingSchedule
                        {
                            startDateTime = DateTimeUtils.ToUTCString(trackingOperationDate),
                            lightIntensity = phase.operations[j].lightIntensity
                        };

                        if (j != phase.operations.Count - 1)
                        {
                            int numberOfHoursToAdd = phase.operations[j + 1].offsetHours - phase.operations[j].offsetHours;
                            trackingOperationDate = trackingOperationDate.AddHours(numberOfHoursToAdd).AddMinutes(phase.operations[j + 1].offsetMinutes);
                            scheduleToAdd.endDateTime = DateTimeUtils.ToUTCString(trackingOperationDate);
                        }
                        else
                        {
                            //at the last operation so end date will be repetition end date
                            scheduleToAdd.endDateTime = DateTimeUtils.ToUTCString(repetitionEndDate);
                            //for next repetition make start date match the current end date.
                            repetitionStartDate = repetitionEndDate;
                        }

                        lightingSchedules.Add(scheduleToAdd);
                    }
                }
            }
            return lightingSchedules;
        }

        public List<WateringSchedule> GetWateringSchedules(Recipe recipeToUse, string trayStartDate)
        {
            List<WateringSchedule> wateringSchedule = new List<WateringSchedule>();

            //ensure order is correct from recipe API for watering phases
            var orderedWaterPhases = recipeToUse.wateringPhases.OrderBy(r => r.order).ToList();
            var waterPhaseTrackingDate = DateTimeUtils.ParseInputDate(trayStartDate);

            foreach (var phase in orderedWaterPhases)
            {
                for (int i = 0; i < phase.repetitions; i++)
                {
                    var scheduleToAdd = new WateringSchedule
                    {
                        startDateTime = DateTimeUtils.ToUTCString(waterPhaseTrackingDate),
                        amount = phase.amount
                    };

                    waterPhaseTrackingDate = waterPhaseTrackingDate.AddHours(phase.hours).AddMinutes(phase.minutes);
                    scheduleToAdd.endDateTime = DateTimeUtils.ToUTCString(waterPhaseTrackingDate);

                    wateringSchedule.Add(scheduleToAdd);
                }
            }
            return wateringSchedule;
        }
    }

}