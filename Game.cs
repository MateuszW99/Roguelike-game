using System;
using RLNET;
using Game.Core;
using Game.Logic;
using RogueSharp.Random;

namespace Game
{
    class Game
    {
        public static IRandom Random { get; private set; }

        public static Player Player { get; set; }

        public static DungeonMap DungeonMap { get; private set; }

        public static CommandSystem CommandSystem { get; private set; }

        public static MessageLog MessageLog { get; private set; }

        public static ActionScheduling SchedulingSystem { get; private set; }


        public static bool _renderRequired = true;

        // The screen height and width are in number of tiles
        private static readonly int _screenWidth = 100;
        private static readonly int _screenHeight = 70;
        private static RLRootConsole _rootConsole;

        // The map console takes up most of the screen and is where the map will be drawn
        private static readonly int _mapWidth = 80;
        private static readonly int _mapHeight = 48;
        private static RLConsole _mapConsole;

        // Below the map console is the message console which displays attack rolls and other information
        private static readonly int _messageWidth = 80;
        private static readonly int _messageHeight = 11;
        private static RLConsole _messageConsole;

        // The stat console is to the right of the map and display player and monster stats
        private static readonly int _statWidth = 20;
        private static readonly int _statHeight = 70;
        private static RLConsole _statConsole;

        // Above the map is the inventory console which shows the players equipment, abilities, and items
        private static readonly int _inventoryWidth = 80;
        private static readonly int _inventoryHeight = 11;
        private static RLConsole _inventoryConsole;

        public static void Main()
        {
            int seed = (int)DateTime.UtcNow.Ticks;
            Random = new DotNetRandom(seed);

            string fontFileName = "terminal8x8.png";
            string consoleTitle = $"Game tutorial - Level 1 - Seed {seed}";

            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle);  
            _mapConsole = new RLConsole(_mapWidth, _mapHeight);
            _messageConsole = new RLConsole(_messageWidth, _messageHeight);
            _statConsole = new RLConsole(_statWidth, _statHeight);
            _inventoryConsole = new RLConsole(_inventoryWidth, _inventoryHeight);

            SchedulingSystem = new ActionScheduling();
            MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7);
            DungeonMap = mapGenerator.CreateMap();
           //Player = new Player(DungeonMap.Rooms[0].Center);//
            DungeonMap.UpdatePlayerFieldOfView();

            CommandSystem = new CommandSystem();

            

            // Create a new MessageLog and print the random seed used to generate the level
            MessageLog = new MessageLog();
            MessageLog.Add("The rogue arrives on level 1");
            MessageLog.Add($"Level created with seed '{seed}'");

            PlaceConsoles();

            _rootConsole.Render += OnRootConsoleUpdate;
            _rootConsole.Render += OnRootConsoleRender;

            _rootConsole.Run();
        }


        private static void PlaceConsoles()
        {
            // Set backround colors and text for each console
            _mapConsole.SetBackColor(0, 0, _mapWidth, _mapHeight, Colors.FloorBackground);
            _mapConsole.Print(1, 1, "", Colors.TextHeading);

            _inventoryConsole.SetBackColor(0, 0, _inventoryWidth, _inventoryHeight, Palette.DbWood);
            _inventoryConsole.Print(1, 1, "Inventory", Colors.TextHeading);
        }

        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            bool didPlayerAct = false;
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
            
            if(keyPress != null)
            {
                if (keyPress.Key == RLKey.Up)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Up);
                }
                else if (keyPress.Key == RLKey.Down)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Down);
                }
                else if (keyPress.Key == RLKey.Left)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Left);
                }
                else if (keyPress.Key == RLKey.Right)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Right);
                }
                else if(keyPress.Key == RLKey.Escape)
                {
                    _rootConsole.Close();
                }
            }

            if(didPlayerAct)
            {
                _renderRequired = true;
                CommandSystem.EndPlayerTurn();
            }
            else
            {
                CommandSystem.ActivateMonsters();
                _renderRequired = true;
            }
        }



        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            if (_renderRequired)
            {
                ClearConsoles();

                DrawConsoles();

                // Blit the sub consoles to the root console in the correct locations
                RLConsole.Blit(_mapConsole, 0, 0, _mapWidth, _mapHeight,
                 _rootConsole, 0, _inventoryHeight);
                RLConsole.Blit(_statConsole, 0, 0, _statWidth, _statHeight,
                  _rootConsole, _mapWidth, 0);
                RLConsole.Blit(_messageConsole, 0, 0, _messageWidth, _messageHeight,
                  _rootConsole, 0, _screenHeight - _messageHeight);
                RLConsole.Blit(_inventoryConsole, 0, 0, _inventoryWidth, _inventoryHeight,
                  _rootConsole, 0, 0);

                _rootConsole.Draw();

                _renderRequired = false;
            }
        }

        private static void ClearConsoles()
        {
            _mapConsole.Clear();
            _statConsole.Clear();
            _messageConsole.Clear();
        }

        private static void DrawConsoles()
        {
            DungeonMap.Draw(_mapConsole, _statConsole);
            Player.Draw(_mapConsole, DungeonMap);
            Player.DrawStats(_statConsole);
            MessageLog.Draw(_messageConsole);
        }

    }
}
