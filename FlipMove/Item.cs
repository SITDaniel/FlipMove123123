using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipMove
{
    public class Item
    {
        public string PrePrefix { get; set; }
        public string Prefix { get; set; }
        public string OriginalName { get; set; }
        public int Weight { get; set; }
        public bool IsConsumable { get; set; }
        public Vector2 Position { get; set; }

        public string Name {
            get
            {
                string name = "";

                if (!string.IsNullOrEmpty(PrePrefix))
                    name += PrePrefix + " ";

                if (!string.IsNullOrEmpty(Prefix))
                    name += Prefix + " ";

                name += OriginalName;

                return name;
            }
        }
        
        public List<StatusEffect> StatTargets { get; set; }
        public bool Found { get; set; }

        public Item()
        {
            StatTargets = new List<StatusEffect>();
            Position = new Vector2(0,0);
        }

        public void Clone(Item item)
        {
            item.PrePrefix = this.PrePrefix;
            item.Prefix = this.Prefix;
            item.OriginalName = this.OriginalName;
            item.Weight = this.Weight;
            item.Position = new Vector2(Position.x,Position.y);
            item.StatTargets = new List<StatusEffect>();
            for (int i = 0; i < this.StatTargets.Count; i++)
            {
                item.StatTargets.Add(this.StatTargets[i]);
            }
        }

    }
}
