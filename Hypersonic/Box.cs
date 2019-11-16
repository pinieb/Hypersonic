namespace Hypersonic
{
    public class Box : Unit
    {
        public char item = Constants.MAP_FLOOR;

        public Box Clone()
        {
            var clone = new Box
            {
                owner = this.owner,
                param1 = this.param1,
                param2 = this.param2,
                position = this.position
            };

            return clone;
        }
    }
}
