using Game.Core;
using Game.Interfaces;
using RLNET;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Logic.MapGeneration.Objects
{
    public class Column : IDrawable
    {
        public RLColor Color { get; set; }
        public RLColor ColorBackground { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Column(int x, int y)
        {
            Symbol = 'o';
            X = x;
            Y = y;
        }

        public void Draw(RLConsole console, IMap map)
        {
            if(!map.GetCell(X, Y).IsExplored)
            {
                return;
            }

            if(map.IsInFov(X, Y))
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

        
    }
}
