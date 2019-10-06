using Game.Core.Items;
using RogueSharp;
using System.Collections.Generic;

namespace Game.Core
{
    public class Player : Actor
    {
        public static PlayerInventory Inventory;


        public Player()
        {
            StarterStats();
        }
        public Player(int x, int y)
        {
            StarterStats();
            X = x;
            Y = y;
        }
        public Player(Point pp)
        {
            StarterStats();
            X = pp.X;
            Y = pp.Y;
        }

        private void StarterStats()
        {
            Attack = 2;
            AttackChance = 50;
            Awareness = 15;
            Color = Colors.Player;
            Defense = 2;
            DefenseChance = 40;
            Gold = 0;
            Health = 100;
            MaxHealth = 100;
            Name = "Rogue";
            Speed = 10;
            Symbol = '@';

            Inventory = new PlayerInventory();
        }

        public void DrawStats(RLNET.RLConsole console, int yPosition)
        {
            console.Print(1, yPosition, $"Name:     {Name}", Colors.Text);
            console.Print(1, ++yPosition, $"Health:     {Health}", Colors.Text);
            AddHealthBar(console, 10, yPosition);
            console.Print(1, ++yPosition, $"Attack:     {Attack}", Colors.Text);
            console.Print(1, ++yPosition, $"Defense:     {Defense}", Colors.Text);
            console.Print(1, ++yPosition, $"Gold:     {Gold}", Colors.Text);
        }
    }
}
