public class Bomb : Unit
{
	public int countDown
	{
		get
		{
			return this.param1;
		}
		set
		{
			this.param1 = value;
		}
	}

	public int range
	{
		get
		{
			return this.param2;
		}
		set
		{
			this.param2 = value;
		}
	}

	public Bomb Clone()
	{
		var clone = new Bomb();
		clone.owner = this.owner;
		clone.param1 = this.param1;
		clone.param2 = this.param2;
		clone.position = this.position;

		return clone;
	}
}
