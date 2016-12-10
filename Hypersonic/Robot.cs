using System;
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
		clone.x = this.x;
		clone.y = this.y;
		clone.maxBombs = this.maxBombs;
		clone.isAlive = this.isAlive;
		clone.boxesDestroyed = this.boxesDestroyed;

		return clone;
	}

	public List<Move> getMoves(char[,] map)
	{
		var types = new List<MoveType>();
		var directions = new List<MoveDirection>();

		types.Add(MoveType.Move);
		if (this.bombs > 0 && map[this.x, this.y] != Constants.MAP_BOMB)
		{
			types.Add(MoveType.Bomb);
		}

		directions.Add(MoveDirection.Stay);
		if (this.x - 1 >= 0)
		{
			var cell = map[this.x - 1, this.y];
			if (cell == Constants.MAP_FLOOR || cell == Constants.MAP_ITEM_RANGE || cell == Constants.MAP_ITEM_BOMB)
			{
				directions.Add(MoveDirection.Left);
			}
		}

		if (this.x + 1 < map.GetLength(0))
		{
			var cell = map[this.x + 1, this.y];
			if (cell == Constants.MAP_FLOOR || cell == Constants.MAP_ITEM_RANGE || cell == Constants.MAP_ITEM_BOMB)
			{
				directions.Add(MoveDirection.Right);
			}
		}

		if (this.y - 1 >= 0)
		{
			var cell = map[this.x, this.y - 1];
			if (cell == Constants.MAP_FLOOR || cell == Constants.MAP_ITEM_RANGE || cell == Constants.MAP_ITEM_BOMB)
			{
				directions.Add(MoveDirection.Up);
			}
		}

		if (this.y + 1 < map.GetLength(1))
		{
			var cell = map[this.x, this.y + 1];
			if (cell == Constants.MAP_FLOOR || cell == Constants.MAP_ITEM_RANGE || cell == Constants.MAP_ITEM_BOMB)
			{
				directions.Add(MoveDirection.Down);
			}
		}

		var moves = new List<Move>();
		foreach (MoveType t in types)
		{
			foreach (MoveDirection d in directions)
			{
				moves.Add(new Move(t, d));
			}
		}

		return moves;
	}

	public Bomb makeBomb()
	{
		var b = new Bomb();
		b.countDown = Constants.BOMB_TIMER;
		b.range = this.bombRange;
		b.owner = this.owner;
		b.x = this.x;
		b.y = this.y;

		this.bombs--;

		return b;
	}

	public void getCommand(Move move)
	{
		int comx = this.x;
		int comy = this.y;

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
