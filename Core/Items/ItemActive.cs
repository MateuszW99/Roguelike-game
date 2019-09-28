using Game.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Items
{
    public class ItemActive : Item
    {
        private int quantity;
        public int Quantity
        {
            get => quantity;
            set
            {
                quantity = value;
            }
        }

        protected ItemActive(int x, int y) : base(x, y)
        { }

        public void Add(Player player)
        {
            if (!Player.Inventory.Actives.Contains(this))
            {
                Player.Inventory.Actives.Add(this);
            }
            Player.Inventory.Actives.Last().Quantity++;
            Game.MessageLog.Add($"You found the {this.Name}!");
        }

        public void Use(Player player, int? itemNumber)
        {
            if(itemNumber == null)
            {
                return;
            }
            if(this.Quantity <= 0)
            {
                Game.MessageLog.Add($"You don't have any {this.Name} to use.");
                return;
            }
            //player.Equipment[itemNumber].
            Player.Inventory.Actives[itemNumber.GetValueOrDefault()].Quantity--;
        }

        public virtual void GiveEffect()
        { }
    }
}
