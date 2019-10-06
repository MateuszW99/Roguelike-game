using Game.Core;

namespace Game.Interfaces
{
    public interface IItem
    {
        string Name { get; set; }
        char Symbol { get; set; }

        void Add();
        void Use();
        
    }
}
