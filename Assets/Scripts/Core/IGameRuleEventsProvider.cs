using System;

namespace Checkers.Core
{
    public interface IGameRuleEventsProvider
    {
        public event Action<ColorType> PlayerWon;
    }
}
