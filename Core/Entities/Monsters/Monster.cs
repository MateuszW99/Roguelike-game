using Game.Logic;
using Game.Logic.Behavior;
using System;

namespace Game.Core
{
    public class Monster : Actor
    {
        public void DrawStats(RLNET.RLConsole statConsole, int position)
        {
            // Y = 13 is below the player stats
            int yPosition = 13 + (position * 2);

            statConsole.Print(1, yPosition, Symbol.ToString(), Color);

            AddHealthBar(statConsole, 3, yPosition);

            statConsole.Print(2, yPosition, $": {Name}", Palette.DbLight);
        }

        // We want a null integer when the monster hasn't been alerted
        public int? TurnsAlerted { get; set; }

        public virtual void PerformAction(CommandSystem commandSystem)
        {
            var behavior = new MoveAndAttack();
            behavior.Act(this, commandSystem);
        }
    }
}
