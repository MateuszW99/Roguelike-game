using Game.Logic.MapGeneration;
using RogueSharp;

namespace Game.Core.Items
{
    public class Chestplate : ItemPassive
    {
        public Chestplate(int x, int y) : base(x, y)
        {
            Symbol = 'A';
            Name = "Chestplate";
            Stats = 1;
        }
    }
}
