using System;
using System.Collections.Generic;
using System.Linq;

namespace Checkers.Core
{
    public class Board
    {
        public int[,] Cells { get; private set; }

        public Dictionary<int, Chip> ChipsOnBoard { get; private set; }

        public ColorType ActivePlayerColor { get; private set; } = ColorType.White;

        public event Action<int> ChipRemoved;

        private Dictionary<ColorType, Position[]> _allowedMoveDirections;

        public Board()
        {
            Cells = new int[,]
            {
                { 1, 0, 1, 0, 1, 0, 1, 0 },
                { 0, 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0 },
                { 0, 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0 },
                { 0, 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0 },
                { 0, 1, 0, 1, 0, 1, 0, 1 }
            };

            ChipsOnBoard = new Dictionary<int, Chip>()
            {
                { 0, new Chip(ColorType.White, 0, 0) },
                { 1, new Chip(ColorType.White, 0, 2) },
                { 2, new Chip(ColorType.White, 0, 4) },
                { 3, new Chip(ColorType.White, 0, 6) },
                { 4, new Chip(ColorType.White, 1, 1) },
                { 5, new Chip(ColorType.White, 1, 3) },
                { 6, new Chip(ColorType.White, 1, 5) },
                { 7, new Chip(ColorType.White, 1, 7) },
                { 8, new Chip(ColorType.White, 2, 0) },
                { 9, new Chip(ColorType.White, 2, 2) },
                { 10, new Chip(ColorType.White, 2, 4) },
                { 11, new Chip(ColorType.White, 2, 6) },
                { 12, new Chip(ColorType.Black, 5, 1) },
                { 13, new Chip(ColorType.Black, 5, 3) },
                { 14, new Chip(ColorType.Black, 5, 5) },
                { 15, new Chip(ColorType.Black, 5, 7) },
                { 16, new Chip(ColorType.Black, 6, 0) },
                { 17, new Chip(ColorType.Black, 6, 2) },
                { 18, new Chip(ColorType.Black, 6, 4) },
                { 19, new Chip(ColorType.Black, 6, 6) },
                { 20, new Chip(ColorType.Black, 7, 1) },
                { 21, new Chip(ColorType.Black, 7, 3) },
                { 22, new Chip(ColorType.Black, 7, 5) },
                { 23, new Chip(ColorType.Black, 7, 7) },
            };

            _allowedMoveDirections = new Dictionary<ColorType, Position[]>()
            {
                { ColorType.White, new Position[]{ new Position(1, -1), new Position(1, 1)} },
                { ColorType.Black, new Position[]{ new Position(-1, -1), new Position(-1, 1)} },
            };
        }

        public void RemoveChip(int id)
        {
            if (ChipsOnBoard.Remove(id))
            {
                ChipRemoved?.Invoke(id);
            }
        }

        public bool TryMoveChip(int chipId, Position newPosition)
        {
            if (ChipsOnBoard.TryGetValue(chipId, out var chip) && (chip.Color == ActivePlayerColor))
            {
                var allowedPositions = GetAllowedPositionsToMoveChip(chipId);

                if (allowedPositions.Contains(newPosition))
                {
                    // Сделать нормально
                    if (Math.Abs(chip.Position.X - newPosition.X) > 1)
                    {
                        var chipToRemove = ChipsOnBoard.Where(item => item.Value.Position == new Position((chip.Position.X + newPosition.X) / 2, (chip.Position.Y + newPosition.Y) / 2)).FirstOrDefault();

                        RemoveChip(chipToRemove.Key);
                    }

                    chip.Position = newPosition;
                    PassTurnToNextPlayer();

                    return true;
                }
            }

            return false;
        }

        public bool IsPositionInBoard(Position position)
        {
            return position.X < Cells.GetLength(0) && position.Y < Cells.GetLength(1);
        }

        public bool IsCellFree(Position position)
        {
            bool isChipOnCell = ChipsOnBoard.Values.Select(chip => chip.Position).Contains(position);
            return !isChipOnCell;
        }

        public List<Position> GetAllowedPositionsToMoveChip(int chipId)
        {
            var allowedPositions = new List<Position>();

            if (ChipsOnBoard.TryGetValue(chipId, out var chip))
            {
                foreach (Position position in _allowedMoveDirections[chip.Color])
                {
                    var consideredPosition = new Position() { X = chip.Position.X + position.X, Y = chip.Position.Y + position.Y };

                    if (IsPositionInBoard(consideredPosition))
                    {
                        if (IsCellFree(consideredPosition))
                        {
                            allowedPositions.Add(consideredPosition);
                        }
                        else if (ChipsOnBoard.Values.Where(chip => chip.Position == consideredPosition).FirstOrDefault().Color != chip.Color)
                        {
                            consideredPosition = new Position() { X = consideredPosition.X + position.X, Y = consideredPosition.Y + position.Y };

                            if (IsPositionInBoard(consideredPosition) && IsCellFree(consideredPosition))
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
    }
}