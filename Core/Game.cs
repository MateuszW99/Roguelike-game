using System;
using RLNET;
using Game.Core;
using Game.Logic;
using RogueSharp.Random;
using Game.Logic.MapGeneration;
using Game.Core.Items;

namespace Game
{
    class Game
    {
        public static IRandom Random { get; private set; }

        public static Player Player { get; set; }

        public static DungeonMap DungeonMap { get; private set; }

        public static CommandSystem CommandSystem { get; private set; }

        public static MessageLog MessageLog { get; private set; }

        //public static PlayerInventory  PlayerInventory { get; set; }

        public static ActionScheduling SchedulingSystem { get; private set; }

        public static int mapLevel = 1;

        public static bool renderRequired = true;

        public static bool runGame = true;

        // The screen height and width are in number of tiles
        private static readonly int _screenWidth = 100;
        private static readonly int _screenHeight = 70;
        private static RLRootConsole rootConsole;

        // The map console takes up most of the screen and is where the map will be drawn
        private static readonly int _mapWidth = 80;
        private static readonly int _mapHeight = 48;
        private static RLConsole mapConsole;

        // Below the map console is the message console which displays attack rolls and other information
        private static readonly int _messageWidth = 80;
        private static readonly int _messageHeight = 11;
        private static RLConsole Messages;

        // The stat console is to the right of the map and display player and monster stats
        private static readonly int _statWidth = 20;
        private static readonly int _statHeight = 70;
        private static RLConsole Stats;

        // Above the map is the inventory console which shows the players equipment, abilities, and items
        private static readonly int _inventoryWidth = 80;
        private static readonly int _inventoryHeight = 11;
        private static RLConsole QuickBar;

        public static void Main()
        {
            int seed = (int)DateTime.UtcNow.Ticks;
            Random = new DotNetRandom(seed);

            string fontFileName = "terminal8x8.png";
            string consoleTitle = $"Game tutorial - Level {mapLevel} - Seed {seed}";

            rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle);
            mapConsole = new RLConsole(_mapWidth, _mapHeight);
            Messages = new RLConsole(_messageWidth, _messageHeight);
            Stats = new RLConsole(_statWidth, _statHeight);
            QuickBar = new RLConsole(_inventoryWidth, _inventoryHeight);

            SchedulingSystem = new ActionScheduling();

            MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7, mapLevel);
            DungeonMap = mapGenerator.CreateMap(mapConsole);
            DungeonMap.UpdatePlayerFieldOfView();

            CommandSystem = new CommandSystem();

            // Create a new MessageLog and print the random seed used to generate the level
            MessageLog = new MessageLog();
            MessageLog.Add("The rogue arrives on level 1");
            MessageLog.Add($"Level created with seed '{seed}'");

            // PlayerInventory keeps player's equipment and prints its content onto the QuickBar
            //PlayerInventory = new PlayerInventory();

            PlaceConsoles();

            rootConsole.Render += OnRootConsoleUpdate;
            rootConsole.Render += OnRootConsoleRender;


            rootConsole.Run();
        }


        private static void PlaceConsoles()
        {
            // Set backround colors and text for each console
            mapConsole.SetBackColor(0, 0, _mapWidth, _mapHeight, Colors.FloorBackground);
            mapConsole.Print(1, 1, "", Colors.TextHeading);

            //QuickBar.SetBackColor(0, 0, _inventoryWidth, _inventoryHeight, Palette.DbWood);
            //QuickBar.Print(1, 1, "Inventory", Colors.TextHeading);
        }

        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            bool didPlayerAct = false;
            RLKeyPress keyPress = rootConsole.Keyboard.GetKeyPress();
            
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
                    rootConsole.Close();
                }
                else if(keyPress.Key == RLKey.Period)
                {
                    if(DungeonMap.CanMoveDownToNextLevel())
                    {
                        MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7, ++mapLevel);
                        DungeonMap = mapGenerator.CreateMap(mapConsole);
                        MessageLog = new MessageLog();                    
                        CommandSystem = new CommandSystem();
                        rootConsole.Title = $"Game - Level {mapLevel}";
                        MessageLog.Add($"You reached {mapLevel} level!");
                        didPlayerAct = true;
                    }
                }
                else if (keyPress.Key == RLKey.P) // shortcut to get more scrolls for testing
                {
                    int? x = null;
                    int? y = null;
                    PlayerInventory.AddToQuickBar(new ScrollOfDestruction(x, y));
                }
                else if (keyPress.Key == RLKey.O) // shortcut to get more scrolls for testing
                {
                    int? x = null;
                    int? y = null;
                    PlayerInventory.AddToQuickBar(new ScrollOfTeleport(x, y));
                }
                else if(keyPress.Key == RLKey.Number1)
                {
                    didPlayerAct = CommandSystem.UseItem(Quickbar.ScrollTeleport);
                }
                else if(keyPress.Key == RLKey.Number2)
                {
                    didPlayerAct = CommandSystem.UseItem(Quickbar.ScrollDestruction);
                }
            }

            if(didPlayerAct)
            {
                renderRequired = true;
                CommandSystem.EndPlayerTurn();
            }
            else
            {
                CommandSystem.ActivateMonsters();
                renderRequired = true;
            }
        }

        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            if (renderRequired)
            {
                ClearConsoles();

                DrawConsoles();

                // Blit the sub consoles to the root console in the correct locations
                RLConsole.Blit(mapConsole, 0, 0, _mapWidth, _mapHeight,
                 rootConsole, 0, _inventoryHeight);
                RLConsole.Blit(Stats, 0, 0, _statWidth, _statHeight,
                  rootConsole, _mapWidth, 0);
                RLConsole.Blit(Messages, 0, 0, _messageWidth, _messageHeight,
                  rootConsole, 0, _screenHeight - _messageHeight);
                RLConsole.Blit(QuickBar, 0, 0, _inventoryWidth, _inventoryHeight,
                  rootConsole, 0, 0);

                rootConsole.Draw();
                
                renderRequired = false;
            }
        }

        private static void ClearConsoles()
        {
            mapConsole.Clear();
            Stats.Clear();
            Messages.Clear();
            QuickBar.Clear();
        }

        private static void DrawConsoles()
        {
            DungeonMap.Draw(mapConsole, Stats);
            Player.Draw(mapConsole, DungeonMap);
            Player.DrawStats(Stats, 1);
            MessageLog.Draw(Messages);
            Player.Inventory.Draw(QuickBar);
        }
    }
}
