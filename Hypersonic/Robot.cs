using System;
using System.Collections;
using System.Collections.Generic;

public class Robot : Unit
{
	public bool isAlive = true;
	public int boxesDestroyed = 0;
	public int maxBombs = 1;
	public int bombs
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

	public int bombRange
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

	public Robot Clone()
	{
		var clone = new Robot();
		clone.owner = this.owner;
		clone.param1 = this.param1;
		clone.param2 = this.param2;
		clone.position = this.position;
		clone.maxBombs = this.maxBombs;
		clone.isAlive = this.isAlive;
		clone.boxesDestroyed = this.boxesDestroyed;

		return clone;
	}

	public Bomb makeBomb()
	{
		var b = new Bomb();
		b.countDown = Constants.BOMB_TIMER;
		b.range = this.bombRange;
		b.owner = this.owner;
		b.position = this.position;

		this.bombs--;

		return b;
	}

	public void getCommand(Move move)
	{
		var mapPos = BitState.GetMapIndex(this.position);
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
			Console.WriteLine("MOVE {0} {1}", comx, comy);
		}
		else
		{
			Console.WriteLine("BOMB {0} {1}", comx, comy);
		}
	}
}
