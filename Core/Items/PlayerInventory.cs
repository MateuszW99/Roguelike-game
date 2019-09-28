using System.Collections.Generic;
using System.Text;
using RLNET;

namespace Game.Core.Items
{
    public class PlayerInventory
    {
        public List<ItemActive> Actives;
        public List<ItemPassive> Passives;

        public PlayerInventory()
        {
            Actives = new List<ItemActive>();
            Passives = new List<ItemPassive>();
        }

        public void Draw(RLConsole console)
        {
            for(int i = 0; i < Actives.Count; i++)
            {
                StringBuilder itemDescription = new StringBuilder();
                itemDescription.Append(Actives[i].Name);
                itemDescription.Append(" x ");
                itemDescription.Append(Actives[i].Quantity);
                console.Print(1, i + 1, itemDescription.ToString(), RLColor.White);               
            }
        }

    }
}
