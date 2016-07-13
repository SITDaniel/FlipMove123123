using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipMove
{
    public class Game
    {
        public Player PlayerOne { get; set; }
        public Map CurrentMap { get; set; }

        public Game()
        {
            PlayerOne = new Player(10,60);
            CurrentMap = new Map(PlayerOne);
        }

        public void Update()
        {
            CurrentMap.Update();
            if(PlayerOne.Update(CurrentMap))
            {
                CurrentMap.PlayerUpdated();
            }
        }

        public void Draw()
        {
            CurrentMap.Draw();
            PlayerOne.Draw();
        }
    }
}

