using System;

public class Unit : IEquatable<Unit>
{
	public int owner;
	public int param1;
	public int param2;
	public int position;

	public bool Equals(Unit other)
	{
		return null != other && this.position == other.position && this.owner == other.owner;
	}

	public override bool Equals(object obj)
	{
		return this.Equals(obj as Unit);
	}

	public override int GetHashCode()
	{
		return this.owner * 3 + this.position * 5;
	}

	public int right()
	{
		if ((this.position >= 0 && this.position < 12) ||
			(this.position >= 20 && this.position < 32) ||
			(this.position >= 40 && this.position < 52) ||
			(this.position >= 60 && this.position < 72) ||
			(this.position >= 80 && this.position < 92) ||
			(this.position >= 100 && this.position < 112))
		{
			return this.position + 1;
		}

		return -1;
	}

	public int left()
	{
		if ((this.position > 0 && this.position <= 12) ||
			(this.position > 20 && this.position <= 32) ||
			(this.position > 40 && this.position <= 52) ||
			(this.position > 60 && this.position <= 72) ||
			(this.position > 80 && this.position <= 92) ||
			(this.position > 100 && this.position <= 112))
		{
			return this.position - 1;
		}

		return -1;
	}

	public int down()
	{
		if (this.position % 2 == 0 &&
		    (this.position >= 0 && this.position <= 12) ||
			(this.position >= 20 && this.position <= 32) ||
			(this.position >= 40 && this.position <= 52) ||
			(this.position >= 60 && this.position <= 72) ||
			(this.position >= 80 && this.position <= 92))
		{
			return this.position - (this.position % 20) / 2 + 13;
		}
		else if ((this.position >= 13 && this.position <= 19) ||
				 (this.position >= 33 && this.position <= 39) ||
				 (this.position >= 53 && this.position <= 59) ||
				 (this.position >= 73 && this.position <= 79) ||
				 (this.position >= 93 && this.position <= 99))
		{
			return this.position + this.position % (13 + 20 * (this.position / 20)) + 7;
		}

		return -1;
	}

	public int up()
	{
		if (this.position % 2 == 0 &&
		    (this.position >= 20 && this.position <= 32) ||
			(this.position >= 40 && this.position <= 52) ||
			(this.position >= 60 && this.position <= 72) ||
			(this.position >= 80 && this.position <= 92) ||
		    (this.position >= 100 && this.position <= 112))
		{
			return this.position - (this.position % 20) / 2 - 7;
		}
		else if ((this.position >= 13 && this.position <= 19) ||
				 (this.position >= 33 && this.position <= 39) ||
				 (this.position >= 53 && this.position <= 59) ||
				 (this.position >= 73 && this.position <= 79) ||
				 (this.position >= 93 && this.position <= 99))
		{
			return this.position + this.position % (13 + 20 * (this.position / 20)) - 13;
		}

		return -1;
	}
}
