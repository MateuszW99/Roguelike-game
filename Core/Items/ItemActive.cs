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
            if(!player.Actives.Contains(this))
            {
                player.Actives.Add(this);
            }
            player.Actives.Last().Quantity++;
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
            player.Actives[itemNumber.GetValueOrDefault()].Quantity--;

        }
    }
}
