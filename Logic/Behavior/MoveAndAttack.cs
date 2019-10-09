using Game.Core;
using Game.Logic.MapGeneration;
using RogueSharp;

namespace Game.Logic.Behavior
{
    public class MoveAndAttack : IBehavior
    {
        public bool Act(Monster monster, CommandSystem commandSystem)
        {
            FieldOfView monsterFOV = new FieldOfView(Game.DungeonMap);

            // When the monster hasn't been alerted, compute its FOV
            // Use its Awareness value for the distance in the FOV
            // If the player is in the monster's FOV alert it
            // Print the message to messageLog 
            if (!monster.TurnsAlerted.HasValue)
            {
                monsterFOV.ComputeFov(monster.X, monster.Y, monster.Awareness, true);
                if(monsterFOV.IsInFov(Game.Player.X, Game.Player.Y))
                {
                    Game.MessageLog.Add($"{monster.Name} was alerted by {Game.Player.Name}!");
                    monster.TurnsAlerted = 1;
                }
            }

            if(monster.TurnsAlerted.HasValue)
            {
                // Before finding a path monster and player cells must be walkable
                Game.DungeonMap.SetIsWalkable(monster.X, monster.Y, true);
                Game.DungeonMap.SetIsWalkable(Game.Player.X, Game.Player.Y, true);

                PathFinder pathFinder = new PathFinder(Game.DungeonMap);
                Path path = null;

                try
                {
                    Game.DungeonMap.SetIsWalkable(monster.X, monster.Y, false);
                    Game.DungeonMap.SetIsWalkable(Game.Player.X, Game.Player.Y, false);
                    if (monster.IsInRange())
                    {
                        commandSystem.Attack(monster, Game.Player);
                        return true;
                    }


                    path = pathFinder.ShortestPath(
                        Game.DungeonMap.GetCell(monster.X, monster.Y),
                        Game.DungeonMap.GetCell(Game.Player.X, Game.Player.Y));

                }
                catch(PathNotFoundException)
                {
                    // Something made it impossible for the monster to reach the player
                    // i.e. other monsters blocking the way
                    Game.MessageLog.Add($"{monster.Name} is wating for a turn.");
                }


                if(path != null)
                {
                    try
                    {
                        commandSystem.MoveMonster(monster, (Cell)path.StepForward());
                    }
                    catch (NoMoreStepsException)
                    {
                        Game.MessageLog.Add($"{monster.Name} growls at the {Game.Player.Name} in anger!");
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
