using Game.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Items
{
    public class ItemActive : Item, IEquatable<ItemActive>
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

        protected ItemActive(int? x, int? y) : base(x, y)
        {
        }


        public override void Use()
        {
            if (this.Quantity <= 0)
            {
                Game.MessageLog.Add($"You don't have any {this.Name} to use.");
                return;
            }
            GiveEffect();
            Quantity--;
            if (this.Quantity <= 0)
            {
                Player.Inventory.Actives.Remove(this);
            }
            }

        public virtual void GiveEffect()
        { }

        public static void RandomDrop(Monster monster)
        {
            int randomActive = Game.Random.Next(1, 5);
            switch (randomActive)
            {
                case 1:
                    {
                        return;
                    }
                case 2:
                    {
                        ScrollOfTeleport scroll = new ScrollOfTeleport(monster.X, monster.Y);
                        scroll.DropItem(monster);
                        return;
                    }
                case 3:
                    {
                        ScrollOfDestruction scroll = new ScrollOfDestruction(monster.X, monster.Y);
                        scroll.DropItem(monster);
                        return;
                    }
                case 4:
                case 5:
                    {
                        HealthPotion potion = new HealthPotion(monster.X, monster.Y);
                        potion.DropItem(monster);
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        // IEquatable
        public override string ToString()
        {
            return Name.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj == null ? false : !(obj is ItemActive tempItem) ? false : Equals(tempItem);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public bool Equals(ItemActive tempItem)
        {
            return tempItem == null ? false : Name.Equals(tempItem.Name);
        }
    }
}
