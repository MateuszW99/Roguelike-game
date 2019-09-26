using Game.Core;

namespace Game.Logic.Behavior
{
    public interface IBehavior
    {
        bool Act(Monster monster, CommandSystem command);
    }
}
