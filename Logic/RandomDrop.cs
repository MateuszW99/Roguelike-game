using Game.Core;
using Game.Core.Items;

namespace Game.Logic
{
    static class RandomDrop
    {
        static int numberOfDrops = 2;
        public static void Next(Monster monster)
        {
            int rand = Game.Random.Next(1, 2);
            switch(rand)
            {
                
                case 1:
                    {
                        GoldPile.DropGold(monster);
                        break;
                    }
                case 2:
                    {
                        Sword sword = new Sword(monster.X, monster.Y);
                        sword.DropItem(monster);
                        break;
                    }
            }
        }
    }
}
