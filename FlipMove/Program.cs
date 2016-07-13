using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipMove
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Settings.LoadTestValues();
            Game game = new Game();
            Console.CursorVisible = false;

            while(true)
            {
                game.Update();

                game.Draw();
            }
        }
    }
}
