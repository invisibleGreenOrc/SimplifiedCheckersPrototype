using System;
using System.Collections.Generic;
using System.Linq;

namespace Checkers.Core
{
    public class CheckersGame
    {
        private Board _board;

        private List<Chip> _startChipsPosition;
        
        private Dictionary<ColorType, Position[]> _allowedMoveDirections;

        private Dictionary<ColorType, Position[]> _winPositions;
        
        public event Action<ColorType> PlayerWon;

        public event Action<int> ChipRemoved;

        public ColorType ActivePlayerColor { get; private set; } = ColorType.White;

        public int[,] BoardCells { get => _board?.Cells; }

        public List<Chip> ChipsOnBoard { get => _board?.ChipsOnBoard; }

        public CheckersGame()
        {
            _allowedMoveDirections = new Dictionary<ColorType, Position[]>()
            {
                { ColorType.White, new Position[]{ new Position(1, -1), new Position(1, 1)} },
                { ColorType.Black, new Position[]{ new Position(-1, -1), new Position(-1, 1)} },
            };

            _startChipsPosition = new List<Chip>()
            {
                {new Chip(0, ColorType.White, 0, 0) },
                {new Chip(1, ColorType.White, 0, 2) },
                {new Chip(2, ColorType.White, 0, 4) },
                {new Chip(3, ColorType.White, 0, 6) },
                {new Chip(4, ColorType.White, 1, 1) },
                {new Chip(5, ColorType.White, 1, 3) },
                {new Chip(6, ColorType.White, 1, 5) },
                {new Chip(7, ColorType.White, 1, 7) },
                {new Chip(8, ColorType.White, 2, 0) },
                {new Chip(9, ColorType.White, 2, 2) },
                {new Chip(10, ColorType.White, 2, 4) },
                {new Chip(11, ColorType.White, 2, 6) },
                {new Chip(12, ColorType.Black, 5, 1) },
                {new Chip(13, ColorType.Black, 5, 3) },
                {new Chip(14, ColorType.Black, 5, 5) },
                {new Chip(15, ColorType.Black, 5, 7) },
                {new Chip(16, ColorType.Black, 6, 0) },
                {new Chip(17, ColorType.Black, 6, 2) },
                {new Chip(18, ColorType.Black, 6, 4) },
                {new Chip(19, ColorType.Black, 6, 6) },
                {new Chip(20, ColorType.Black, 7, 1) },
                {new Chip(21, ColorType.Black, 7, 3) },
                {new Chip(22, ColorType.Black, 7, 5) },
                {new Chip(23, ColorType.Black, 7, 7) },
            };

            _winPositions = new Dictionary<ColorType, Position[]>()
            {
                { ColorType.White, new Position[7] },
                { ColorType.Black, new Position[7] },
            };

            for (var i = 0; i < 7; i++)
            {
                _winPositions[ColorType.Black][i] = new Position(0, i);
                _winPositions[ColorType.White][i] = new Position(7, i);
            }
        }

        public void StartNewGame()
        {
            if (_board is not null)
            {
                _board.ChipRemoved -= OnChipRemoved;
            }

            _board = new Board(_startChipsPosition);

            _board.ChipRemoved += OnChipRemoved;
        }

        public bool TryMakeMove(int chipId, Position positionToMove)
        {
            Chip chipToMove = _board.GetChip(chipId);

            if (chipToMove is not null && (chipToMove.Color == ActivePlayerColor))
            {
                var allowedPositions = GetAllowedPositionsToMoveChip(chipToMove.Id);

                if (allowedPositions.Contains(positionToMove))
                {
                    // Сделать нормально
                    if (Math.Abs(chipToMove.Position.X - positionToMove.X) > 1)
                    {
                        var removedChipPosition = new Position((chipToMove.Position.X + positionToMove.X) / 2, (chipToMove.Position.Y + positionToMove.Y) / 2);
                        Chip chipToRemove = _board.GetChip(removedChipPosition);

                        RemoveChip(chipToRemove.Id);
                    }

                    MoveChip(chipToMove.Id, positionToMove);
                    PassTurnToNextPlayer();

                    return true;
                }
            }

            return false;
        }

        public List<Position> GetAllowedPositionsToMoveChip(int chipId)
        {
            var allowedPositions = new List<Position>();

            Chip chip = _board.GetChip(chipId);

            if (chip is not null)
            {
                foreach (Position position in _allowedMoveDirections[chip.Color])
                {
                    var consideredPosition = new Position() { X = chip.Position.X + position.X, Y = chip.Position.Y + position.Y };

                    if (_board.IsPositionInBoard(consideredPosition))
                    {
                        if (_board.IsCellFree(consideredPosition))
                        {
                            allowedPositions.Add(consideredPosition);
                        }
                        else if (_board.GetChip(consideredPosition).Color != chip.Color)
                        {
                            consideredPosition = new Position() { X = consideredPosition.X + position.X, Y = consideredPosition.Y + position.Y };

                            if (_board.IsPositionInBoard(consideredPosition) && _board.IsCellFree(consideredPosition))
                            {
                                allowedPositions.Add(consideredPosition);
                            }
                        }
                    }
                }
            }

            return allowedPositions;
        }

        private void PassTurnToNextPlayer()
        {
            ActivePlayerColor = (ColorType)(((int)ActivePlayerColor + 1) % 2);
        }

        private void OnChipRemoved(int chipId)
        {
            ChipRemoved?.Invoke(chipId);
        }

        private void RemoveChip(int chipId)
        {
            Chip chipToRemove = _board.GetChip(chipId);

            if (chipToRemove is not null)
            {
                _board.RemoveChip(chipToRemove);

                if (_board.GetChipsCount(chipToRemove.Color) <= 0)
                {
                    PlayerWon?.Invoke(ActivePlayerColor);
                }
            }
        }

        private void MoveChip(int chipId, Position positionToMove)
        {
            _board.MoveChip(chipId, positionToMove);

            Chip movedChip = _board.GetChip(chipId);

            if (_winPositions[movedChip.Color].Contains(movedChip.Position))
            {
                PlayerWon?.Invoke(movedChip.Color);
            }
        }
    }
}
