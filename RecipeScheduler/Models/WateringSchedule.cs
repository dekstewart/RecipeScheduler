using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeScheduler.Models
{
    public class WateringSchedule
    {
        public int amount { get; set; }
        public string startDateTime { get; set; }
        public string endDateTime { get; set; }
    }
}
