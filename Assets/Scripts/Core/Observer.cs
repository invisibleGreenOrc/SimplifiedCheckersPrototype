using System.IO;
using System.Threading.Tasks;

namespace Checkers.Core
{
    public class Observer
    {
        private IGameEventsProvider _gameEventsProvider;

        private string _filePath;

        public Observer(IGameEventsProvider gameRuleEventsProvider)
        {
            _gameEventsProvider = gameRuleEventsProvider;

            _gameEventsProvider.ChipMoved += OnChipMoved;
            _gameEventsProvider.ChipRemoved += OnChipRemoved;
            _gameEventsProvider.PlayerWon += OnPlayerWon;

            _filePath = Path.ChangeExtension("GameActionsLog", "txt");

            File.Delete(_filePath);
        }

        private void OnChipMoved(int chipId, Position newPosition)
        {
            var str = $"{GameAction.ChipMoved} {chipId} {newPosition}";

            SerializeAsync(str);
        }

        private void OnChipRemoved(int chipId)
        {
            var str = $"{GameAction.ChipRemoved} {chipId}";

            SerializeAsync(str);
        }

        private void OnPlayerWon(ColorType playerColor)
        {
            var str = $"{GameAction.PlayerWon} {playerColor}";

            SerializeAsync(str);
        }

        private async Task SerializeAsync(string input)
        {
            await using (var fileStream = new FileStream(_filePath, FileMode.Append))
            {
                await using (var streamWriter = new StreamWriter(fileStream))
                {
                    await streamWriter.WriteLineAsync(input);
                    streamWriter.Flush();
                }
            }
        }
    }
}
