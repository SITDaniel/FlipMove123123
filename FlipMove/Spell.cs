using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipMove
{
    public class Spell
    {
        public string Name { get; set; }
        public int StatusValue { get; set; }
        public List<Stats> StatTargets { get; set; }
        public bool UsedAgaisntEnemies { get; set; }
        public int Duration { get; set; } //in turns

    }
}
