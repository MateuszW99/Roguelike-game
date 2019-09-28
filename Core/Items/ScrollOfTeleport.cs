using Game.Logic.MapGeneration;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Items
{
    public class ScrollOfTeleport : ItemActive
    {
        public ScrollOfTeleport(int x, int y) : base(x, y)
        {
            Symbol = 'T';
            Name = "Scroll of Teleportation";
        }

        public override void GiveEffect()
        {
            bool teleported = true;
            while (!teleported)
            {
                int randomRoom = Game.Random.Next(0, DungeonMap.Rooms.Count - 1);
                int destinationX = DungeonMap.Rooms[randomRoom].Center.X + 1;
                int destinationY = DungeonMap.Rooms[randomRoom].Center.Y + 1;
                //if()
            }
        }
    }

}
