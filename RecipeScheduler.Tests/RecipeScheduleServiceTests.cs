using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RecipeScheduler.Models;
using RecipeScheduler.Services;
using RecipeScheduler.Utilites;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RecipeScheduler.Tests
{
    public class RecipeScheduleServiceTests
    {
        private Mock<ILogger<RecipeScheduleService>> _loggerMock { get; }
        private Mock<IRecipeAPI> _recipeAPIMock { get; }
        private Mock<IFileOperations> _fileOperationsMock { get; }

        public RecipeScheduleServiceTests()
        {
            _loggerMock = new Mock<ILogger<RecipeScheduleService>>();
            _recipeAPIMock = new Mock<IRecipeAPI>();
            _fileOperationsMock = new Mock<IFileOperations>();
        }


        [Fact]
        public async Task RecipeScheduleRun_Success_VerifyAllCalls()
        {
            //1.Arrange
            var inputTrays = new TowerTrays
            {
                TowerTray = new List<TowerTray>
                {
                    new TowerTray
                    {
                         recipeName = "Basil",
                         startDate = "2022-02-24T12:30:00.0000000Z",
                         trayNumber = 1
                    },
                    new TowerTray
                    {
                         recipeName = "Tomatoes",
                         startDate = "2022-02-20T15:00:00.0000000Z",
                         trayNumber = 1
                    }
                }
            };

            var recipeAPIReturn = new Recipes
            {
                recipes = new List<Recipe>
                 {
                     new Recipe
                     {
                         name = "Basil",
                         lightingPhases = new List<LightingPhase>
                         {
                             new LightingPhase
                             {
                                 name = "phase1",
                                 order = 0,
                                 hours = 24,
                                 minutes = 0,
                                 repetitions = 2,
                                 operations = new List<Operation>
                                 {
                                     new Operation
                                     {
                                        lightIntensity = 1,
                                        offsetHours = 0,
                                        offsetMinutes = 0
                                     },
                                     new Operation
                                     {
                                         lightIntensity = 2,
                                         offsetHours = 6,
                                         offsetMinutes = 0
                                     },
                                     new Operation
                                     {
                                         lightIntensity = 0,
                                         offsetHours = 18,
                                         offsetMinutes = 0
                                     }
                                 }
                             }
                         },
                         wateringPhases = new List<WateringPhase>
                         {
                             new WateringPhase
                             {
                                 name = "water phase 1",
                                 amount = 10,
                                 order = 0,
                                 hours = 24,
                                 minutes = 0,
                                 repetitions = 2
                             }
                         }
                     }
                 }
            };

            _fileOperationsMock.Setup(x => x.GetTowerTrayInput()).ReturnsAsync(inputTrays);
            _recipeAPIMock.Setup(x => x.GetRecipes()).ReturnsAsync(recipeAPIReturn);

            var recipeScheduleService = ConstructRecipeScheduleService();

            //2.Act
            await recipeScheduleService.Run();

            //3.Assert
            _recipeAPIMock.VerifyAll();
            _recipeAPIMock.Verify(x => x.GetRecipes(), Times.Once());
            _recipeAPIMock.VerifyNoOtherCalls();

            _fileOperationsMock.VerifyAll();
            _fileOperationsMock.Verify(x => x.GetTowerTrayInput(), Times.Once());
            _fileOperationsMock.Verify(x => x.WriteScheduleOutput(It.IsAny<RecipeSchedules>()), Times.Once());
            _fileOperationsMock.VerifyNoOtherCalls();

        }

        [Fact]
        public void RecipeScheduleGetLightingSchedules_Success_VerifyReturnList()
        {
            //1. Arrange
            var recipeToUse = new Recipe
            {
                name = "Basil",
                lightingPhases = new List<LightingPhase> 
                {
                    new LightingPhase
                    {
                        name = "phase1",
                        order = 0,
                        hours = 24,
                        minutes = 0,
                        repetitions = 2,
                        operations = new List<Operation>
                        {
                            new Operation
                            {
                            lightIntensity = 1,
                            offsetHours = 0,
                            offsetMinutes = 0
                            },
                            new Operation
                            {
                                lightIntensity = 2,
                                offsetHours = 6,
                                offsetMinutes = 0
                            },
                            new Operation
                            {
                                lightIntensity = 0,
                                offsetHours = 18,
                                offsetMinutes = 0
                            }
                        }
                    }         
                }
            };
            string trayStartDate = "2022-02-24T12:30:00.0000000Z";
            
            int hoursToAdd = recipeToUse.lightingPhases.First().hours * recipeToUse.lightingPhases.First().repetitions;
            int minutesToAdd = recipeToUse.lightingPhases.First().minutes * recipeToUse.lightingPhases.First().repetitions;

            var lastScheduleEndDate = DateTimeUtils.ParseInputDate(trayStartDate).AddHours(hoursToAdd).AddMinutes(minutesToAdd);

            var recipeScheduleService = ConstructRecipeScheduleService();

            //2.Act
            var result = recipeScheduleService.GetLightingSchedules(recipeToUse, trayStartDate);

            //3.Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().BeOfType<List<LightingSchedule>>();
            result.Should().HaveCount(6);
            result.First().startDateTime.Should().BeEquivalentTo(trayStartDate);
            result.Last().endDateTime.Should().BeEquivalentTo(DateTimeUtils.ToUTCString(lastScheduleEndDate));
        }

        [Fact]
        public void RecipeScheduleGetWateringSchedules_Success_VerifyReturnList()
        {
            //1. Arrange
            var recipeToUse = new Recipe
            {
                name = "Basil",
                wateringPhases = new List<WateringPhase> 
                {
                    new WateringPhase
                    {
                       amount = 10,
                       name = "Phase1",
                       order = 0,
                       hours = 24,
                       minutes = 0,
                       repetitions = 3
                    },
                    new WateringPhase
                    {
                       amount = 10,
                       name = "Phase2",
                       order = 0,
                       hours = 24,
                       minutes = 0,
                       repetitions = 2
                    }
                }
            };
            string trayStartDate = "2022-02-24T12:30:00.0000000Z";

            int hoursToAdd = 0;
            int minutesToAdd = 0; 

            foreach(var phase in recipeToUse.wateringPhases)
            {
                hoursToAdd += phase.hours * phase.repetitions;
                minutesToAdd += phase.minutes * phase.repetitions;
            }

            var lastScheduleEndDate = DateTimeUtils.ParseInputDate(trayStartDate).AddHours(hoursToAdd).AddMinutes(minutesToAdd);

            var recipeScheduleService = ConstructRecipeScheduleService();

            //2.Act
            var result = recipeScheduleService.GetWateringSchedules(recipeToUse, trayStartDate);

            //3.Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().BeOfType<List<WateringSchedule>>();
            result.Should().HaveCount(5);
            result.First().startDateTime.Should().BeEquivalentTo(trayStartDate);
            result.Last().endDateTime.Should().BeEquivalentTo(DateTimeUtils.ToUTCString(lastScheduleEndDate));
        }

        private RecipeScheduleService ConstructRecipeScheduleService()
        {
            return new RecipeScheduleService(
                _loggerMock.Object,
                _recipeAPIMock.Object,
                _fileOperationsMock.Object
                );
        }
    }
}