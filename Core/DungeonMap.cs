using RLNET;
using RogueSharp;
using System.Collections.Generic;
using System.Linq;

namespace Game.Core
{
    public class DungeonMap : Map
    {
        private readonly char walkable = '.';
        private readonly char wall = '#';

        public List<Rectangle> Rooms;
        public List<Monster> Monsters;
        public static List<GoldPile> GoldPiles;

        public DungeonMap()
        {
            Rooms = new List<Rectangle>();
            Monsters = new List<Monster>();
            GoldPiles = new List<GoldPile>();
        }

        // The draw method is called every time the map is updated
        // it will render all of the symbols/colors for each cell to the map sub console
        public void Draw(RLConsole mapConsole, RLConsole statConsole)
        {
            foreach (Cell cell in GetAllCells())
            {
                SetConsoleSymbolForCell(mapConsole, cell);
            }


            int i = 0; // Index to know which postion to draw monster stats at
            foreach (Monster monster in Monsters)
            {
                monster.Draw(mapConsole, this);
                // Only draw stats for the monsters that are in FOV
                if(IsInFov(monster.X, monster.Y))
                {
                    // Pass the index to DrawStats and increment it
                    monster.DrawStats(statConsole, i);
                    i++;
                }
            }

        }

        private void SetConsoleSymbolForCell(RLConsole console, Cell cell)
        {
            // When a cell hasn't been explored yet, don't draw it
            if (!cell.IsExplored)
            {
                return;
            }

            // When a cell is currently in the field-of-view it should be drawn with ligher colors
            if (IsInFov(cell.X, cell.Y))
            {
                // Choose the symbol to draw based on if the cell is walkable or not
                // '.' for floor and '#' for walls
                if (cell.IsWalkable)
                {
                    console.Set(cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, walkable);
                }
                else
                {
                    console.Set(cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, wall);
                }
            }
            // When a cell is outside of the field of view draw it with darker colors
            else
            {
                if (cell.IsWalkable)
                {
                    console.Set(cell.X, cell.Y, Colors.Floor, Colors.FloorBackground, walkable);
                }
                else
                {
                    console.Set(cell.X, cell.Y, Colors.Wall, Colors.WallBackground, wall);
                }
            }
        }

        // This method will be called any time we move the player to update field-of-view
        public void UpdatePlayerFieldOfView()
        {
            Player player = Game.Player;
            // Compute the field-of-view based on the player's location and awareness
            ComputeFov(player.X, player.Y, player.Awareness, true);
            // Mark all cells in field-of-view as having been explored
            foreach (Cell cell in GetAllCells())
            {
                if (IsInFov(cell.X, cell.Y))
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            }
        }



        private void UpdateActorPosition(Actor actor, int x, int y)
        {
            actor.X = x; actor.Y = y;
        }

        // Return true when able to place the actor on the cell
        public bool SetActorPostion(Actor actor, int x, int y)
        {
            // Only allow actor placement if the cell is walkable
            if(GetCell(x, y).IsWalkable)
            {
                // The cell the actor was previously on is now walkable
                SetIsWalkable(actor.X, actor.Y, true);

                UpdateActorPosition(actor, x, y);

                // The cell the actor is now on in not walkable
                SetIsWalkable(actor.X, actor.Y, false);

                if(actor is Player)
                {
                    UpdatePlayerFieldOfView();
                }
                return true;
            }
            return false;
        }

        
        public void SetIsWalkable(int x, int y, bool isWalkable)
        {
            Cell cell = (Cell)GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
        }

        public void AddPlayer(Player player)
        {
            Game.Player = player;
            SetIsWalkable(player.X, player.Y, false);
            UpdatePlayerFieldOfView();
            Game.SchedulingSystem.Add(player);
        }

        public void AddMonster(Monster monster)
        {
            Monsters.Add(monster);
            // A new monster cannot be walkable
            SetIsWalkable(monster.X, monster.Y, false);
            Game.SchedulingSystem.Add(monster);
        }

        public void RemoveMonster(Monster monster)
        {
            Monsters.Remove(monster);
            SetIsWalkable(monster.X, monster.Y, true);
            SetCellProperties(monster.X, monster.Y, true, true, true);
            Game.SchedulingSystem.Remove(monster);
            //DropGold(monster);
        }

        public Monster GetMonsterAt(int x, int y)
        {
            return Monsters.FirstOrDefault(m => m.X == x && m.Y == y);
        }

        public Point? GetRandomWalkableLocation(Rectangle room)
        {
            if (DoesRoomHasWalkableSpace(room))
            {
                for (int i = 0; i < 100; i++)
                {
                    int x = Game.Random.Next(1, room.Width - 2) + room.X;
                    int y = Game.Random.Next(1, room.Height - 2) + room.Y;
                    if(IsWalkable(x, y))
                    {
                        return new Point(x, y);
                    }
                }
            }

            return null;
        }

        private bool DoesRoomHasWalkableSpace(Rectangle room)
        {
            for(int x = 1; x <= room.Width; x++)
            {
                for(int y = 1; y <= room.Height; y++)
                {
                    if(IsWalkable(x + room.X, y +room.Y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void DropGold(Monster monster)
        {
            Cell monsterLocation = new Cell(monster.X, monster.Y, true, true, true);
            DungeonMap.GoldPiles.Add(new GoldPile(monsterLocation, monster.Gold));
            Game.MessageLog.Add($"  {monster.Name} died and dropped {monster.Gold} Gold.");
        }

        public static void SearchForGold()
        {
            for(int i = GoldPiles.Count - 1; i >= 0; i--)
            {
                if(Game.Player.X == DungeonMap.GoldPiles[i].Location.X && Game.Player.Y == DungeonMap.GoldPiles[i].Location.Y)
                {
                    AddGoldToPlayer(DungeonMap.GoldPiles[i]);
                    Game.MessageLog.Add($"{Game.Player.Name} acquired {DungeonMap.GoldPiles[i].Gold} Gold.");
                    GoldPiles.Remove(GoldPiles[i]);
                    // Do not use break here due to one cell possibly having more than one pile of gold
                }
            }
        }

        private static void AddGoldToPlayer(GoldPile goldPile)
        {
            Game.Player.Gold += goldPile.Gold;
        }

    }
}

