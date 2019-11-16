namespace Hypersonic
{
    public enum MoveType
    {
        Move,
        Bomb
    }

    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right,
        Stay
    }

    public class Move
    {
        public MoveType type;
        public MoveDirection direction;

        public Move(MoveType type, MoveDirection direction)
        {
            this.type = type;
            this.direction = direction;
        }

        public override string ToString()
        {
            return this.type.ToString() + " " + this.direction.ToString();
        }
    }
}
