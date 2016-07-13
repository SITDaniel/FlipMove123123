using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipMove
{
    public class Room
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public List<Item> Items { get; set; }
        public int DifficultyScale { get; set; }
        public List<Door> Doors { get; set; }
        public bool Drawn { get; set; }
        public Player PlayerOne { get; set; }

        public Room(Vector2 StartPosition)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            this.Position = new Vector2(StartPosition.x, StartPosition.y);
            this.Size = new Vector2(rand.Next(5,10), rand.Next(5, 10));
            this.Items = new List<Item>();
            this.Doors = new List<Door>();

            int ndoors = 1;

            while(ndoors > 0)
            {
                Door door = new Door();
                switch(rand.Next(0, 2)) //finds out if the door will be vertical or horizontal
                {
                    case 0:
                        door.Position.x = this.Position.x + this.Size.x;
                        door.Position.y = rand.Next(this.Position.y+1,(this.Position.y+this.Size.y));
                        door.IsHorizontal = true;
                        break;
                    case 1:
                        door.Position.y = this.Position.y + this.Size.y;
                        door.Position.x = rand.Next(this.Position.x + 1, (this.Position.x + this.Size.x));
                        break;
                }

                this.Doors.Add(door);

                ndoors--;
            }

            Vector2 ItemPosition = new Vector2(0,0);
            ItemPosition.x = rand.Next(Position.x + 1, Position.x + Size.x);
            ItemPosition.y = rand.Next(Position.y + 1, Position.y + Size.y);
            this.Items.Add(Settings.GiveRandomItem());
            this.Items[Items.Count - 1].Position = ItemPosition;

        }

        public void Update()
        {

        }

        public static bool IsOutOfBounds(Vector2 Position)
        {
            return (Position.x < 0 || Position.x >= 80 || Position.y < 0 || Position.y >= 20);
        }

        public void Draw()
        {
            if(!Drawn)
            { 
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
            
                Vector2 StartPosition = new Vector2(PlayerOne.CenterPosition.x + this.Position.x, PlayerOne.CenterPosition.y + this.Position.y);
                
                if(!IsOutOfBounds(StartPosition))
                { 
                    Console.SetCursorPosition(StartPosition.x,StartPosition.y ); // top left
                    Console.Write("╔");
                }

                if (!IsOutOfBounds(new Vector2(StartPosition.x + this.Size.x, StartPosition.y)))
                {
                    Console.SetCursorPosition(StartPosition.x + this.Size.x, StartPosition.y); //top right
                    Console.Write("╗");
                }

                if (!IsOutOfBounds(new Vector2(StartPosition.x, StartPosition.y + this.Size.y)))
                {
                    Console.SetCursorPosition(StartPosition.x, StartPosition.y + this.Size.y); //bottom left
                    Console.Write("╚");
                }

                if (!IsOutOfBounds(new Vector2(StartPosition.x + this.Size.x, StartPosition.y + this.Size.y)))
                {
                    Console.SetCursorPosition(StartPosition.x + this.Size.x, StartPosition.y + this.Size.y); //bottom right
                    Console.Write("╝");
                }

                for(int x = StartPosition.x+1; x < (StartPosition.x + this.Size.x); x++)
                {
                    if (!IsOutOfBounds(new Vector2(x, StartPosition.y)))
                    {
                        Console.SetCursorPosition(x, StartPosition.y); // top left
                        Console.Write("═");
                    }

                    if (!IsOutOfBounds(new Vector2(x, StartPosition.y + this.Size.y)))
                    {
                        Console.SetCursorPosition(x, StartPosition.y + this.Size.y); // top left
                        Console.Write("═");
                    }
                }

                for (int y = StartPosition.y + 1; y < (StartPosition.y + this.Size.y); y++)
                {
                    if (!IsOutOfBounds(new Vector2(StartPosition.x, y)))
                    {
                        Console.SetCursorPosition(StartPosition.x, y); // top left
                        Console.Write("║");
                    }
                    if (!IsOutOfBounds(new Vector2(StartPosition.x + this.Size.x, y)))
                    {
                        Console.SetCursorPosition(StartPosition.x + this.Size.x, y); // top left
                        Console.Write("║");
                    }
                }

                for (int i = 0; i < this.Doors.Count; i++)
                {
                    if (!IsOutOfBounds(new Vector2(PlayerOne.CenterPosition.x + this.Doors[i].Position.x, PlayerOne.CenterPosition.y + this.Doors[i].Position.y)))
                    {
                        Console.SetCursorPosition(PlayerOne.CenterPosition.x + this.Doors[i].Position.x, PlayerOne.CenterPosition.y + this.Doors[i].Position.y);
                        Console.Write(" ");
                    }
                }

                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (!IsOutOfBounds(new Vector2(PlayerOne.CenterPosition.x + this.Items[i].Position.x, PlayerOne.CenterPosition.y + this.Items[i].Position.y)))
                    {
                        Console.SetCursorPosition(PlayerOne.CenterPosition.x + this.Items[i].Position.x, PlayerOne.CenterPosition.y + this.Items[i].Position.y);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("*");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                Drawn = true;
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public bool DetectCollision(Vector2 Position, out Door InDoor, out Item ItemFound)
        {
            ItemFound = null;
            InDoor = null; //used to save iterations

            //checks if the player is currently in a door
            for(int i = 0; i < Doors.Count; i++)
            {
                if (Position.x == (Doors[i].Position.x) && Position.y == (Doors[i].Position.y))
                {
                    InDoor = Doors[i];
                    return false;
                }
            }
            
            //checks if player is going agaisnt the wall
            if((Position.x == this.Position.x || Position.x == this.Position.x + this.Size.x) 
            && (Position.y >= this.Position.y && Position.y <= (this.Position.y + this.Size.y)) ) //left-right wall
            {
                return true;
            }
            else if ((Position.y == this.Position.y || Position.y == this.Position.y + this.Size.y)
            && (Position.x >= this.Position.x && Position.x <= (this.Position.x + this.Size.x))) //top-down wall
            {
                return true;
            }
            
            //checks if the player stepped on an item
            for(int i = 0; i < Items.Count; i++)
            {
                if (Position.x == (Items[i].Position.x) && Position.y == (Items[i].Position.y))
                {
                    ItemFound = Items[i];
                    //Items.RemoveAt(i);
                    return false;
                }
            }

            return false;
        }
    }
}
