using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Items
{
    public class Boots : ItemPassive
    {
        public Boots(int x, int y) : base(x, y)
        {
            Symbol = 'B';
            Name = "Boots";
            Stats = -1; // Boots give speed and lower speed => more movement
        }
    }
}
