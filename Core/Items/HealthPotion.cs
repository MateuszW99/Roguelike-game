using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Items
{
    public class HealthPotion : ItemActive
    {
        public HealthPotion(int? x, int? y) : base(x, y)
        {
            Symbol = 'H';
            Name = "Health Potion";
        }

        // Heal 15 life
        public override void GiveEffect()
        {
            int regen = 15;
            Game.Player.Health += regen;
            if (Game.Player.Health > 100)
            {
                Game.Player.Health = 100;
                Game.MessageLog.Add("Life fully regenered!");
            }
            else
            {
                Game.MessageLog.Add($"Healed {regen} life!");
            }
        }

    }
}
