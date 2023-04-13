using System.Collections.Generic;
using System.Linq;

namespace Checkers.Core
{
    public class Board
    {
        public int[,] Cells { get; private set; }

        public Dictionary<int, Chip> Chips { get; private set; }

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

            Chips = new Dictionary<int, Chip>()
            {
                { 1, new Chip(ColorType.White, 0, 2) },
                { 0, new Chip(ColorType.White, 0, 0) },
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
        }

        public void RemoveChip(int id)
        {
            Chips.Remove(id);
        }

        public bool TryMoveChip(int checkerId, Position newPosition)
        {
            if (IsPositionInBoard(newPosition) is false)
            {
                return false;
            }

            if (Chips.TryGetValue(checkerId, out var checker))
            {
                checker.Position = newPosition;
                return true;
            }

            return false;
        }

        public bool IsPositionInBoard(Position position)
        {
            return position.X < Cells.GetLength(0) && position.Y < Cells.GetLength(1);
        }

        public bool IsCellFree(Position position)
        {
            bool isChipOnCell = Chips.Values.Select(checker => checker.Position).Contains(position);
            return !isChipOnCell;
        }
    }
}