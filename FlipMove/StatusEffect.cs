using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipMove
{
    public class StatusEffect
    {
        public Stats Status { get; set; }
        public int Value { get; set; }
        public int Duration { get; set; } //in turns  (0 means instant)
    }
}
