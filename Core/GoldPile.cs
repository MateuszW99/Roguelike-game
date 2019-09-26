using RogueSharp;

namespace Game.Core
{
    public class GoldPile
    {
        public int Gold { get; set; }
        public Cell Location { get; set; }

        public GoldPile(Cell cell, int goldAmount)
        {
            Location = cell;
            Gold = goldAmount;
        }
    }
}
