using Game.Core;
using Game.Core.Items;

namespace Game.Logic
{
    static class RandomDrop
    {
        public static void Next(Monster monster)
        {
            int rand = Game.Random.Next(1, 5);
            switch (rand)
            {

                case 1:
                case 2:
                    {
                        GoldPile.DropGold(monster);
                        return;
                    }
                case 3:
                case 4:
                    {
                        ItemPassive.RandomDrop(monster);
                        return;
                    }
                case 5:
                    {
                        ItemActive.RandomDrop(monster);
                        return;
                    }
            }
        }
    }
}
