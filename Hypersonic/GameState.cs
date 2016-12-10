using System;
using System.Collections.Generic;
using System.Linq;

public class GameState
{
	public char[,] map;
	public int[] scores;
	public Robot self;
	public List<Robot> enemies;
	public List<Bomb> bombs;
	public int turnNumber;

	private GameState()
	{
	}

	public GameState(char[,] map, Robot self, List<Robot> enemies, List<Bomb> bombs, int[] scores, int turnNumber)
	{
		this.map = map;
		this.self = self;
		this.enemies = enemies;
		this.bombs = bombs;
		this.turnNumber = turnNumber;
		this.scores = scores;
	}

	public GameState Clone()
	{
		var clone = new GameState();
		clone.map = (char[,])this.map.Clone();
		clone.scores = (int[])this.scores.Clone();
		clone.enemies = this.enemies.ConvertAll(e => e.Clone());
		clone.bombs = this.bombs.ConvertAll(b => b.Clone());
		clone.self = self.Clone();

		return clone;
	}

	public void play(Solution s)
	{
		foreach (Move m in s.moves)
		{
			this.play(m);
			if (!this.self.isAlive)
			{
				return;
			}
		}
	}

	public void play(Move move)
	{
		foreach (Bomb b in this.bombs)
		{
			b.countDown--;
		}

		// handle explosions
		this.handleExplosions();

		// move
		if (move.type == MoveType.Bomb)
		{
			this.map[self.x, self.y] = Constants.MAP_BOMB;
			this.bombs.Add(self.makeBomb());
		}

		switch (move.direction)
		{
			case MoveDirection.Right:
				self.x++;
				break;

			case MoveDirection.Left:
				self.x--;
				break;

			case MoveDirection.Up:
				self.y--;
				break;

			case MoveDirection.Down:
				self.y++;
				break;
		}

		this.handleNewPosition(move.type);
	}

	public void handleExplosions()
	{
		var exploded = new List<Bomb>();
		var destroyedBoxes = new List<Box>();
		var destroyedItems = new List<Unit>();

		List<Bomb> toExplode = this.bombs.Where(bomb => bomb.countDown <= 0).ToList();

		while (toExplode.Count > 0)
		{
			var b = toExplode[0];

			if (this.self.x == b.x && this.self.y == b.y)
			{
				this.self.isAlive = false;
				return;
			}

			// left
			for (int i = 1; i < b.range; i++)
			{
				var explosionX = b.x - i;
				var explosionY = b.y;
				if (explosionX < 0)
				{
					break;
				}

				if (!handleExplosion(b, explosionX, explosionY, toExplode, exploded, destroyedBoxes, destroyedItems))
				{
					break;
				}
			}

			// right
			for (int i = 1; i < b.range; i++)
			{
				var explosionX = b.x + i;
				var explosionY = b.y;
				if (explosionX >= this.map.GetLength(0))
				{
					break;
				}

				if (!handleExplosion(b, explosionX, explosionY, toExplode, exploded, destroyedBoxes, destroyedItems))
				{
					break;
				}
			}

			// up
			for (int i = 1; i < b.range; i++)
			{
				var explosionX = b.x;
				var explosionY = b.y - i;
				if (explosionY < 0)
				{
					break;
				}

				if (!handleExplosion(b, explosionX, explosionY, toExplode, exploded, destroyedBoxes, destroyedItems))
				{
					break;
				}
			}

			// down
			for (int i = 1; i < b.range; i++)
			{
				var explosionX = b.x;
				var explosionY = b.y + i;
				if (explosionY >= this.map.GetLength(1))
				{
					break;
				}

				if (!handleExplosion(b, explosionX, explosionY, toExplode, exploded, destroyedBoxes, destroyedItems))
				{
					break;
				}
			}

			toExplode.RemoveAt(0);
			exploded.Add(b);
		}

		foreach (Bomb b in exploded)
		{
			this.bombs.Remove(b);
			if (b.owner == this.self.owner)
			{
				self.bombs++;
			}
			else
			{
				foreach (Robot r in this.enemies)
				{
					if (b.owner == r.owner)
					{
						r.bombs++;
					}
				}
			}

			this.map[b.x, b.y] = Constants.MAP_FLOOR;
		}

		foreach (Box b in destroyedBoxes)
		{
			this.map[b.x, b.y] = b.item;
		}

		foreach (Unit i in destroyedItems)
		{
			this.map[i.x, i.y] = Constants.MAP_FLOOR;
		}
	}

	private bool handleExplosion(Bomb b, int explosionX, int explosionY, List<Bomb> toExplode, List<Bomb> exploded, List<Box> destroyedBoxes, List<Unit> destroyedItems)
	{
		if (this.self.x == explosionX && this.self.y == explosionY)
		{
			this.self.isAlive = false;
			return false;
		}

		var cell = this.map[explosionX, explosionY];
		if (cell == Constants.MAP_BOX || cell == Constants.MAP_BOX_RANGE || cell == Constants.MAP_BOX_BOMB)
		{
			this.scores[b.owner]++;

			var box = new Box();
			box.x = explosionX;
			box.y = explosionY;

			if (cell == Constants.MAP_BOX_RANGE)
			{
				box.item = Constants.MAP_ITEM_RANGE;
			}
			else if (cell == Constants.MAP_BOX_BOMB)
			{
				box.item = Constants.MAP_ITEM_BOMB;
			}

			destroyedBoxes.Add(box);
			return false;
		}
		else if (cell == Constants.MAP_BOMB)
		{
			var temp = this.bombs.First(bomb => bomb.x == explosionX && bomb.y == explosionY);
			if (!toExplode.Contains(temp) && !exploded.Contains(temp))
			{
				toExplode.Add(temp);
			}
			return false;
		}
		else if (cell == Constants.MAP_ITEM_RANGE || cell == Constants.MAP_ITEM_BOMB)
		{
			var item = new Unit();
			item.x = explosionX;
			item.y = explosionY;
			destroyedItems.Add(item);
			return false;
		}
		else if (cell == Constants.MAP_WALL)
		{
			return false;
		}

		return true;
	}

	private void handleNewPosition(MoveType type)
	{
		var cell = this.map[this.self.x, this.self.y];
		if (cell == Constants.MAP_ITEM_BOMB)
		{
			this.self.maxBombs++;
		}
		else if (cell == Constants.MAP_ITEM_RANGE)
		{
			this.self.bombRange++;
		}
		else
		{
			return;
		}

		this.map[this.self.x, this.self.y] = Constants.MAP_FLOOR;
	}

	public double score()
	{
		if (!this.self.isAlive)
		{
			return -8000000;
		}

		double score = 0;

		for (int i = 0; i < this.scores.Length; i++)
		{
			if (i == this.self.owner)
			{
				score += 7 * this.scores[i];
			}
			else
			{
				score -= 7 * this.scores[i];
			}

			score += this.self.bombRange + this.self.maxBombs;
		}

		return score;
	}

	public void printMap()
	{
		for (int i = 0; i < this.map.GetLength(1); i++)
		{
			for (int j = 0; j < this.map.GetLength(0); j++)
			{
				if (this.self.x == j && this.self.y == i)
				{
					Console.Error.Write("*");
				}
				else
				{
					Console.Error.Write(this.map[j, i]);
				}
			}
			Console.Error.WriteLine();
		}
	}
}
