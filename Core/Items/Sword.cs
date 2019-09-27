using Game.Logic.MapGeneration;
using RogueSharp;

namespace Game.Core.Items
{
    public class Sword : ItemPassive
    {
        public Sword(int x, int y) : base(x, y)
        {
            Symbol = 'X';
            Name = "Sword";
            Stats = 1;
        }

        public override void Use(Player player, int? itemNumber)
        {
            player.Attack += Stats;
        }

        public override void DropItem(Monster monster)
        {
            //Cell monsterLocation = new Cell(monster.X, monster.Y, true, true, true);
            DungeonMap.Items.Add(this);
            Game.MessageLog.Add($"  {monster.Name} died and dropped an item.");
        }

    }
}
