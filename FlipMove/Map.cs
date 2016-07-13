using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipMove
{
    public class Map
    {
        public List<Room> Rooms { get; set; }
        public Player PlayerOne { get; set; }
        public int DifficultyScale { get; set; }

        public Map(Player PlayerOne)
        {
            Rooms = new List<Room>();
            DifficultyScale = 1;
            this.PlayerOne = PlayerOne;
            Rooms.Add(new Room(new Vector2(0,0)));
            Rooms[0].PlayerOne = PlayerOne;
        }

        public void Update()
        {
        }

        public void PlayerUpdated()
        {
            for(int i = 0; i < Rooms.Count; i++)
            {
                Rooms[i].Drawn = false;
            }
        }

        public void ClearCaughtItems()
        {
            foreach(Room room in Rooms)
            {
                for(int i = 0;i < room.Items.Count; i++)
                {
                    if(room.Items[i].Found)
                    {
                        room.Items.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public void Draw()
        {
            for (int i = 0; i < Rooms.Count; i++)
            {
                Rooms[i].Draw();
            }
        }

        public bool DetectCollision(Vector2 Position, out Item ItemFound)
        {
            bool collides = false;
            Door InDoor = null;
            ItemFound = null;
            for(int i = 0; i < Rooms.Count && !collides && InDoor == null && ItemFound == null; i++)
            {
                collides = Rooms[i].DetectCollision(Position, out InDoor, out ItemFound);
            }
            
            if(InDoor != null)
            {
                if(!InDoor.ConnectsRooms)
                { 
                    if(InDoor.IsHorizontal)
                    { 
                        Rooms.Add(new Room(new Vector2(InDoor.Position.x+1, InDoor.Position.y-1)));
                        Rooms[Rooms.Count - 1].Doors.Add(new Door(new Vector2(InDoor.Position.x+1,InDoor.Position.y)));
                        Rooms[Rooms.Count - 1].PlayerOne = PlayerOne;
                    }
                    else
                    {
                        Rooms.Add(new Room(new Vector2(InDoor.Position.x - 1, InDoor.Position.y + 1)));
                        Rooms[Rooms.Count - 1].Doors.Add(new Door(new Vector2(InDoor.Position.x, InDoor.Position.y+1)));
                        Rooms[Rooms.Count - 1].PlayerOne = PlayerOne;
                    }
                    InDoor.ConnectsRooms = true;
                }
            }

            return collides;
        }
    }
}
