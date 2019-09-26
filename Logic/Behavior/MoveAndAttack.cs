using Game.Core;
using Game.Logic.MapGeneration;
using RogueSharp;

namespace Game.Logic.Behavior
{
    public class MoveAndAttack : IBehavior
    {
        public bool Act(Monster monster, CommandSystem commandSystem)
        {
            DungeonMap dungeonMap = Game.DungeonMap;
            Player player = Game.Player;
            FieldOfView monsterFOV = new FieldOfView(dungeonMap);

            // When the monster hasn't been alerted, compute its FOV
            // Use its Awareness value for the distance in the FOV
            // If the player is in the monster's FOV alert it
            // Print the message to messageLog 
            if (!monster.TurnsAlerted.HasValue)
            {
                monsterFOV.ComputeFov(monster.X, monster.Y, monster.Awareness, true);
                if(monsterFOV.IsInFov(player.X, player.Y))
                {
                    Game.MessageLog.Add($"{monster.Name} was alerted by {player.Name}!");
                    monster.TurnsAlerted = 1;
                }
            }

            if(monster.TurnsAlerted.HasValue)
            {
                // Before finding a path, make sure to make the monster and player Cells walkable
                dungeonMap.SetIsWalkable(monster.X, monster.Y, true);
                dungeonMap.SetIsWalkable(player.X, player.Y, true);

                PathFinder pathFinder = new PathFinder(dungeonMap);
                Path path = null;

                try
                {
                    path = pathFinder.ShortestPath(
                        dungeonMap.GetCell(monster.X, monster.Y),
                        dungeonMap.GetCell(player.X, player.Y));
                }
                catch(PathNotFoundException)
                {
                    // Something made it impossible for the monster to reach the player
                    // It could be the otherm onsters blocking the way
                    Game.MessageLog.Add($"{monster.Name} is wating for a turn.");
                }

                dungeonMap.SetIsWalkable(monster.X, monster.Y, false);
                dungeonMap.SetIsWalkable(player.X, player.Y, false);
                if(path != null)
                {
                    try
                    {
                        // TODO: This should be path.StepForward() but there is a bug in RogueSharp V3
                        // The bug is that a Path returned from a PathFinder does not include the source Cell
                        commandSystem.MoveMonster(monster, (Cell)path.StepForward());
                    }
                    catch(NoMoreStepsException)
                    {
                        Game.MessageLog.Add($"{monster.Name} growls at the {player.Name} in anger!");
                    }
                }

                monster.TurnsAlerted++;

                // Lose alerted status every 15 turns
                // As long as the player is in monster's FOV, the monster will stay alerted
                // Otherwise, the creature will stop chasing the player
                if(monster.TurnsAlerted > 15)
                {
                    monster.TurnsAlerted = null;
                }
            }

            return true;
        }
    }
}
