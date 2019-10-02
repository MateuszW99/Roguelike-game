using RLNET;
using RogueSharp;
using System.Collections.Generic;
using System.Linq;
using Game.Core;
using Game.Logic.MapGeneration.Objects;
using Game.Core.Items;

namespace Game.Logic.MapGeneration
{
    public class DungeonMap : Map
    {
        private readonly char walkable = '.';
        private readonly char wall = '#';
        //private readonly char column = 'o';

        public static List<Rectangle> Rooms { get; set; }
        public List<Monster> Monsters;
        public static List<GoldPile> GoldPiles;
        public static List<Column> Columns;
        public static List<Item> Items;
        public List<Door> Doors { get; set; }
        public Stairs StairsUp { get; set; }
        public Stairs StairsDown { get; set; }


        public DungeonMap()
        {
            // When creating a new level, clear all the objects from the floor below
            Game.SchedulingSystem.Clear();

            Rooms = new List<Rectangle>();
            Monsters = new List<Monster>();
            GoldPiles = new List<GoldPile>();
            Columns = new List<Column>();
            Doors = new List<Door>();
            Items = new List<Item>();
        }

        // The draw method is called every time the map is updated
        // it will render all of the symbols/colors for each cell to the map console
        public void Draw(RLConsole mapConsole, RLConsole statConsole)
        {
            foreach (Cell cell in GetAllCells())
            {
                SetConsoleSymbolForCell(mapConsole, cell);
            }

            foreach (Door door in Doors)
            {
                door.Draw(mapConsole, this);
            }

            foreach(Column column in Columns)
            {
                column.Draw(mapConsole, this);
            }

            StairsDown.Draw(mapConsole, this);
            StairsUp.Draw(mapConsole, this);

            int i = 0; // Index to know which postion to draw monster stats at
            foreach (Monster monster in Monsters)
            {
                monster.Draw(mapConsole, this);
                // Only draw stats for the monsters that are in FOV
                if(IsInFov(monster.X, monster.Y))
                {
                    monster.DrawStats(statConsole, i);
                    i++;
                }
            }

            foreach(GoldPile gold in GoldPiles)
            {
                gold.Draw(mapConsole, this);
            }

            foreach(Item item in Items)
            {
                item.Draw(mapConsole, this);
            }
        }

        private void SetConsoleSymbolForCell(RLConsole console, Cell cell)
        {
            // When a cell hasn't been explored yet, don't draw it
            if (!cell.IsExplored)
            {
                return;
            }

            // When a cell is currently in the FOV it should be drawn with ligher colors
            if (IsInFov(cell.X, cell.Y))
            {
                if (cell.IsWalkable)
                {
                    console.Set(cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, walkable);
                }
                else
                {
                    console.Set(cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, wall);
                }
            }
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

        // This method will be called any time the players moves to update field-of-view
        public void UpdatePlayerFieldOfView()
        {
            Player player = Game.Player;
            // Compute the FOV based on the player's location and awareness
            ComputeFov(player.X, player.Y, player.Awareness, true);
            // Mark all cells in FOV as having been explored
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
            if(GetCell(x, y).IsWalkable)
            {
                SetIsWalkable(actor.X, actor.Y, true);
                UpdateActorPosition(actor, x, y);
                SetIsWalkable(actor.X, actor.Y, false);
                OpenDoor(actor, x, y);

                if(actor is Player)
                {
                    UpdatePlayerFieldOfView();
                    Item.SearchForItems();
                    GoldPile.SearchForGold();
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
            if(DoesRoomHasWalkableSpace(room))
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


        // Return the door at (x, y) or null if there is none
        public Door GetDoor(int x, int y)
        {
            return Doors.SingleOrDefault(d => d.X == x && d.Y == y);
        }

        private void OpenDoor(Actor actor, int x, int y)
        {
            Door door = GetDoor(x, y);
            if (door != null && !door.IsOpen)
            {
                door.IsOpen = true;
                var cell = GetCell(x, y);
                // Door won't block FOV when opened
                SetCellProperties(x, y, true, cell.IsWalkable, cell.IsExplored);
                Game.MessageLog.Add($"{actor.Name} opened a door");
            }
        }

        public bool CanMoveDownToNextLevel()
        {
            Player player = Game.Player;
            return StairsDown.X == player.X && StairsDown.Y == player.Y;
        }

        private static bool IsColumn(int x, int y)
        {
            foreach (Column column in DungeonMap.Columns)
            {
                if(column.X == x && column.Y == y)
                return true;
            }
            return false;
        }

        private void SetIsExplored(int x, int y, bool IsExplored)
        {
            Cell cell = (Cell)GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, IsExplored);
        }

        public static bool CanTeleport(int x, int y)
        {
            if(DungeonMap.IsColumn(x, y))
            {
                return false;
            }
            // TODO: check for stairs
            return true;
        }



    }
}


