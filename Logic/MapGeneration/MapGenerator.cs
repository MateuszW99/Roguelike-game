using RogueSharp;
using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core;
using RLNET;
using Game.Logic.MapGeneration.Objects;
using Game.Core.Entities.Monsters;

namespace Game.Logic.MapGeneration
{
    class MapGenerator
    {
        private readonly int width;
        private readonly int height;
        private readonly int maxRooms;
        private readonly int roomMaxSize;
        private readonly int roomMinSize;

        private readonly DungeonMap map;

        public static Player Player { get; set; }



        public MapGenerator(int width, int height, int maxRooms, int roomMaxSize, int roomMinSize, int mapLevel)
        {
            this.width = width;
            this.height = height;
            this.maxRooms = maxRooms;
            this.roomMaxSize = roomMaxSize;
            this.roomMinSize = roomMinSize;
            map = new DungeonMap();
        }

        public DungeonMap CreateMap(RLConsole mapConsole)
        {
            map.Initialize(width, height);

            // Place as many rooms as possible
            for (int i = 0; i < maxRooms; i++)
            {
                int roomWidth = Game.Random.Next(roomMinSize, roomMaxSize);
                int roomHeight = Game.Random.Next(roomMinSize, roomMaxSize);
                int roomXPosition = Game.Random.Next(0, width - roomWidth - 1);
                int roomYPosition = Game.Random.Next(0, height - roomHeight - 1);

                var newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);

                // Check to see if the room rectangle intersects with any other rooms
                bool newRoomIntersects = DungeonMap.Rooms.Any(room => newRoom.Intersects(room));
                if (!newRoomIntersects)
                {
                    DungeonMap.Rooms.Add(newRoom);
                }
            }

            foreach (Rectangle room in DungeonMap.Rooms)
            {
                CreateRoom(room);
            }

