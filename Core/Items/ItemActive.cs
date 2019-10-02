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
            this.GiveEffect();
            this.Quantity--;
        }

        public virtual void GiveEffect()
        { }

        // IEquatable
        public override string ToString()
        {
            return Name.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            ItemActive tempItem = obj as ItemActive;
            if (tempItem == null) return false;
            else return Equals(tempItem);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public bool Equals(ItemActive tempItem)
        {
            if (tempItem == null) return false;
            return this.Name.Equals(tempItem.Name);
        }
    }
}
