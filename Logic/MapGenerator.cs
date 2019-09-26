using Game.Core.Monsters;
using RogueSharp;
using RogueSharp.DiceNotation;
using System;
using System.Linq;

namespace Game.Core
{
    class MapGenerator
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _maxRooms;
        private readonly int _roomMaxSize;
        private readonly int _roomMinSize;

        private readonly DungeonMap _map;

        public static Player Player { get; set; }

        public MapGenerator(int width, int height, int maxRooms, int roomMaxSize, int roomMinSize)
        {
            _width = width;
            _height = height;
            _maxRooms = maxRooms;
            _roomMaxSize = roomMaxSize;
            _roomMinSize = roomMinSize;
            _map = new DungeonMap();
        }

        public DungeonMap CreateMap()
        {
            // Set the properties of all cells to false
            _map.Initialize(_width, _height);

            // Place as many rooms as possible
            for (int i = 0; i < _maxRooms; i++)
            {
                int roomWidth = Game.Random.Next(_roomMinSize, _roomMaxSize);
                int roomHeight = Game.Random.Next(_roomMinSize, _roomMaxSize);
                int roomXPosition = Game.Random.Next(0, _width - roomWidth - 1);
                int roomYPosition = Game.Random.Next(0, _height - roomHeight - 1);

                var newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);

                // Check to see if the room rectangle intersects with any other rooms
                bool newRoomIntersects = _map.Rooms.Any(room => newRoom.Intersects(room));
                // If not, add it to the room list
                if (!newRoomIntersects)
                {
                    _map.Rooms.Add(newRoom);
                }
            }

            foreach (Rectangle room in _map.Rooms)
            {
                CreateRoom(room);
            }

            // Create corridors connecting the rooms
            for (int i = 1; i < _map.Rooms.Count; i++)
            {
                // For all remaining previous rooms get the center of the room and the previous room
                int previousRoomCenterX = _map.Rooms[i - 1].Center.X;
                int previousRoomCenterY = _map.Rooms[i - 1].Center.Y;
                int currentRoomCenterX = _map.Rooms[i].Center.X;
                int currentRoomCenterY = _map.Rooms[i].Center.Y;
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
            PlacePlayer();
            PlaceMonsters();
            return _map;
        }

        private void CreateRoom(Rectangle room)
        {
            for(int x = room.Left + 1; x < room.Right; x++)
            {
                for(int y = room.Top + 1; y < room.Bottom; y++)
                {
                    // The last flag is set to false in order not to show not explored rooms
                    _map.SetCellProperties(x, y, true, true, false);
                }
            }
        }

        public void PlacePlayer()
        {
            Player player = global::Game.Game.Player;
            if (player == null)
            {
                player = new Player(_map.Rooms[0].Center);
            }
            _map.AddPlayer(player);
        }

        private void PlaceMonsters()
        {
            // Don't spawn monster in the starting room
            for(int i = 1; i < _map.Rooms.Count; i++) 
            {
                // Each room has a60& chance to containg monsters
                if(Dice.Roll("1D10") < 7)
                {
                    // Spawn between 1 and 4 monsters
                    var numberOfMonsters = Dice.Roll("1D4");
                    for(int j = 0; j < numberOfMonsters; j++)
                    {
                        // Find space to place a monster
                        Point randomLocation = (Point)_map.GetRandomWalkableLocation(_map.Rooms[i]);

                        if(randomLocation != null)
                        {
                            // Hard code the monster to be created at level 1
                            var monster = Kobold.Create(1);
                            monster.X = randomLocation.X;
                            monster.Y = randomLocation.Y;
                            _map.AddMonster(monster);
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
                _map.SetCellProperties(x, yPosition, true, true);
            }
        }

        // Draw a tunnel out of the map parallel to the y axis
        private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
        {
            for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                _map.SetCellProperties(xPosition, y, true, true);
            }
        }
    }
}
