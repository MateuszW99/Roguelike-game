using RogueSharp;

namespace Game.Core
{
    public class Player : Actor
    {
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
        }

        public void DrawStats(RLNET.RLConsole console)
        {
            console.Print(1, 1, $"Name:     {Name}", Colors.Text);
            console.Print(1, 3, $"Health:     {Health}", Colors.Text);
            console.Print(1, 5, $"Attack:     {Attack}", Colors.Text);
            console.Print(1, 7, $"Defense:     {Defense}", Colors.Text);
            console.Print(1, 9, $"Gold:     {Gold}", Colors.Text);
        }
    }
}
