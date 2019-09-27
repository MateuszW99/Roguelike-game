using Game.Core;

namespace Game.Interfaces
{
    public interface IItem
    {
        string Name { get; set; }
        char Symbol { get; set; }

        void Add(Player player);
        void Use(Player player, int? itemNumber);
        
    }
}
