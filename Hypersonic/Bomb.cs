namespace Hypersonic
{
    public class Bomb : Unit
    {
        public int countDown
        {
            get
            {
                return param1;
            }
            set
            {
                param1 = value;
            }
        }

        public int range
        {
            get
            {
                return param2;
            }
            set
            {
                param2 = value;
            }
        }

        public Bomb Clone()
        {
            var clone = new Bomb
            {
                owner = owner,
                param1 = param1,
                param2 = param2,
                position = position
            };

            return clone;
        }
    }
}
