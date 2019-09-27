using Game.Interfaces;
using Game.Logic.MapGeneration;
using RLNET;
using RogueSharp;

namespace Game.Core
{
    public class GoldPile :IDrawable
    {
        public int Gold { get; set; }
        public Cell Location { get; set; }
        public RLColor Color { get; set; }
        public RLColor ColorBackground { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public GoldPile(int x, int y, int goldAmount)
        {
            X = x;
            Y = y;
            Gold = goldAmount;
            Color = Colors.Gold;
        }

        public void Draw(RLConsole console, IMap map)
        {
            if(!map.IsExplored(X, Y))
            {
                return;
            }

            if(map.IsInFov(X, Y))
            {
                console.Set(X, Y, Color, Colors.FloorBackgroundFov, 'G');
            }
            else
            {
                console.SetColor(X, Y, Colors.Floor);
                console.SetBackColor(X, Y, Colors.FloorBackground);
            }

            
        }

        public static void AddGoldToPlayer(int gold)
        {
            Game.Player.Gold += gold;
        }

        public static void DropGold(Monster monster)
        {
            Cell monsterLocation = new Cell(monster.X, monster.Y, true, true, true);
            DungeonMap.GoldPiles.Add(new GoldPile(monsterLocation.X, monsterLocation.Y, monster.Gold));
            Game.MessageLog.Add($"  {monster.Name} died and dropped {monster.Gold} Gold.");
        }

        public static void SearchForGold()
        {
            for (int i = DungeonMap.GoldPiles.Count - 1; i >= 0; i--)
            {
                if (Game.Player.X == DungeonMap.GoldPiles[i].X && Game.Player.Y == DungeonMap.GoldPiles[i].Y)
                {
                    GoldPile.AddGoldToPlayer(DungeonMap.GoldPiles[i].Gold);
                    Game.MessageLog.Add($"{Game.Player.Name} acquired {DungeonMap.GoldPiles[i].Gold} Gold.");
                    DungeonMap.GoldPiles.Remove(DungeonMap.GoldPiles[i]);
                    // Do not use break here due to one cell possibly having more than one pile of gold
                }
            }
        }
    }
}
