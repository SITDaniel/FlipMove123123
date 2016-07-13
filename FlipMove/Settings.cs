using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipMove
{
    public class Settings
    {
        public static List<Item> Items;
        public static List<Spell> Spells;
        public static List<string> Preprefixes;
        public static List<string> Prefixes;

        public static void LoadTestValues()
        {
            Items = new List<Item>();
            Spells = new List<Spell>();

            for(int i = 0; i < 5;i++)
            { 
                Item item = new Item();
                item.IsConsumable = false;
                item.OriginalName = "sup" + (i+1).ToString();
                StatusEffect effect = new StatusEffect();
                effect.Duration = 0;
                effect.Status = Stats.Vitality;
                effect.Value = 10;
                item.StatTargets.Add(effect);
                item.Weight = 10;
                Items.Add(item);
            }
        }

        public static Item GiveRandomItem()
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            Item item = new Item();
            int i = rand.Next(0, Items.Count);
            Items[i].Clone(item);
            return item;
        }

        public static Spell GiveRandomSpell()
        {
            Spell spell = new Spell();
            return spell;
        }
    }
}
