using System;
using System.Collections;

public class BitMaps
{
	public static BitArray[,] bombRight;
	public static BitArray[,] bombLeft;
	public static BitArray[,] bombUp;
	public static BitArray[,] bombDown;

	public static BitArray[] moveRight;
	public static BitArray[] moveLeft;
	public static BitArray[] moveUp;
	public static BitArray[] moveDown;

	public static void GenerateMoves()
	{
		moveRight = new BitArray[Constants.PlayableSquares];
		moveLeft = new BitArray[Constants.PlayableSquares];
		moveUp = new BitArray[Constants.PlayableSquares];
		moveDown = new BitArray[Constants.PlayableSquares];

		for (int p = 0; p < Constants.PlayableSquares; p++)
		{
			var mapPos = BitState.GetMapIndex(p);

			// right
			var bits = new BitArray(Constants.PlayableSquares);
			bits[p] = true;

			if (mapPos.Item1 + 1 < Constants.BoardWidth && mapPos.Item2 % 2 == 0)
			{
				bits[p + 1] = true;
			}

			moveRight[p] = bits;

			// left
			bits = new BitArray(Constants.PlayableSquares);
			bits[p] = true;

			if (mapPos.Item1 - 1 >= 0 && mapPos.Item2 % 2 == 0)
			{
				bits[p - 1] = true;
			}

			moveLeft[p] = bits;

			// up
			bits = new BitArray(Constants.PlayableSquares);
			bits[p] = true;

			if (mapPos.Item2 - 1 >= 0 && mapPos.Item1 % 2 == 0)
			{
				bits[BitState.GetBitIndex(mapPos.Item1, mapPos.Item2 - 1)] = true;
			}

			moveUp[p] = bits;

			// down
			bits = new BitArray(Constants.PlayableSquares);
			bits[p] = true;

			if (mapPos.Item2 + 1 < Constants.BoardHeight && mapPos.Item1 % 2 == 0)
			{
				bits[BitState.GetBitIndex(mapPos.Item1, mapPos.Item2 + 1)] = true;
			}

			moveDown[p] = bits;
		}
	}

	public static void GenerateBombs()
	{
		bombRight = new BitArray[Constants.BoardWidth, Constants.PlayableSquares];
		bombLeft = new BitArray[Constants.BoardWidth, Constants.PlayableSquares];
		bombUp = new BitArray[Constants.BoardWidth, Constants.PlayableSquares];
		bombDown = new BitArray[Constants.BoardWidth, Constants.PlayableSquares];
		for (int i = 1; i <= Constants.BoardWidth; i++)
		{
			for (int p = 0; p < Constants.PlayableSquares; p++)
			{
				var mapPos = BitState.GetMapIndex(p);

				// right
				var bits = new BitArray(Constants.PlayableSquares);

				if (mapPos.Item1 + i < Constants.BoardWidth && mapPos.Item2 % 2 == 0)
				{
					bits[p + i] = true;
				}

				bombRight[i - 1, p] = bits;

				// left
				bits = new BitArray(Constants.PlayableSquares);

				if (mapPos.Item1 - i >= 0 && mapPos.Item2 % 2 == 0)
				{
					bits[p - i] = true;
				}

				bombLeft[i - 1, p] = bits;

				// up
				bits = new BitArray(Constants.PlayableSquares);

				if (mapPos.Item2 - i >= 0 && mapPos.Item1 % 2 == 0)
				{
					bits[BitState.GetBitIndex(mapPos.Item1, mapPos.Item2 - i)] = true;
				}

				bombUp[i - 1, p] = bits;

				// down
				bits = new BitArray(Constants.PlayableSquares);

				if (mapPos.Item2 + i < Constants.BoardHeight && mapPos.Item1 % 2 == 0)
				{
					bits[BitState.GetBitIndex(mapPos.Item1, mapPos.Item2 + i)] = true;
				}

				bombDown[i - 1, p] = bits;
			}
		}
	}
}
