using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlipMove
{
    public class Player
    {
        public Vector2 Position { get; set; }
        public Vector2 CenterPosition { get; set; }
        public int Strength { get; set; }
        private int _Vitality;
        public int Vitality { get { return _Vitality; } set { _Vitality = value; if (_Vitality > MaxVitality) _Vitality = MaxVitality; } }
        public int MaxVitality { get; set; }
        public int Defense { get; set; }
        private int _Mana;
        public int Mana { get { return _Mana; } set { _Mana = value; if (_Mana > MaxMana) _Mana = MaxMana; } }
        public int MaxMana { get; set; }
        public int Magic { get; set; }
        public int Level { get; set; }
        public long CashMoney { get; set; }
        public long Experience { get; set; }
        public long ExperienceToNextLevel { get { return ExperienceRequiredNextLevel - Experience; } }
        public long ExperienceRequiredNextLevel { get { return (Level * 100 * 3); } }
        public const char Sprite = 'P';
        public bool DrawnStats { get; set; }
        private string _HelpText;
        public string HelpText { get { return _HelpText; } }
        public bool IsInInventory { get; set; }
        public int BattlePointer { get; set; }
        public int BattlePosition { get; set; }
        public bool BattleHit { get; set; }
        public bool GoingRight { get; set; }
        public bool GoingUp { get; set; }

        public InventoryManager Inventory { get; set; }
        public SpellManager Grimoire { get; set; }
        public bool InFight { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// Initializes the player with random values
        /// </summary>
        /// <param name="MinScale">Determines the lowest possible scale for the parameters</param>
        /// <param name="MaxScale">Determines the highest possible scale for the parameters</param>
        public Player(int MinScale, int MaxScale)
        {
            DrawnStats = false;
            MaxScale += 1;
            Inventory = new InventoryManager();
            Random rand = new Random(DateTime.Now.Millisecond);
            this.Strength = rand.Next(MinScale,MaxScale);
            this.MaxVitality = rand.Next(MinScale, MaxScale);
            this.Vitality = this.MaxVitality;
            this.Defense = rand.Next(MinScale, MaxScale);
            this.MaxMana = rand.Next(MinScale, MaxScale);
            this.Mana = this.MaxMana;
            this.Magic = rand.Next(MinScale, MaxScale);
            this.Inventory.MaxWeight = rand.Next(MinScale, MaxScale);
            this.Level = 1;
            this.CashMoney = 0;
            this.Experience = 0;
            this.InFight = false;
            this.GoingRight = true;
            this.GoingUp = true;
            this.BattlePointer = 0;
            InFight = true;

            Position = new Vector2(1,1);
            CenterPosition = new Vector2(0,0);

            int NumberOfItems = rand.Next(1, 4);

            while(Inventory.Items.Count < NumberOfItems)
            {
                Inventory.Items.Add(Settings.GiveRandomItem());
            }

            int NumberOfSpells= rand.Next(0, 2);

            while (Inventory.Items.Count < NumberOfItems)
            {
                Grimoire.Spells.Add(Settings.GiveRandomSpell());
            }

            if(File.Exists("help.txt"))
            { 
                using (StreamReader sr = new StreamReader("help.txt"))
                {
                    _HelpText = sr.ReadToEnd();
                }
            }
        }

        public bool Update(Map map)
        {
            if(!string.IsNullOrEmpty(Message))
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;

                    if(key == ConsoleKey.Enter || key == ConsoleKey.Spacebar 
                    || key == ConsoleKey.F1 || key == ConsoleKey.I 
                    || key == ConsoleKey.Escape)
                    {
                        Message = "";
                        Console.Clear();
                        DrawnStats = false;
                        IsInInventory = false;
                        return true;
                    }
                    else if(IsInInventory)
                    {
                        if(key == ConsoleKey.X || key == ConsoleKey.C)
                        {
                            Console.SetCursorPosition(3, 21);
                            Console.Write("Choose Item: ");
                            string response = Console.ReadLine();

                            int responseNumber = 0;
                            int.TryParse(response, out responseNumber); 

                            if(responseNumber > 0 && (responseNumber-1) < Inventory.Items.Count)
                            {
                                Console.Clear();
                                if(key == ConsoleKey.X)
                                {
                                    UseItem(Inventory.Items[responseNumber - 1]);
                                }
                                Message = key == ConsoleKey.X ? "Item consumed!" : "Item dropped!";
                                Inventory.Items.RemoveAt(responseNumber - 1);
                                IsInInventory = false;
                            }
                            Console.Clear();
                            DrawnStats = false;
                        }
                    }
                }
                return false;
            }

            bool Updated = false;
            if (!this.InFight)
            {
                if (Console.KeyAvailable)
                {
                    Console.Clear();
                    // Console.SetCursorPosition(Position.x,Position.y);
                    // Console.Write(" ");
                    Updated = true;
                    ConsoleKey key = Console.ReadKey(true).Key;
                    Vector2 TempPosition = new Vector2(this.Position.x, this.Position.y);
                    DrawnStats = false;
                    switch (key)
                    {
                        case ConsoleKey.LeftArrow:
                        case ConsoleKey.A: TempPosition.x--; break;
                        case ConsoleKey.RightArrow:
                        case ConsoleKey.D: TempPosition.x++; break;
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W: TempPosition.y--; break;
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S: TempPosition.y++; break;
                        case ConsoleKey.F1: Message = HelpText; break;
                        case ConsoleKey.I: Message = Inventory.GetInventoryMessage(); IsInInventory = true; break;
                    }

                    Item ItemFound = null;
                    if (TempPosition.x != Position.x || TempPosition.y != Position.y)
                    {
                        bool collides = map.DetectCollision(TempPosition, out ItemFound);

                        if (!collides)
                        {
                            Position = new Vector2(TempPosition.x, TempPosition.y);
                            CenterPosition = new Vector2(35 - Position.x, 10 - Position.y);
                            if (ItemFound != null)
                            {
                                Message = "You Found \"" + ItemFound.Name + "\"!!" + Environment.NewLine;
                                if (Inventory.CurrentWeight + ItemFound.Weight > Inventory.MaxWeight)
                                {
                                    Message += "Unfortunately, it is too heavy for you to carry.";
                                }
                                else
                                {
                                    ItemFound.Found = true;
                                    Inventory.Items.Add(ItemFound);
                                    map.ClearCaughtItems();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (Console.KeyAvailable)
                {
                    Console.Clear();
                    Updated = true;
                    if(!BattleHit)
                    { 
                        ConsoleKey key = Console.ReadKey(true).Key;
                        DrawnStats = false;
                        switch (key)
                        {
                            case ConsoleKey.Spacebar: BattleHit = true; break;
                        }
                    }
                }
            }

            LevelUp();

            return Updated;
        }

        public void Draw()
        {
            if (this.InFight)
            {
                Thread.Sleep(30);
                Console.SetCursorPosition(28,20);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("═════");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("════");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("══");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("════");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("═════");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("]");
                Console.SetCursorPosition(29+BattlePointer, 20);
                Console.Write("║");
                if (!BattleHit)
                { 
                    if(GoingRight)
                    {
                        BattlePointer++;
                        if(BattlePointer == 20)
                        {
                            GoingRight = false;
                        }
                    }
                    else
                    {
                        BattlePointer--;
                        if (BattlePointer == 0)
                        {
                            GoingRight = true;
                        }
                    }
                    //if (GoingUp)
                    //{
                    //    BattlePosition++;
                    //    if (BattlePosition == 5)
                    //    {
                    //        GoingUp = false;
                    //    }
                    //}
                    //else
                    //{
                    //    BattlePosition--;
                    //    if (BattlePosition == 0)
                    //    {
                    //        GoingUp = true;
                    //    }
                    //}
                    //for(int i = 0; i < 6; i++)
                    //{
                    //    Console.SetCursorPosition(30, 10+i);
                    //    Console.Write(" ");
                    //}
                    //Console.SetCursorPosition(30, 10+BattlePosition);
                    //Console.ForegroundColor = ConsoleColor.Green;
                    //Console.Write(Sprite);
                    //Console.ForegroundColor = ConsoleColor.White;
                }
            }
            if (!string.IsNullOrEmpty(Message) && !DrawnStats)
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.Write("╔");
                for (int i = 0; i < 78; i++) Console.Write("═");
                Console.Write("╗");
                for (int i = 1; i < 23; i++) Console.Write("║                                                                              ║");
                Console.Write("╚");
                for (int i = 0; i < 78; i++) Console.Write("═");
                Console.Write("╝");
                string[] sentences = Message.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                for(int i = 0; i < sentences.Length; i++)
                {
                    Console.SetCursorPosition(3, i+3);
                    Console.Write(sentences[i]);
                }

                if(IsInInventory)
                {
                    Console.SetCursorPosition(3, 20);
                    Console.WriteLine("X - CONSUME       Y - DROP");
                }

                DrawnStats = true;
                return;
            }
            //Console.SetCursorPosition(Position.x, Position.y);
            if(string.IsNullOrEmpty(Message) && !InFight)
            { 
                Console.SetCursorPosition(35, 10);
                CenterPosition = new Vector2(35 - Position.x, 10 - Position.y);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(Sprite);
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(5,0);
                Console.Write("╚══HELP:F1══╝");
            }

            Console.SetCursorPosition(0, 20);

            if(!DrawnStats)
            {
                DrawnStats = true;

                for(int i = 0; i < 80; i++) Console.Write("═");
                Console.SetCursorPosition(2, 22);
                Console.Write("VIT: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(Vitality.ToString() + "/" + MaxVitality.ToString());

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(2, 24);
                Console.Write("MAN: ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(Mana.ToString() + "/" + MaxMana.ToString());

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(22, 22);
                Console.Write("STR: ");
                Console.Write(Strength.ToString());
                
                Console.SetCursorPosition(22, 24);
                Console.Write("DEF: ");
                Console.Write(Defense.ToString());
                
                Console.SetCursorPosition(42, 22);
                Console.Write("MAG: ");
                Console.Write(Magic.ToString());

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.SetCursorPosition(42, 24);
                Console.Write("EXP: ");
                Console.Write(Experience + "/" + ExperienceRequiredNextLevel);

                Console.SetCursorPosition(62, 22);
                Console.Write("LVL: ");
                Console.Write(Level.ToString());
                
                Console.SetCursorPosition(62, 24);
                Console.Write("WGT: ");
                if (Inventory.CurrentWeight >= Inventory.MaxWeight)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.Write(Inventory.CurrentWeight + "/" + Inventory.MaxWeight);
                Console.ForegroundColor = ConsoleColor.White;
            }

        }

        public void LevelUp()
        {
            if(ExperienceToNextLevel <= 0)
            {
                Experience = -ExperienceToNextLevel;
                Level++;
                Random rand = new Random(DateTime.Now.Millisecond);
                int Strength = rand.Next(1, 3);
                int Vitality = rand.Next(1, 3);
                int Defense = rand.Next(1, 3);
                int Mana = rand.Next(1, 3);
                int Magic = rand.Next(1, 3);

                Message += Environment.NewLine;

                if (Strength > 0)
                {
                    this.Strength += Strength;
                    Message += "Strength went up " + Strength + " point";

                    if (Strength > 1)
                        Message += "s";

                    Message += Environment.NewLine;
                }

                if (MaxVitality > 0)
                {
                    this.MaxVitality += MaxVitality;
                    this.Vitality = this.MaxVitality;
                    Message += "Vitality went up " + MaxVitality + " point";

                    if (MaxVitality > 1)
                        Message += "s";

                    Message += Environment.NewLine;
                }

                if (Defense > 0)
                {
                    this.Defense += Defense;
                    Message += "Defense went up " + Defense + " point";

                    if (Defense > 1)
                        Message += "s";

                    Message += Environment.NewLine;
                }

                if (Mana > 0)
                {
                    this.MaxMana += Mana;
                    this.Mana = this.MaxMana;
                    Message += "Mana went up " + Mana + " point";

                    if (Mana > 1)
                        Message += "s";

                    Message += Environment.NewLine;
                }

                if (Magic > 0)
                {
                    this.Magic += Magic;
                    Message += "Magic went up " + Magic + " point";

                    if (Magic > 1)
                        Message += "s";

                    Message += Environment.NewLine;
                }

            }
        }

        public void UseItem(Item item)
        {
            for(int i = 0; i < item.StatTargets.Count; i++)
            {
                switch(item.StatTargets[i].Status)
                {
                    case Stats.Vitality: Vitality += item.StatTargets[i].Value; break;
                    case Stats.Mana: Mana += item.StatTargets[i].Value; break;
                }
            }
        }
    }
}
