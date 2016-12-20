using System;

public class Unit : IEquatable<Unit>
{
	public int owner;
	public int param1;
	public int param2;
	public int x;
	public int y;
	public int position;

	public bool Equals(Unit other)
	{
		return null != other && this.x == other.x && this.y == other.y && this.owner == other.owner;
	}

	public override bool Equals(object obj)
	{
		return this.Equals(obj as Unit);
	}

	public override int GetHashCode()
	{
		return this.owner * 3 + this.x * 5 + this.y * 7;
	}
}
