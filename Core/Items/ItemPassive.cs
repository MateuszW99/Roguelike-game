﻿using Game.Interfaces;
using Game.Logic.MapGeneration;
using System.Linq;

namespace Game.Core.Items
{
    public class ItemPassive : Item
    {
        private int stats;
        public int Stats
        {
            get => stats;
            set
            {
                stats = value;
            }
        }

        protected ItemPassive(int x, int y) : base(x, y)
        {

        }

        public override void Add()
        {
            if (!Player.Inventory.Passives.Contains(this))
            {
                Player.Inventory.Passives.Add(this);
                Game.MessageLog.Add($"You found the {this.Name}!");
            }
            else
            {
                Game.MessageLog.Add($"You can't carry more than 1 {this.Name}.");
            }
        }

        public override void Use()
        {
            if(this is Sword)
            {
                Game.Player.Attack += Stats;
            }
            else if(this is Armor)
            {
                Game.Player.Defense += Stats;
            }
        }

        //public override void DropItem(Monster monster)
       // {

        //}
    }
}
