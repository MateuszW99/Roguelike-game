using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Entities.Monsters
{
    class Ghost : Monster
    {
        public static Ghost Create(int level)
        {
            int health = Dice.Roll("1D2");
            return new Ghost
            {
                Attack = Dice.Roll("1D3") + level / 3,
                AttackChance = Dice.Roll("25D3"),
                Awareness = 4,
                Range = 3,
                Color = Colors.Kobold,
                Defense = Dice.Roll("1D3"),
                DefenseChance = 1,
                Gold = Dice.Roll("1D4"),
                Health = health,
                MaxHealth = health,
                Name = "Ghost",
                Speed = 20,
                Symbol = 'g'
            };
        }
    }
}
