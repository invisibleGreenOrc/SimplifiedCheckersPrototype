using Checkers.Core;

namespace Checkers
{
    public interface IReplayer : IGameEventsProvider
    {
        void Start();
    }
}