using RogueSharp.DiceNotation;

namespace Game.Core.Entities.Monsters
{
    class Kobold : Monster
    {
        public static Kobold Create(int level)
        {
            int health = Dice.Roll("2D5");
            return new Kobold
            {
                Attack = Dice.Roll("1D3") + level / 3,
                AttackChance = Dice.Roll("25D3"),
                Awareness = 2,
                Range = 0,
                Color = Colors.Kobold,
                Defense = Dice.Roll("1D3"),
                DefenseChance = Dice.Roll("10D4"),
                Gold = Dice.Roll("1D8"),
                Health = health,
                MaxHealth = health,
                Name = "Lower Kobold",
                Speed = 25,
                Symbol = 'k'
            };
        }
    }
}
