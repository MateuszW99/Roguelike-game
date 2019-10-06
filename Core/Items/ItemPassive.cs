using Game.Interfaces;
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
            else if(this is Chestplate)
            {
                Game.Player.Defense += Stats;
            }
            else if(this is Boots)
            {
                Game.Player.Speed += Stats;
            }
        }

        public static void RandomDrop(Monster monster)
        {
            int randomPassive = (Game.Random.Next(1, 3));
            switch (randomPassive)
            {
                case 1:
                    {
                        Sword sword = new Sword(monster.X, monster.Y);
                        sword.DropItem(monster);
                        return;
                    }
                case 2:
                    {
                        Chestplate chestplate = new Chestplate(monster.X, monster.Y);
                        chestplate.DropItem(monster);
                        return;
                    }
                case 3:
                    {
                        Boots boots = new Boots(monster.X, monster.Y);
                        boots.DropItem(monster);
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }
    }
}
