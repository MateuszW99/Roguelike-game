using Game.Logic.MapGeneration;
using RogueSharp;

namespace Game.Core.Items
{
    public class Armor : ItemPassive
    {
        public Armor(int x, int y) : base(x, y)
        {
            Symbol = 'A';
            Name = "Armor";
            Stats = 1;
        }
    }
}
