using Game.Logic.MapGeneration;

namespace Game.Core.Items
{
    public class ScrollOfDestruction : ItemActive
    {

        public ScrollOfDestruction(int? x, int? y) : base(x, y)
        {
            //Quantity++;
            Symbol = 'D';
            Name = "Scroll of Destruction";
        }

        public override void GiveEffect()
        {
            HitMonstersAround();
        }

        private void HitMonstersAround()
        {
            // top row
            int y = Game.Player.Y - 1;
            for (int x = Game.Player.X - 1; x <= Game.Player.X + 2; x++)
            {
                Monster monster = Game.DungeonMap.GetMonsterAt(x, y);
                if (monster != null)
                {
                    monster.Health -= Game.Random.Next(1, 10);
                    CheckMonsterHealth(monster);
                }
            }

            // cell on the left & on the right from the player
            y = Game.Player.Y;
            for (int x = Game.Player.X - 1; x <= Game.Player.X + 1; x += 2)
            {
                Monster monster = Game.DungeonMap.GetMonsterAt(x, y);
                if (monster != null)
                {
                    monster.Health -= Game.Random.Next(1, 10);
                    CheckMonsterHealth(monster);
                }
            }

            // bottom row
            y = Game.Player.Y + 1;
            for (int x = Game.Player.X - 1; x <= Game.Player.X + 2; x++)
            {
                Monster monster = Game.DungeonMap.GetMonsterAt(x, y);
                if (monster != null)
                {
                    monster.Health -= Game.Random.Next(1, 10);
                    CheckMonsterHealth(monster);
                }
            }
        }

        private void CheckMonsterHealth(Monster monster)
        {
            if(monster.Health <= 0)
            {
                Game.DungeonMap.RemoveMonster(monster);
            }
        }
    }
}
