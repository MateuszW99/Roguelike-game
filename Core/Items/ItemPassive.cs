using Game.Interfaces;
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

        }

        //protected override void DropItem(Monster monster)
        //{

        //}
    }
}
