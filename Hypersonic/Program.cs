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
		//Solution lastBest = null;
		int[] scores = null;

		// game loop
		while (true)
		{
			turnNumber++;

			char[,] map = new char[width, height];
			Robot self = new Robot();
			List<Robot> enemies = new List<Robot>();
			List<Bomb> bombs = new List<Bomb>();

			for (int i = 0; i < height; i++)
			{
				var row = Console.ReadLine().ToCharArray();
				for (int j = 0; j < width; j++)
				{
					map[j, i] = row[j];
					//Console.Error.Write(row[j]);
				}
				//Console.Error.WriteLine();
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
					r.x = int.Parse(inputs[2]);
					r.y = int.Parse(inputs[3]);
					r.param1 = int.Parse(inputs[4]);
					r.param2 = int.Parse(inputs[5]);

					if (r.owner == myId)
					{
						self = r;
					}
					else
					{
						enemies.Add(r);
					}
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
					r.x = int.Parse(inputs[2]);
					r.y = int.Parse(inputs[3]);
					r.param1 = int.Parse(inputs[4]);
					r.param2 = int.Parse(inputs[5]);

					bombs.Add(r);
					map[r.x, r.y] = Constants.MAP_BOMB;
				}
			}

			// Write an action using Console.WriteLine()
			// To debug: Console.Error.WriteLine("Debug messages...");

			//Console.WriteLine("BOMB 1 0");
			// Random rnd = new Random();
			// var moves = self.getMoves(map);
			// self.getCommand(moves[rnd.Next(moves.Count)]);

			// if (enemies.Count > 1)
			// {
			//     Console.WriteLine("MOVE {0} {1}", self.x, self.y);
			//     continue;
			// }

			//if (turnNumber == 1)
			//{
			//	scores = new int[enemies.Count + 1];
			//}

			var gameState = new GameState(map, self, enemies, bombs, turnNumber);

			// gameState.printMap();
			// Console.Error.WriteLine();
			// foreach (Move m in best.moves)
			// {
			//     Console.Error.WriteLine(m);
			//     gameState.play(m);
			//     gameState.printMap();
			//     Console.Error.WriteLine(gameState.score());
			//     Console.Error.WriteLine();
			// }

			//Console.Error.WriteLine("Score: {0}", best.score);
			//Console.Error.WriteLine("Move: {0}", best.moves[0]);
			//Console.Error.WriteLine("Is alive: {0}", gameState.self.isAlive);
			//Console.Error.WriteLine("Position: ({0}, {1})", gameState.self.x, gameState.self.y);

			var best = Solution.generateBestRandomSolution(gameState, 20, 90);

			gameState.play(best.moves[0]);
			self.getCommand(best.moves[0]);
			//foreach (int s in scores)
			//{
			//	Console.Error.WriteLine(s);
			//}
			// best.moves.RemoveAt(0);
			// lastBest = best;
		}
	}
}
