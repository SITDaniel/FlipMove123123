using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipMove
{
    public class Door
    {
        public Vector2 Position { get; set;}
        public bool ConnectsRooms { get; set; }
        public bool IsHorizontal { get; set; }

        public Door()
        {
            Position = new Vector2(0,0);
            ConnectsRooms = false;
            IsHorizontal = false;
        }
        
        public Door(Vector2 StartUp)
        {
            Position = new Vector2(StartUp.x, StartUp.y);
            ConnectsRooms = true;
        }
    }
}
