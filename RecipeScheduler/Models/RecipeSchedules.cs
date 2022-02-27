using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeScheduler.Models
{
    public class RecipeSchedules
    {
        public IList<RecipeSchedule> recipeSchedules { get; set; } = new List<RecipeSchedule>();
    }
}
