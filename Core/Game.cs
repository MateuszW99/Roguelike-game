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
        public static DungeonMap DungeonMap { get; set; }
        public static CommandSystem CommandSystem { get; set; }
        public static MessageLog MessageLog { get; set; }
        public static ActionScheduling SchedulingSystem { get; set; }

        public static int mapLevel = 1;

        public static bool renderRequired = true;

        public static bool runGame = true;

        // The screen height and width are in number of tiles
        public static readonly int screenWidth = 100;
        public static readonly int screenHeight = 70;
        public static RLRootConsole rootConsole;

        // The map console takes up most of the screen and is where the map will be drawn
        public static readonly int mapWidth = 80;
        public static readonly int mapHeight = 48;
        public static RLConsole mapConsole;

        // Below the map console is the message console which displays attack rolls and other information
        private static readonly int messageWidth = 80;
        private static readonly int messageHeight = 11;
        private static RLConsole Messages;

        // The stat console is to the right of the map and display player and monster stats
        private static readonly int statWidth = 20;
        private static readonly int statHeight = 70;
        private static RLConsole Stats;

        // Above the map is the inventory console which shows the players items
        private static readonly int quickbarWidth = 80;
        private static readonly int quickbarHeight = 11;
        private static RLConsole QuickBar;

        public static void Main()
        {
            int seed = (int)DateTime.UtcNow.Ticks;
            Random = new DotNetRandom(seed);

            string fontFileName = "terminal8x8.png";
            string consoleTitle = $"Game tutorial - Level {mapLevel} - Seed {seed}";

            rootConsole = new RLRootConsole(fontFileName, screenWidth, screenHeight, 8, 8, 1f, consoleTitle);
            mapConsole = new RLConsole(mapWidth, mapHeight);
            Messages = new RLConsole(messageWidth, messageHeight);
            Stats = new RLConsole(statWidth, statHeight);
            QuickBar = new RLConsole(quickbarWidth, quickbarHeight);

            SchedulingSystem = new ActionScheduling();

            MapGenerator mapGenerator = new MapGenerator(mapWidth, mapHeight, 20, 13, 7, mapLevel);
            DungeonMap = mapGenerator.CreateMap(mapConsole);
            DungeonMap.UpdatePlayerFieldOfView();

            CommandSystem = new CommandSystem();

            // Create a new MessageLog and print the random seed used to generate the level
            MessageLog = new MessageLog();
            MessageLog.Add("The rogue arrives on level 1", RLColor.White);
            MessageLog.Add($"Level created with seed '{seed}'", RLColor.White);

            // PlayerInventory keeps player's equipment and prints its content onto the QuickBar
            //PlayerInventory = new PlayerInventory();

            PlaceConsoles();
            //rootConsole.CursorVisible = true;
            
            rootConsole.Render += OnRootConsoleUpdate;
            rootConsole.Render += OnRootConsoleRender;


            rootConsole.Run();
        }


        private static void PlaceConsoles()
        {
            // Set backround colors and text for each console
            mapConsole.SetBackColor(0, 0, mapWidth, mapHeight, Colors.FloorBackground);
            mapConsole.Print(1, 1, "", Colors.TextHeading);

            //QuickBar.SetBackColor(0, 0, quickbarWidth, quickbarHeight, Palette.DbWood);
            //QuickBar.Print(1, 1, "Inventory", Colors.TextHeading);
        }

        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            bool didPlayerAct = false;
            RLKeyPress keyPress = rootConsole.Keyboard.GetKeyPress();

            didPlayerAct = CommandSystem.GetKeyPress(keyPress);

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
                RLConsole.Blit(mapConsole, 0, 0, mapWidth, mapHeight,
                 rootConsole, 0, quickbarHeight);
                RLConsole.Blit(Stats, 0, 0, statWidth, statHeight,
                  rootConsole, mapWidth, 0);
                RLConsole.Blit(Messages, 0, 0, messageWidth, messageHeight,
                  rootConsole, 0, screenHeight - messageHeight);
                RLConsole.Blit(QuickBar, 0, 0, quickbarWidth, quickbarHeight,
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
