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
            // Multiply postion by 2 to have more space
            int yPosition = 13 + (position * 2);

            statConsole.Print(1, yPosition, Symbol.ToString(), Color);

            // Calculate the width of health bard
            int width = Convert.ToInt32(((double) Health / (double) MaxHealth) * 16.0);
            int remainingWidth = 16 - width;

            // Set the background colors of the health bar to show damage
            statConsole.SetBackColor(3, yPosition, width, 1, Palette.Primary);
            statConsole.SetBackColor(3 + width, yPosition, remainingWidth, 1, Palette.PrimaryDarkest);

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
