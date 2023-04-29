using System;

namespace Checkers.Core
{
    public interface IChipMovementEventsProvider
    {
        public event Action<int> ChipRemoved;

        public event Action<int, Position> ChipMoved;
    }
}
