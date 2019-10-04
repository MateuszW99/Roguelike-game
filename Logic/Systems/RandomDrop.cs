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
                    {
                        GoldPile.DropGold(monster);
                        return;
                    }
                case 3:
                case 4:
                    {
                        Sword sword = new Sword(monster.X, monster.Y);
                        sword.DropItem(monster);
                        return;
                    }
                case 5:
                case 6:
                    {
                        Armor armor = new Armor(monster.X, monster.Y);
                        armor.DropItem(monster);
                        return;
                    }
                case 7:
                case 8:
                    {
                        int randomDrop = Game.Random.Next(0, 4);
                        if (randomDrop == 0)
                        {
                            return;
                        }
                        else if (randomDrop == 1)
                        {
                            ScrollOfTeleport scroll = new ScrollOfTeleport(monster.X, monster.Y);
                            scroll.DropItem(monster);
                            return;
                        }
                        else if (randomDrop == 2)
                        {
                            ScrollOfDestruction scroll = new ScrollOfDestruction(monster.X, monster.Y);
                            scroll.DropItem(monster);
                            return;
                        }
                        else if (randomDrop == 3 || randomDrop == 4)
                        {
                            HealthPotion potion = new HealthPotion(monster.X, monster.Y);
                            potion.DropItem(monster);
                            return;
                        }
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
