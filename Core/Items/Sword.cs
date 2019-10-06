using Game.Logic.MapGeneration;
using RogueSharp;

namespace Game.Core.Items
{
    public class Sword : ItemPassive
    {
        public Sword(int x, int y) : base(x, y)
        {
            Symbol = 'S';
            Name = "Sword";
            Stats = 1;
        }
    }
}
