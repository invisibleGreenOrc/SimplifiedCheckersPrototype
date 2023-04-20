using System;
using System.Collections.Generic;
using System.Linq;

namespace Checkers.Core
{
    public class Board
    {
        public int[,] Cells { get; private set; }

        public List<Chip> ChipsOnBoard { get; private set; }

        public ColorType ActivePlayerColor { get; private set; } = ColorType.White;

        public event Action<int> ChipRemoved;

        public event Action<ColorType> PlayerWon;

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

            ChipsOnBoard = new List<Chip>()
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

            _allowedMoveDirections = new Dictionary<ColorType, Position[]>()
            {
                { ColorType.White, new Position[]{ new Position(1, -1), new Position(1, 1)} },
                { ColorType.Black, new Position[]{ new Position(-1, -1), new Position(-1, 1)} },
            };
        }

        public void RemoveChip(Position chipPosition)
        {
            Chip chip = ChipsOnBoard.Where(chip => chip.Position == chipPosition).FirstOrDefault();

            if (chip is not null && ChipsOnBoard.Remove(chip))
            {
                ChipRemoved?.Invoke(chip.Id);

                int chipsLeft = ChipsOnBoard.Where(item => item.Color == chip.Color).Count();

                if (chipsLeft <= 0)
                {
                    PlayerWon?.Invoke(ActivePlayerColor);
                }
            }
        }

        public bool TryMoveChip(int chipId, Position positionToMove)
        {
            Chip chip = ChipsOnBoard.Where(chip => chip.Id == chipId).FirstOrDefault();

            if (chip is not null && (chip.Color == ActivePlayerColor))
            {
                var allowedPositions = GetAllowedPositionsToMoveChip(chip.Id);

                if (allowedPositions.Contains(positionToMove))
                {
                    // Сделать нормально
                    if (Math.Abs(chip.Position.X - positionToMove.X) > 1)
                    {
                        var chipToRemove = ChipsOnBoard.Where(item => item.Position == new Position((chip.Position.X + positionToMove.X) / 2, (chip.Position.Y + positionToMove.Y) / 2)).FirstOrDefault();

                        RemoveChip(chipToRemove.Position);
                    }

                    chip.Position = positionToMove;
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
            bool isChipOnCell = ChipsOnBoard.Select(chip => chip.Position).Contains(position);
            return !isChipOnCell;
        }

        public List<Position> GetAllowedPositionsToMoveChip(int chipId)
        {
            var allowedPositions = new List<Position>();

            Chip chip = ChipsOnBoard.Where(chip => chip.Id == chipId).FirstOrDefault();

            if (chip is not null)
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
                        else if (ChipsOnBoard.Where(chip => chip.Position == consideredPosition).FirstOrDefault().Color != chip.Color)
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

        public void Clear()
        {
            foreach (Chip chip in ChipsOnBoard)
            {
                ChipRemoved?.Invoke(chip.Id);
            }

            ChipsOnBoard.Clear();
        }

        private void PassTurnToNextPlayer()
        {
            ActivePlayerColor = (ColorType)(((int)ActivePlayerColor + 1) % 2);
        }
    }
}