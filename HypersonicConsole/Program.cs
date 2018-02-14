using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HypersonicConsole
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			var timer = new Stopwatch();
			timer.Start();
			BitMaps.GenerateMoves();
			BitMaps.GenerateBombs();
			timer.Stop();

			char[,] map = new char[13, 11];
			List<Robot> bots = new List<Robot>();
			List<Bomb> bombs = new List<Bomb>();
			var bomb = new Bomb();
			bomb.position = 2;
			bomb.owner = 0;
			bomb.range = 3;
			bomb.countDown = 7;
			bombs.Add(bomb);

			bomb = new Bomb();
			bomb.position = 13;
			bomb.owner = 0;
			bomb.range = 3;
			bomb.countDown = 4;
			bombs.Add(bomb);

			for (int i = 0; i < 11; i++)
			{
				for (int j = 0; j < 13; j++)
				{
					map[j, i] = '.';
				}
			}

			var r = new Robot();
			r.owner = 0;
			var x = 0;
			var y = 0;
			r.position = BitState.GetBitIndex(x, y);
			r.param1 = 1;
			r.param2 = 3;

			bots.Add(r);

			var bgs = new BitState(map, bots, bombs, 0);

			var best = BitSolution.generateBestRandomSolution(bgs, 0, 20, 90);
			//var move = new Move(MoveType.Move, MoveDirection.Stay);
			var move = best.moves[0];

			bgs.getBot(0).getCommand(move);

			bgs.play(best, 0);
			bgs.score(0);

			Console.Error.WriteLine(timer.ElapsedMilliseconds);
		}
	}
}
