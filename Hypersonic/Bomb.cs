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
		var clone = new Bomb();
		clone.owner = owner;
		clone.param1 = param1;
		clone.param2 = param2;
		clone.position = position;

		return clone;
	}
}
