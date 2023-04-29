using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Checkers.Core
{
    public class FileReplayer : IReplayer
    {
        private List<string> _source = new();

        private int _delay;

        public event Action<int> ChipRemoved;
        
        public event Action<int, Position> ChipMoved;
        
        public event Action<ColorType> PlayerWon;

        public FileReplayer(string filePath, int delay)
        {
            ReadFile(filePath);
            _delay = delay;
        }

        public async void Start()
        {
            foreach (string sourceRecord in _source)
            {
                string[] recordParts = sourceRecord.Split(' ');

                Enum.TryParse(recordParts[0], out GameAction action);

                switch (action)
                {
                    case GameAction.ChipMoved:
                        await ChipMovedCommandHandler(recordParts);
                        break;

                    case GameAction.ChipRemoved:
                        await ChipRemovedCommandHandler(recordParts);
                        break;

                    case GameAction.PlayerWon:
                        await PlayerWonCommandHandler(recordParts);
                        break;

                    default:
                        break;
                }
            }
        }

        private Task PlayerWonCommandHandler(string[] recordParts)
        {
            if (Enum.TryParse(recordParts[1], out ColorType color))
            {
                PlayerWon?.Invoke(color);
            }

            return Task.CompletedTask;
        }

        private Task ChipRemovedCommandHandler(string[] recordParts)
        {
            if (int.TryParse(recordParts[1], out int chipId))
            {
                ChipRemoved?.Invoke(chipId);
            }

            return Task.CompletedTask;
        }

        private Task ChipMovedCommandHandler(string[] recordParts)
        {
            if (int.TryParse(recordParts[1], out int chipId) && int.TryParse(recordParts[2], out int x) && int.TryParse(recordParts[3], out int y))
            {
                ChipMoved?.Invoke(chipId, new Position(x, y));
            }

            return Task.Delay(_delay);
        }

        private void ReadFile(string filePath)
        {
            if (File.Exists(filePath) is false)
            {
                throw new FileNotFoundException();
            }

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    while (!streamReader.EndOfStream)
                    {
                        _source.Add(streamReader.ReadLine());
                    }
                }
            }
        }
    }
}
