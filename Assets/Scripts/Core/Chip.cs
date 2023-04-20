namespace Checkers.Core
{
    public class Chip
    {
        public int Id { get; private set; }

        public ColorType Color { get; private set; }

        public Position Position { get; set; }

        public Chip(int id, ColorType color, int x, int y)
        {
            Id = id;
            Color = color;
            Position = new Position(x, y);
        }
    }
}