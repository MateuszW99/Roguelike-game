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

        virtual public void Use()
        { }

        virtual public void Add()
        { }

        // IDrawable
        public RLColor Color { get; set; }
        public RLColor ColorBackground { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Item(int? x, int? y)
        {
            if(x == null || y == null)
            {
                return;
            }
            X = (int)x;
            Y = (int)y;
        }

        public void Draw(RLConsole console, IMap map)
        {
            if(!map.GetCell(X, Y).IsExplored)
            {
                return;
            }

            if (map.IsInFov(X, Y))
            {
                if(this is ItemPassive)
                {
                    Color = Colors.FloorFov;
                    
                }
                else if(this is ItemActive)
                {
                    if(this is HealthPotion)
                    {
                        Color = Colors.Potion;
                    }
                    else
                    {
                        Color = Colors.Scroll;
                    }
                }
                ColorBackground = Colors.FloorBackground;
                console.Set(X, Y, Color, ColorBackground, Symbol);
            }
            else
            {
                Color = Colors.Floor;
                ColorBackground = Colors.FloorBackground;
                console.SetColor(X, Y, Color);
                console.SetBackColor(X, Y, ColorBackground);
            }
            
        }

        public virtual void DropItem(Monster monster)
        {
            DungeonMap.Items.Add(this);
            Game.MessageLog.Add($"  {monster.Name} died and dropped {this.Name}.");
        }

        public static void SearchForItems()
        {
            for(int i = DungeonMap.Items.Count - 1; i >= 0; i--)
            {
                if (DungeonMap.Items[i].X == Game.Player.X && DungeonMap.Items[i].Y == Game.Player.Y)
                {
                    if (DungeonMap.Items[i] is ItemPassive)
                    {
                        DungeonMap.Items[i].Use();
                    }
                    else if(DungeonMap.Items[i] is ItemActive)
                    {
                        PlayerInventory.AddToQuickBar((ItemActive)DungeonMap.Items[i]);
                    }
                    Game.MessageLog.Add($"You picked the {DungeonMap.Items[i].Name}.");
                    DungeonMap.Items.RemoveAt(i);
                }
            }
        }
    }
}
