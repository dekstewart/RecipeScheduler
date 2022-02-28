# Recipe Scheduler Console Application
First, ensure the RecipeAPI is up and running on `localhost:8080/swagger` or `localhost:8080/recipe` as per the instructions on `https://github.com/intelligent-growth-solutions/tech-test-software-engineer`

To run the recipe scheduler application navigate to where the directory with the RecipeScheduler.sln file and run the command

```
dotnet build
```

Alternatively, open the solution in Visual Studio 2022 and select Build > Build Solution in the menu.

Navigate to the build folder at 

```
RecipeScheduler\bin\Debug\net6.0
```

Either double click the `RecipeScheduler.exe` file or run it in the command line
```
RecipeScheduler.exe
```

A schedule.json file should be produced in the output folder in the same directory as the build folder.

## Thought process
I decided to use a console application to complete the tech test as I thought it would be quickest to produce a json file to save to disk.

I split the logic using service classes for various parts to allow for dependency injection and unit testing to take place i.e. 
* FileOperations contains methods that deal with reading and writing to disk
* RecipeAPI fetches the information from the recipe API with URL specified in appsettings.json
* RecipeScheduleService uses the other services to get what it needs and loops through each recipe to produce the schedules as needed.

## Assumptions
* input startDate for towers are generally in a yyyy-MM-dd and time format but for the Strawberries recipe name it appeared to be more American format with the yyy-dd-MM format so I allowed for that as an alternative date format when trying to parse this.
* In the recipe API I assume that anything relating to minutes did not go above 59 to make an hour+ although I don't think it should make too much of a difference.
* Any order number field from the recipe API happened to already be in ascending order but I made sure of it in code prior to producing any schedules.

## Improvements
* Add some error handling for where things may go wrong. Does it need to stop all schedules from being produced or only miss out the recipe in question?
* Add logging to the error handling to show where things go wrong.
* Implement logging to possibly write file to disk with serilog or some other place where errors can be searched upon e.g. datadog?
* Possibly update the Worker.cs file to run the RecipeSchedulerService on some kind of schedule itself or run as a scheduled task within Windows if deployed on a VM for example.
* Confirm if date format assumption is correct or to add some checking for valid dates from the input json.
* Json file produced is overwritten each time application is run. Add some code to make filename unique each time?
* Tidy up some classes where there may be some duplication in property names.
* More unit tests after the above to account for relevant new scenarios and any other ones missed so far.
