using System;
using Game.Core;
using Game.Interfaces;
using RLNET;
using RogueSharp;

namespace Game.Logic.MapGeneration
{
    public class Stairs : IDrawable
    {
        private char Up = '<';
        private char Down = '>';

        public RLColor Color { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsUp { get; set; }

        public void Draw(RLConsole console, IMap map)
        {
            if(!map.GetCell(X, Y).IsExplored)
            {
                return;
            }
            Symbol = IsUp ? Up : Down;
            if(map.IsInFov(X, Y))
            {
                Color = Colors.Player;
            }
            else
            {
                Color = Colors.Floor;
            }

            console.Set(X, Y, Color, null, Symbol);
        }

        public bool IsStairs(int x, int y)
        {
            if(x == this.X && y == this.Y)
            {
                return true;
            }
            return false;
        }
    }
}
