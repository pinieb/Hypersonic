using System;
using System.Collections.Generic;

public class Robot : Unit
{
	public bool isAlive = true;
	public int boxesDestroyed = 0;
	public int maxBombs = 1;

	public Bomb hitBy = null;

	public int bombs
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

	public int bombRange
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

	public Robot Clone()
	{
		var clone = new Robot();
		clone.owner = owner;
		clone.param1 = param1;
		clone.param2 = param2;
		clone.position = position;
		clone.maxBombs = maxBombs;
		clone.isAlive = isAlive;
		clone.boxesDestroyed = boxesDestroyed;

		return clone;
	}

	public Bomb makeBomb()
	{
		var b = new Bomb();
		b.countDown = Constants.BOMB_TIMER;
		b.range = bombRange;
		b.owner = owner;
		b.position = position;

		bombs--;

		return b;
	}

	public string getCommand(Move move)
	{
		var mapPos = BitState.GetMapIndex(position);
		int comx = mapPos.Item1;
		int comy = mapPos.Item2;

		switch (move.direction)
		{
			case MoveDirection.Right:
				comx++;
				break;

			case MoveDirection.Left:
				comx--;
				break;

			case MoveDirection.Up:
				comy--;
				break;

			case MoveDirection.Down:
				comy++;
				break;
		}

		if (move.type == MoveType.Move)
		{
			return "MOVE " + comx + " " + comy;
		}
		else
		{
			return "BOMB " + comx + " " + comy;
		}
	}
}
