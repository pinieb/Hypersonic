using System;

public class Unit : IEquatable<Unit>
{
	public int owner;
	public int param1;
	public int param2;
	public int position;

	public bool Equals(Unit other)
	{
		return null != other && position == other.position && owner == other.owner;
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as Unit);
	}

	public override int GetHashCode()
	{
		return owner * 3 + position * 5;
	}

	public int right()
	{
		if ((position >= 0 && position < 12) ||
			(position >= 20 && position < 32) ||
			(position >= 40 && position < 52) ||
			(position >= 60 && position < 72) ||
			(position >= 80 && position < 92) ||
			(position >= 100 && position < 112))
		{
			return position + 1;
		}

		return -1;
	}

	public int left()
	{
		if ((position > 0 && position <= 12) ||
			(position > 20 && position <= 32) ||
			(position > 40 && position <= 52) ||
			(position > 60 && position <= 72) ||
			(position > 80 && position <= 92) ||
			(position > 100 && position <= 112))
		{
			return position - 1;
		}

		return -1;
	}

	public int down()
	{
		if (position % 2 == 0 &&
		    (position >= 0 && position <= 12) ||
			(position >= 20 && position <= 32) ||
			(position >= 40 && position <= 52) ||
			(position >= 60 && position <= 72) ||
			(position >= 80 && position <= 92))
		{
			return position - (position % 20) / 2 + 13;
		}
		else if ((position >= 13 && position <= 19) ||
				 (position >= 33 && position <= 39) ||
				 (position >= 53 && position <= 59) ||
				 (position >= 73 && position <= 79) ||
				 (position >= 93 && position <= 99))
		{
			return position + position % (13 + 20 * (position / 20)) + 7;
		}

		return -1;
	}

	public int up()
	{
		if (position % 2 == 0 &&
		    (position >= 20 && position <= 32) ||
			(position >= 40 && position <= 52) ||
			(position >= 60 && position <= 72) ||
			(position >= 80 && position <= 92) ||
		    (position >= 100 && position <= 112))
		{
			return position - (position % 20) / 2 - 7;
		}
		else if ((position >= 13 && position <= 19) ||
				 (position >= 33 && position <= 39) ||
				 (position >= 53 && position <= 59) ||
				 (position >= 73 && position <= 79) ||
				 (position >= 93 && position <= 99))
		{
			return position + position % (13 + 20 * (position / 20)) - 13;
		}

		return -1;
	}
}
