using Game.Interfaces;
using Game.Logic.MapGeneration;
using RLNET;
using RogueSharp;

namespace Game.Core.Items
{
    public class Item : IItem, IDrawable
    {
        // IItem
        public string Name { get; set; }
        public char Symbol { get; set; }

        virtual public void Use(Player player, int? itemNumber)
        { }

        virtual public void Add(Player player)
        { }

        // IDrawable
        public RLColor Color { get; set; }
        public RLColor ColorBackground { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Item(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Draw(RLConsole console, IMap map)
        {
            if(!map.GetCell(X, Y).IsExplored)
            {
                return;
            }

            if (map.IsInFov(X, Y))
            {
                Color = Colors.FloorFov;
                ColorBackground = Colors.FloorBackground;
            }
            else
            {
                Color = Colors.Floor;
                ColorBackground = Colors.FloorBackground;
            }
            console.Set(X, Y, Color, ColorBackground, Symbol);
        }

        public virtual void DropItem(Monster monster)
        { }

        public static void SearchForItems()
        {
            for(int i = DungeonMap.Items.Count - 1; i >= 0; i--)
            {
                if(DungeonMap.Items[i].X == Game.Player.X && DungeonMap.Items[i].Y == Game.Player.Y)
                {
                    Game.MessageLog.Add("x");
                    DungeonMap.Items[i].Use(Game.Player, null);
                    Game.MessageLog.Add($"You picked the {DungeonMap.Items[i].Name}.");
                    DungeonMap.Items.RemoveAt(i);
                }
            }
        }
    }
}
