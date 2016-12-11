using System;
using System.Collections;
using System.Collections.Generic;

public class BitState
{
	public const int BoardWidth = 13;
	public const int BoardHeight = 11;
	public const int PlayableSquares = 113;
	public BitArray boxes;
	public BitArray items;
	public BitArray[] players;
	public BitArray[] bombs;

	public static int GetBitIndex(int x, int y)
	{
		int index = -1;
		for (int j = 0; j < y; j++)
		{
			for (int i = 0; i < x; i++)
			{
				if (i % 2 == 0 || j % 2 == 0)
				{
					// no box
					index++;
				}
			}
		}

		return index;
	}

	public static Tuple<int, int> GetMapIndex(int index)
	{
		int x = 0;
		int y = 0;

		for (int j = 0; j < BoardHeight; j++)
		{
			for (int i = 0; i < BoardWidth; i++)
			{
				if (index <= 0)
				{
					break;
				}

				y = j;
				x = i;
				index--;
			}

		}

		return new Tuple<int, int>(x, y);
	}
}