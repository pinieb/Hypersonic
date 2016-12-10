public class Box : Unit
{
	public char item = Constants.MAP_FLOOR;

	public Box Clone()
	{
		var clone = new Box();
		clone.owner = this.owner;
		clone.param1 = this.param1;
		clone.param2 = this.param2;
		clone.x = this.x;
		clone.y = this.y;

		return clone;
	}
}
