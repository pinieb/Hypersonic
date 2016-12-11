using System;
using System.Collections;
using System.Collections.Generic;

public class BitState
{
	public const int BoardWidth = 13;
	public const int BoardHeight = 11;
	public const int PlayableSquares = 113;
	public const int MaxPlayers = 4;
	public BitArray[] boxes;
	public BitArray[] items;
	public BitArray[] playerMaps;
	public BitArray[] bombMap;
	public List<Robot> bots;
	public List<Bomb> bombs;
	public int turnNumber;

	public BitState(char[,] map, List<Robot> bots, List<Bomb> bombs, int turnNumber)
	{
		this.turnNumber = turnNumber;
		this.bots = bots;
		this.bombs = bombs;

		this.playerMaps = new BitArray[MaxPlayers];
		this.bombMap = new BitArray[MaxPlayers];
		this.boxes = new BitArray[3];
		this.items = new BitArray[2];

		for (int i = 0; i < 3; i++)
		{
			this.boxes[i] = new BitArray(PlayableSquares);
		}

		for (int i = 0; i < 2; i++)
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
}