            // Create corridors connecting the rooms
            for (int i = 1; i < DungeonMap.Rooms.Count; i++)
            {
                // For all remaining previous rooms get the center of the room and the previous room
                int previousRoomCenterX = DungeonMap.Rooms[i - 1].Center.X;
                int previousRoomCenterY = DungeonMap.Rooms[i - 1].Center.Y;
                int currentRoomCenterX = DungeonMap.Rooms[i].Center.X;
                int currentRoomCenterY = DungeonMap.Rooms[i].Center.Y;
                // Random chance to draw a 'L' or 'Г' shaped corridor 
                if (Game.Random.Next(1, 2) == 1)
                {
                    CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, previousRoomCenterY);
                    CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, currentRoomCenterX);
                }
                else
                {
                    CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, previousRoomCenterX);
                    CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, currentRoomCenterY);
                }
            }

            foreach (Rectangle room in DungeonMap.Rooms)
            {
                CreateDoors(room);
            }

            CreateStairs();

            foreach(Rectangle room in DungeonMap.Rooms)
            {
                if(Game.Random.Next(1, 2) == 1)
                {
                    CreateObstacles(mapConsole, room);
                }
            }

            PlacePlayer();
            PlaceMonsters();

            return map;
        }

        private void CreateRoom(Rectangle room)
        {
            for (int x = room.Left + 1; x < room.Right; x++)
            {
                for (int y = room.Top + 1; y < room.Bottom; y++)
                {
                    // The last flag is set to false in order not to show unexplored rooms
                    map.SetCellProperties(x, y, true, true, false);
                }
            }
        }

        public void PlacePlayer()
        {
            Player player = Game.Player;
            if (player == null)
            {
                player = new Player(DungeonMap.Rooms[0].Center);
            }
            if(!map.GetCell(player.X, player.Y).IsWalkable)
            {
                map.SetActorPostion(player, DungeonMap.Rooms[0].Center.X, DungeonMap.Rooms[0].Center.Y);
            }
            map.AddPlayer(player);
        }

        private void PlaceMonsters()
        {
            // Don't spawn monster in the starting room
            for (int i = 1; i < DungeonMap.Rooms.Count; i++)
            {
                // Each room has 60% chance to containg monsters
                if (Dice.Roll("1D10") < 7)
                {
                    // Spawn between 1 and 4 monsters
                    var numberOfMonsters = Dice.Roll("1D4");
                    for (int j = 0; j < numberOfMonsters; j++)
                    {
                        // Find space to place a monster
                        Point randomLocation = (Point)map.GetRandomWalkableLocation(DungeonMap.Rooms[i]);
                        if(randomLocation == DungeonMap.Rooms[i].Center)
                        {
                            return;
                        }
                        if (randomLocation != null)
                        {
                            // Hard coded monster to be created at level 1
                            
                            var monster = Monster.RandomSpawn(randomLocation);            
                            map.AddMonster(monster);
                        }
                    }
                }
            }
        }

        // Draw a tunnel out of the map parallel to the x axis
        private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
        {
            for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
            {
                map.SetCellProperties(x, yPosition, true, true);
            }
        }

        // Draw a tunnel out of the map parallel to the y axis
        private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
        {
            for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                map.SetCellProperties(xPosition, y, true, true);
            }
        }

        private void CreateObstacles(RLNET.RLConsole console, Rectangle room)
        {
            int numberOfObstacles = Game.Random.Next(1, 6);
            for (int i = 0; i < numberOfObstacles; i++)
            {
                int xPosition = Game.Random.Next(room.Left + 2, room.Right - 2);
                int yPosition = Game.Random.Next(room.Top + 2, room.Bottom - 2);
                if (map.IsWalkable(xPosition, yPosition) && !(room.Center.X == xPosition && room.Center.Y == yPosition))
                {
                    map.SetCellProperties(xPosition, yPosition, false, false, false);
                    DungeonMap.Columns.Add(new Column(xPosition, yPosition));
                    map.SetIsWalkable(xPosition, yPosition, false);
                }
                else
                {
                    i--;
                }

            }
        }

        private void CreateDoors(Rectangle room)
        {
            List<Cell> borderCells = new List<Cell>();
            CollectCells(room, borderCells);

            foreach (Cell cell in borderCells)
            {
                if (IsPotentialDoor(cell))
                {
                    // A door must block FOV when it's closed
                    map.SetCellProperties(cell.X, cell.Y, false, true);
                    map.Doors.Add(new Logic.MapGeneration.Door
                    {
                        X = cell.X,
                        Y = cell.Y,
                        IsOpen = false
                    });
                    // The wall has been changed to have doors so quit the method
                    return;
                }
            }
        }

        // Check if a cell is appropriate to become a door
        // To check it, the neighbouring cells of the cell must be checked
        private bool IsPotentialDoor(Cell cell)
        {
            // Door must be walkable
            if (!cell.IsWalkable)
            {
                return false;
            }
            try
            {
                Cell right = (Cell)map.GetCell(cell.X + 1, cell.Y);
                Cell left = (Cell)map.GetCell(cell.X - 1, cell.Y);
                Cell top = (Cell)map.GetCell(cell.X, cell.Y - 1);
                Cell bottom = (Cell)map.GetCell(cell.X, cell.Y + 1);
                
                // Make sure that none of the references is already a door
                if (map.GetDoor(cell.X, cell.Y) != null ||
                    map.GetDoor(cell.X + 1, cell.Y) != null ||
                    map.GetDoor(cell.X - 1, cell.Y) != null ||
                    map.GetDoor(cell.X, cell.Y - 1) != null ||
                    map.GetDoor(cell.X, cell.Y + 1) != null)
                {
                    return false;
                }

                // Try to place a door on the left or right side of the room
                if (right.IsWalkable && left.IsWalkable && !top.IsWalkable && !bottom.IsWalkable)
                {
                    return true;
                }
                // Try to place a door on the top or bottom side of the room
                if (!right.IsWalkable && !left.IsWalkable && top.IsWalkable && bottom.IsWalkable)
                {
                    return true;
                }

                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        void CollectCells(Rectangle room, List<Cell> cellList)
        {
            int xMin = room.Left;
            int xMax = room.Right;
            int yMin = room.Top;
            int yMax = room.Bottom;

            // Add cells from bottom & top row
            for (int x = xMin; x <= xMax; x++)
            {
                cellList.Add(new Cell(x, yMin, true, true, true));
                cellList.Add(new Cell(x, yMax, true, true, true));
            }
            // Add cells from left & right row
            for (int y = yMin + 1; y <= yMax - 1; y++)
            {
                cellList.Add(new Cell(xMin, y, true, true, true));
                cellList.Add(new Cell(xMax, y, true, true, true));
            }
        }

        private void CreateStairs()
        {
            map.StairsUp = new Stairs
            {
                X = DungeonMap.Rooms.First().Center.X + 1,
                Y = DungeonMap.Rooms.First().Center.Y + 1,
                IsUp = true
            };
            map.StairsDown = new Stairs
            {
                X = DungeonMap.Rooms.Last().Center.X,
                Y = DungeonMap.Rooms.Last().Center.Y,
                IsUp = false
            };
        }
       }
    }

