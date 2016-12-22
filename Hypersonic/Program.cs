using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class Player
{
	static void Main(string[] args)
	{
		int turnNumber = 0;

		string[] inputs;
		inputs = Console.ReadLine().Split(' ');
		int width = int.Parse(inputs[0]);
		int height = int.Parse(inputs[1]);
		int myId = int.Parse(inputs[2]);

		// game loop
		while (true)
		{
			turnNumber++;

			char[,] map = new char[width, height];
			List<Robot> bots = new List<Robot>();
			List<Bomb> bombs = new List<Bomb>();

			for (int i = 0; i < height; i++)
			{
				var row = Console.ReadLine().ToCharArray();
				for (int j = 0; j < width; j++)
				{
					map[j, i] = row[j];
				}
			}

			int entities = int.Parse(Console.ReadLine());
			for (int i = 0; i < entities; i++)
			{
				inputs = Console.ReadLine().Split(' ');

				var entityType = int.Parse(inputs[0]);
				if (entityType == Constants.ENTITY_ROBOT)
				{
					var r = new Robot();
					r.owner = int.Parse(inputs[1]);
					var x = int.Parse(inputs[2]);
					var y = int.Parse(inputs[3]);
					r.position = BitState.GetBitIndex(x, y);
					r.param1 = int.Parse(inputs[4]);
					r.param2 = int.Parse(inputs[5]);

					bots.Add(r);
				}
				else if (entityType == Constants.ENTITY_ITEM)
				{
					if (int.Parse(inputs[4]) == Constants.ITEM_RANGE)
					{
						map[int.Parse(inputs[2]), int.Parse(inputs[3])] = Constants.MAP_ITEM_RANGE;
					}
					else
					{
						map[int.Parse(inputs[2]), int.Parse(inputs[3])] = Constants.MAP_ITEM_BOMB;
					}
				}
				else
				{
					var r = new Bomb();
					r.owner = int.Parse(inputs[1]);
					var x = int.Parse(inputs[2]);
					var y = int.Parse(inputs[3]);
					r.position = BitState.GetBitIndex(x, y);
					r.param1 = int.Parse(inputs[4]);
					r.param2 = int.Parse(inputs[5]);

					bombs.Add(r);
					map[x, y] = Constants.MAP_BOMB;
				}
			}

			var bgs = new BitState(map, bots, bombs, turnNumber);

			var best = BitSolution.generateBestRandomSolution(bgs, myId, 20, 80);

			bgs.play(best.moves[0], myId);
			bgs.getBot(myId).getCommand(best.moves[0]);
		}
	}
}
