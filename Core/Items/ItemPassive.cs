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

        public override void Add(Player player)
        {
            if (!player.Passives.Contains(this))
            {
                player.Passives.Add(this);
                Game.MessageLog.Add($"You found the {this.Name}!");
            }
            else
            {
                Game.MessageLog.Add($"You can't carry more than 1 {this.Name}.");
            }
        }

        public override void Use(Player player, int? itemNumber)
        {
            if(this is Sword)
            {
                player.Attack += Stats;
            }
            else if(this is Armor)
            {
                player.Defense += Stats;
            }
        }

        public override void DropItem(Monster monster)
        {
            DungeonMap.Items.Add(this);
            Game.MessageLog.Add($"  {monster.Name} died and dropped {this.Name}.");
        }
    }
}
