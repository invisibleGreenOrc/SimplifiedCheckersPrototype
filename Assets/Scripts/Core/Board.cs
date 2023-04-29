using System;
using System.Collections.Generic;
using System.Linq;

namespace Checkers.Core
{
    public class Board
    {
        public int[,] Cells { get; private set; }

        public List<Chip> ChipsOnBoard { get; private set; }

        public event Action<int> ChipRemoved;

        public event Action<int, Position> ChipMoved;

        public Board(List<Chip> chipsOnBoard)
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

            ChipsOnBoard = chipsOnBoard;
        }

        public int GetChipsCount(ColorType color)
        {
            int chipsCount = ChipsOnBoard.Where(chip => chip.Color == color).Count(); ;

            return chipsCount;
        }

        public void RemoveChip(Chip chip)
        {
            if (chip is not null && ChipsOnBoard.Remove(chip))
            {
                ChipRemoved?.Invoke(chip.Id);
            }
        }

        public void MoveChip(int chipId, Position positionToMove)
        {
            Chip chip = GetChip(chipId);

            if (chip is not null && IsPositionInBoard(positionToMove))
            {
                chip.Position = positionToMove;
                ChipMoved?.Invoke(chip.Id, chip.Position);
            }
        }

        public Chip GetChip(int chipId)
        {
            Chip chip = ChipsOnBoard.Where(chip => chip.Id == chipId).FirstOrDefault();
            return chip;
        }

        public Chip GetChip(Position chipPosition)
        {
            Chip chip = ChipsOnBoard.Where(chip => chip.Position == chipPosition).FirstOrDefault();
            return chip;
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

        public void Clear()
        {
            foreach (Chip chip in ChipsOnBoard)
            {
                ChipRemoved?.Invoke(chip.Id);
            }

            ChipsOnBoard.Clear();
        }
    }
}