using System.Collections.Generic;
using System.Linq;
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
            for(int i = 0; i < Player.Inventory.Actives.Count; i++)
            {
                StringBuilder itemDescription = new StringBuilder();
                itemDescription.Append(Player.Inventory.Actives[i].Name);
                itemDescription.Append(" x ");
                itemDescription.Append(Player.Inventory.Actives[i].Quantity);
                console.Print(1, i + 1, itemDescription.ToString(), RLColor.Cyan);
            }
        }

        public static void AddToQuickBar(ItemActive item)
        {
            if (!Player.Inventory.Actives.Contains(item))
            {
                Player.Inventory.Actives.Add(item);
            }
            
            ItemActive tempItem = Player.Inventory.Actives.Find(x => x.Name == item.Name);
            if(tempItem != null)
            {
                tempItem.Quantity++;
            }
        }
    }
}
