namespace Checkers.Core
{
    public class Chip
    {
        public ColorType Color { get; private set; }

        public Position Position { get; set; }

        public Chip(ColorType color, int x, int y)
        {
            Color = color;
            Position = new Position() { X = x, Y = y };
        }
    }
}