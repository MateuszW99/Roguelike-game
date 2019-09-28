using Game.Core;
using Game.Core.Items;

namespace Game.Logic
{
    static class RandomDrop
    {
        //static int numberOfDrops = 2;
        public static void Next(Monster monster)
        {
            int rand = Game.Random.Next(1, 10);
            switch(rand)
            {
                
                case 1:
                case 2:
                case 3:
                    {
                        GoldPile.DropGold(monster);
                        return;
                    }
                case 4:
                    {
                        Sword sword = new Sword(monster.X, monster.Y);
                        sword.DropItem(monster);
                        return;
                    }
                case 5:
                    {
                        Armor armor = new Armor(monster.X, monster.Y);
                        armor.DropItem(monster);
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }
    }
}
