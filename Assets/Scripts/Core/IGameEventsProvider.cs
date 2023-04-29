using System;

namespace Checkers.Core
{
    public interface IGameEventsProvider
    {
        event Action<int> ChipRemoved;

        event Action<int, Position> ChipMoved;

        event Action<ColorType> PlayerWon;
    }
}