using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeScheduler.Models
{
    public class LightingPhase
    {
        public IList<Operation> operations { get; set; }
        public string name { get; set; }
        public int order { get; set; }
        public int hours { get; set; }
        public int minutes { get; set; }
        public int repetitions { get; set; }
    }
}
