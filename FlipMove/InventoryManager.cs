using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipMove
{
    public class InventoryManager
    {   
        public List<Item> Items { get; set; }
        public int CurrentWeight { get { return Items.Sum(i => i.Weight); } }
        public int MaxWeight { get; set; }
        
        public InventoryManager()
        {
            Items = new List<Item>();
        }

        public string GetInventoryMessage()
        {
            string result = "";

            for(int i = 0; i<Items.Count; i++)
            {
                result += (i+1).ToString() + ") " + Items[i].Name + " - WGT: " + 
                          Items[i].Weight + " - Value: " + Items[i].StatTargets[0].Value + " Effects: " + 
                          Items[i].StatTargets[0].Status.ToString() +  Environment.NewLine;
            }

            return result;
        }
    }

}
