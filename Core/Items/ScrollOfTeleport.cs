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
        private static int quantity = 0;
        public static new int Quantity { get => quantity; set => quantity = value; }

        public ScrollOfTeleport(int? x, int? y) : base(x, y)
        {
            ScrollOfTeleport.Quantity++;
            Symbol = 'T';
            Name = "Scroll of Teleportation";
        }

        public override void GiveEffect()
        {
            bool teleported = false;
            while (!teleported)
            {
                int randomRoom = Game.Random.Next(0, DungeonMap.Rooms.Count - 1);
                int destinationX = DungeonMap.Rooms[randomRoom].Center.X - 1;
                int destinationY = DungeonMap.Rooms[randomRoom].Center.Y - 1;
                if(DungeonMap.CanTeleport(destinationX, destinationY))
                {
                    Game.DungeonMap.SetActorPostion(Game.Player, destinationX, destinationY);
                    teleported = true;
                }
            }
        }

    }

}
