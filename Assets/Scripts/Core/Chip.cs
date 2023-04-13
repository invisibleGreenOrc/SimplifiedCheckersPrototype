namespace Checkers.Core
{
    public class Chip
    {
        public Color Color { get; private set; }

        public Position Position { get; set; }

        public Chip(Color color, int x, int y)
        {
            Color = color;
            Position = new Position() { X = x, Y = y };
        }
    }
}