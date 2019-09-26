using Game.Core;
using Game.Interfaces;
using RLNET;
using RogueSharp;

namespace Game.Logic.MapGeneration
{
    public class Door : IDrawable
    {
        private char closed = '|';
        private char opened = '/';

        public Door()
        {
            Symbol = closed;
            Color = Colors.Door;
            BackgroundColor = Colors.DoorBackground;
        }

        public bool IsOpen { get; set; }

        public RLColor Color { get; set; }
        public RLColor BackgroundColor { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(X, Y).IsExplored)
            {
                return;
            }

            Symbol = IsOpen ? opened : closed;
            if (map.IsInFov(X, Y))
            {
                Color = Colors.DoorFov;
                BackgroundColor = Colors.DoorBackgroundFov;
            }
            else
            {
                Color = Colors.Door;
                BackgroundColor = Colors.DoorBackground;
            }
            console.Set(X, Y, Color, BackgroundColor, Symbol);
        }



    }
}
