using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class BitState
{
	public const int BoardWidth = 13;
	public const int BoardHeight = 11;
	public const int PlayableSquares = 113;
	public const int MaxPlayers = 4;
	public const int BoxTypes = 3;
	public const int ItemTypes = 2;
	public BitArray[] boxes;
	public BitArray[] items;
	public BitArray[] playerMaps;
	public BitArray[] bombMap;
	public List<Robot> bots;
	public List<Bomb> bombs;
	public int turnNumber;

	private BitState()
	{
		this.playerMaps = new BitArray[MaxPlayers];
		this.bombMap = new BitArray[MaxPlayers];
		this.boxes = new BitArray[BoxTypes];
		this.items = new BitArray[ItemTypes];
	}

	public BitState(char[,] map, List<Robot> bots, List<Bomb> bombs, int turnNumber): this()
	{
		this.turnNumber = turnNumber;
		this.bots = bots;
		this.bombs = bombs;

		for (int i = 0; i < BoxTypes; i++)
		{
			this.boxes[i] = new BitArray(PlayableSquares);
		}

		for (int i = 0; i < ItemTypes; i++)
		{
			this.items[i] = new BitArray(PlayableSquares);
		}

		for (int i = 0; i < MaxPlayers; i++)
		{
			this.playerMaps[i] = new BitArray(PlayableSquares);
			this.bombMap[i] = new BitArray(PlayableSquares);
		}

		foreach (Robot bot in bots)
		{
			this.playerMaps[bot.owner][GetBitIndex(bot.x, bot.y)] = true;
		}

		foreach (Bomb bomb in bombs)
		{
			this.bombMap[bomb.owner][GetBitIndex(bomb.x, bomb.y)] = true;
		}

		int index = 0;
		for (int j = 0; j < BoardHeight; j++)
		{
			for (int i = 0; i < BoardWidth; i++)
			{
				if (i % 2 == 1 && j % 2 == 1)
				{
					// skip the walls
					continue;
				}

				if (map[i, j] == Constants.MAP_BOX)
				{
					this.boxes[0][GetBitIndex(i, j)] = true;
				}
				else if (map[i, j] == Constants.MAP_BOX_RANGE)
				{
					this.boxes[1][GetBitIndex(i, j)] = true;
				}
				else if (map[i, j] == Constants.MAP_BOX_BOMB)
				{
					this.boxes[2][GetBitIndex(i, j)] = true;
				}
				else if (map[i, j] == Constants.MAP_ITEM_RANGE)
				{
					this.items[0][GetBitIndex(i, j)] = true;
				}
				else if (map[i, j] == Constants.MAP_ITEM_BOMB)
				{
					this.items[1][GetBitIndex(i, j)] = true;
				}

				index++;
			}
		}
	}

	public BitState Clone()
	{
		var clone = new BitState();
		clone.bots = this.bots.ConvertAll(e => e.Clone());
		clone.bombs = this.bombs.ConvertAll(b => b.Clone());

		for (int i = 0; i < BoxTypes; i++)
		{
			clone.boxes[i] = new BitArray(this.boxes[i]);
		}

		for (int i = 0; i < ItemTypes; i++)
		{
			clone.items[i] = new BitArray(this.items[i]);
		}

		for (int i = 0; i < MaxPlayers; i++)
		{
			clone.playerMaps[i] = new BitArray(this.playerMaps[i]);
			clone.bombMap[i] = new BitArray(this.bombMap[i]);
		}

		return clone;
	}

	public Robot getBot(int id)
	{
		return this.bots.FirstOrDefault(x => x.owner == id);
	}

	public void play(Move move, int playerId)
	{
		this.tick();

		var bot = this.getBot(playerId);

		this.handleExplosions();

		// get position
		var position = this.getPosition(this.playerMaps[playerId]);

		// place bomb if necessary
		if (move.type == MoveType.Bomb)
		{
			this.bombMap[playerId][position] = true;
			this.bombs.Add(bot.makeBomb());
		}

		// move
		switch (move.direction)
		{
			case MoveDirection.Right:
				this.playerMaps[playerId].Xor(Bitmaps.right[0][position]);
				break;

			case MoveDirection.Left:
				this.playerMaps[playerId].Xor(Bitmaps.left[0][position]);
				break;

			case MoveDirection.Up:
				this.playerMaps[playerId].Xor(Bitmaps.up[0][position]);
				break;

			case MoveDirection.Down:
				this.playerMaps[playerId].Xor(Bitmaps.down[0][position]);
				break;
		}

		bot.position = this.getPosition(this.playerMaps[playerId]);
	}

	public static int GetBitIndex(int x, int y)
	{
		if (x % 2 == 1 && y % 2 == 1)
		{
			// it's a wall and we don't capture those
			return -1;
		}

		int index = -1;
		for (int j = 0; j < BoardHeight; j++)
		{
			for (int i = 0; i < BoardWidth; i++)
			{
				if (i % 2 == 1 && j % 2 == 1)
				{
					// skip the walls
					continue;
				}

				index++;

				if (x == i && y == j)
				{
					return index;
				}
			}
		}

		return index;
	}

	public static Tuple<int, int> GetMapIndex(int index)
	{
		int x = 0;
		int y = 0;

		for (int j = 0; j < BoardHeight && index >= 0; j++)
		{
			for (int i = 0; i < BoardWidth && index >= 0; i++)
			{
				y = j;
				x = i;

				// only count the floors
				if (i % 2 == 0 || j % 2 == 0)
				{
					index--;
				}
			}

		}

		return new Tuple<int, int>(x, y);
	}

	public void printMap()
	{
		int index = 0;
		for (int j = 0; j < BoardHeight; j++)
		{
			for (int i = 0; i < BoardWidth; i++)
			{
				if (i % 2 == 0 || j % 2 == 0)
				{
					if (this.boxes[0][index])
					{
						Console.Error.Write(Constants.MAP_BOX);
					}
					else if (this.boxes[1][index])
					{
						Console.Error.Write(Constants.MAP_BOX_RANGE);
					}
					else if (this.boxes[2][index])
					{
						Console.Error.Write(Constants.MAP_BOX_BOMB);
					}
					else if (this.items[0][index])
					{
						Console.Error.Write(Constants.MAP_ITEM_RANGE);
					}
					else if (this.items[1][index])
					{
						Console.Error.Write(Constants.MAP_ITEM_BOMB);
					}
					else
					{
						Console.Error.Write(Constants.MAP_FLOOR);
					}

					index++;
				}
				else
				{
					Console.Error.Write(Constants.MAP_WALL);
				}
			}

			Console.Error.WriteLine();
		}
	}

	public void handleExplosions()
	{
		var exploded = new List<Bomb>();
		var destroyedBoxes = new List<BitArray>();
		var destroyedItems = new List<BitArray>();

		List<Bomb> toExplode = this.bombs.Where(bomb => bomb.countDown <= 0).ToList();

		while (toExplode.Count > 0)
		{
			var b = toExplode[0];
			if (exploded.Contains(b))
			{
				toExplode.RemoveAt(0);
				continue;
			}

			foreach (Robot r in this.bots)
			{
				if (r.position == b.position)
				{
					r.isAlive = false;
				}
			}

			bool continueLeft = true;
			bool continueRight = true;
			bool continueUp = true;
			bool continueDown = true;
			for (int i = 1; i < b.range; i++)
			{
				if (continueLeft)
				{
					var l = Bitmaps.left[i][b.position];

					var hits = this.getBombHits(l);
					if (hits.Count > 0)
					{
						toExplode.AddRange(hits);
						continueLeft = false;
					}

					var boxHits = this.getBoxHits(l);
					if (boxHits.Count > 0)
					{
						destroyedBoxes.AddRange(boxHits);
						continueLeft = false;
					}

					var itemHits = this.getItemHits(l);
					if (itemHits.Count > 0)
					{
						destroyedItems.AddRange(itemHits);
						continueLeft = false;
					}

					foreach (int p in this.getPlayerHits(l))
					{
						this.getBot(p).isAlive = false;
					}
				}

				if (continueRight)
				{
					var r = Bitmaps.right[i][b.position];
					var hits = this.getBombHits(r);
					if (hits.Count > 0)
					{
						toExplode.AddRange(hits);
						continueRight = false;
					}

					var boxHits = this.getBoxHits(r);
					if (boxHits.Count > 0)
					{
						destroyedBoxes.AddRange(boxHits);
						continueRight = false;
					}

					var itemHits = this.getItemHits(r);
					if (itemHits.Count > 0)
					{
						destroyedItems.AddRange(itemHits);
						continueRight = false;
					}

					foreach (int p in this.getPlayerHits(r))
					{
						this.getBot(p).isAlive = false;
					}
				}

				if (continueUp)
				{
					var u = Bitmaps.up[i][b.position];
					var hits = this.getBombHits(u);
					if (hits.Count > 0)
					{
						toExplode.AddRange(hits);
						continueUp = false;
					}

					var boxHits = this.getBoxHits(u);
					if (boxHits.Count > 0)
					{
						destroyedBoxes.AddRange(boxHits);
						continueUp = false;
					}

					var itemHits = this.getItemHits(u);
					if (itemHits.Count > 0)
					{
						destroyedItems.AddRange(itemHits);
						continueUp = false;
					}

					foreach (int p in this.getPlayerHits(u))
					{
						this.getBot(p).isAlive = false;
					}
				}

				if (continueDown)
				{
					var d = Bitmaps.down[i][b.position];
					var hits = this.getBombHits(d);
					if (hits.Count > 0)
					{
						toExplode.AddRange(hits);
						continueDown = false;
					}

					var boxHits = this.getBoxHits(d);
					if (boxHits.Count > 0)
					{
						destroyedBoxes.AddRange(boxHits);
						continueDown = false;
					}

					var itemHits = this.getItemHits(d);
					if (itemHits.Count > 0)
					{
						destroyedItems.AddRange(itemHits);
						continueDown = false;
					}

					foreach (int p in this.getPlayerHits(d))
					{
						this.getBot(p).isAlive = false;
					}
				}
			}

			toExplode.RemoveAt(0);
			exploded.Add(b);
		}

		foreach (Bomb b in exploded)
		{
			this.bombs.Remove(b);

			foreach (Robot r in this.bots)
			{
				if (b.owner == r.owner)
				{
					r.bombs++;
				}
			}

			this.bombMap[b.owner][b.position] = false;
		}

		foreach (BitArray b in destroyedBoxes)
		{
			for (int i = 0; i < BoxTypes; i++)
			{
				this.boxes[i].Xor(b);
				this.items[i].And(b);
			}
		}

		foreach (BitArray i in destroyedItems)
		{
			foreach (BitArray itemMap in this.items)
			{
				itemMap.Xor(i);
			}
		}
	}

	private List<Bomb> getBombHits(BitArray mask)
	{
		var hits = new List<Bomb>();

		for (int p = 0; p < MaxPlayers; p++)
		{
			// check player bombs
			var hit = new BitArray(mask).And(this.bombMap[p]);
			if (hit.Cast<bool>().Contains(true))
			{
				// find bombs that got hit
				hits.AddRange(this.bombs.Where(b => b.position == this.getPosition(hit)));
			}
		}

		return hits;
	}

	private List<BitArray> getBoxHits(BitArray mask)
	{
		var hits = new List<BitArray>();

		for (int p = 0; p < BoxTypes; p++)
		{
			var hit = new BitArray(mask).And(this.boxes[p]);
			if (hit.Cast<bool>().Contains(true))
			{
				hits.Add(hit);
				return hits;
			}
		}

		return hits;
	}

	private List<BitArray> getItemHits(BitArray mask)
	{
		var hits = new List<BitArray>();

		for (int p = 0; p < ItemTypes; p++)
		{
			var hit = new BitArray(mask).And(this.items[p]);
			if (hit.Cast<bool>().Contains(true))
			{
				hits.Add(hit);
				return hits;
			}
		}

		return hits;
	}

	private List<int> getPlayerHits(BitArray mask)
	{
		var hits = new List<int>();

		for (int p = 0; p < MaxPlayers; p++)
		{
			var hit = new BitArray(mask).And(this.playerMaps[p]);
			if (hit.Cast<bool>().Contains(true))
			{
				hits.Add(p);
				return hits;
			}
		}

		return hits;
	}

	private void tick()
	{
		foreach (Bomb b in this.bombs)
		{
			b.countDown--;
		}

		this.turnNumber++;
	}

	private int getPosition(BitArray array)
	{
		int[] intArray = (int[])array.GetType().GetField("m_array", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(array);

		for (var i = 0; i < intArray.Length; i++)
		{
			var b = intArray[i];
			if (b != 0)
			{
				var pos = (i << 5) + 31;
				for (int bit = 31; bit >= 0; bit--)
				{
					if ((b & (1 << bit)) != 0)
						return pos;

					pos--;
				}

				return pos;
			}
		}

		return -1;
	}

	public static void GenerateMoves()
	{
		using (StreamWriter s = new StreamWriter("right.txt"))
		{
			var printed = false;
			s.Write("BitArray[][] right = { ");
			for (int i = 1; i < BoardWidth; i++)
			{
				if (printed)
				{
					s.WriteLine(",");
				}

				s.Write("new BitArray[] { ");

				var printed2 = false;
				for (int p = 0; p < PlayableSquares; p++)
				{
					var bits = new BitArray(PlayableSquares);
					bits[p] = true;

					var mapPos = GetMapIndex(p);

					if (mapPos.Item1 + i < BoardWidth && mapPos.Item2 % 2 == 0)
					{
						bits[p + i] = true;	
					}

					if (printed2)
					{
						s.Write(", ");
					}

					var ints = new int[4];
					bits.CopyTo(ints, 0);

					s.Write("new BitArray(new int[] {{ {0}, {1}, {2}, {3} }})", ints[0], ints[1], ints[2], ints[3]);
					printed2 = true;
				}

				s.Write(" }");
				printed = true;
			}

			s.WriteLine(" };");

			printed = false;
			s.Write("BitArray[][] left = { ");
			for (int i = 1; i < BoardWidth; i++)
			{
				if (printed)
				{
					s.WriteLine(",");
				}

				s.Write("new BitArray[] { ");

				var printed2 = false;
				for (int p = 0; p < PlayableSquares; p++)
				{
					var bits = new BitArray(PlayableSquares);
					bits[p] = true;

					var mapPos = GetMapIndex(p);

					if (mapPos.Item1 - i >= 0 && mapPos.Item2 % 2 == 0)
					{
						bits[p - i] = true;
					}

					if (printed2)
					{
						s.Write(", ");
					}

					var ints = new int[4];
					bits.CopyTo(ints, 0);

					s.Write("new BitArray(new int[] {{ {0}, {1}, {2}, {3} }})", ints[0], ints[1], ints[2], ints[3]);
					printed2 = true;
				}

				s.Write(" }");
				printed = true;
			}

			s.WriteLine(" };");

			printed = false;
			s.Write("BitArray[][] up = { ");
			for (int i = 1; i < BoardHeight; i++)
			{
				if (printed)
				{
					s.WriteLine(",");
				}

				s.Write("new BitArray[] { ");

				var printed2 = false;
				for (int p = 0; p < PlayableSquares; p++)
				{
					var bits = new BitArray(PlayableSquares);
					bits[p] = true;

					var mapPos = GetMapIndex(p);

					if (mapPos.Item2 - i >= 0 && mapPos.Item1 % 2 == 0)
					{
						bits[GetBitIndex(mapPos.Item1, mapPos.Item2 - i)] = true;
					}

					if (printed2)
					{
						s.Write(", ");
					}

					var ints = new int[4];
					bits.CopyTo(ints, 0);

					s.Write("new BitArray(new int[] {{ {0}, {1}, {2}, {3} }})", ints[0], ints[1], ints[2], ints[3]);
					printed2 = true;
				}

				s.Write(" }");
				printed = true;
			}

			s.WriteLine(" };");

			printed = false;
			s.Write("BitArray[][] down = { ");
			for (int i = 1; i < BoardHeight; i++)
			{
				if (printed)
				{
					s.WriteLine(",");
				}

				s.Write("new BitArray[] { ");

				var printed2 = false;
				for (int p = 0; p < PlayableSquares; p++)
				{
					var bits = new BitArray(PlayableSquares);
					bits[p] = true;

					var mapPos = GetMapIndex(p);

					if (mapPos.Item2 + i < BoardHeight && mapPos.Item1 % 2 == 0)
					{
						bits[GetBitIndex(mapPos.Item1, mapPos.Item2 + i)] = true;
					}

					if (printed2)
					{
						s.Write(", ");
					}

					var ints = new int[4];
					bits.CopyTo(ints, 0);

					s.Write("new BitArray(new int[] {{ {0}, {1}, {2}, {3} }})", ints[0], ints[1], ints[2], ints[3]);
					printed2 = true;
				}

				s.Write(" }");
				printed = true;
			}

			s.WriteLine(" };");
		}
	}
}