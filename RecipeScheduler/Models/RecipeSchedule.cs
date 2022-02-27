using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeScheduler.Models
{
    public class RecipeSchedule
    {
        public string name { get; set; }
        public int trayNumber { get; set; }
        public IList<LightingSchedule> lightingSchedule { get; set; } = new List<LightingSchedule>();
        public IList<WateringSchedule> wateringSchedule { get; set; } = new List<WateringSchedule>();
    }
}
