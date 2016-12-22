using System;
using System.Collections.Generic;
using System.Diagnostics;

public class BitSolution
{
	class Node
	{
		public BitSolution solution;
		public BitSolution parent;
		public BitState gameState;

		public Node(BitSolution solution, BitSolution parent, BitState gameState)
		{
			this.solution = solution;
			this.parent = parent;
			this.gameState = gameState;
		}
	}

	public static Random rnd = new Random();
	public List<Move> moves = new List<Move>();
	public double score;

	public static BitSolution generateBestRandomSolution(BitState g, int playerId, int depth, int timeToRun)
	{
		var solutions = new List<BitSolution>();
		var timer = new Stopwatch();
		timer.Start();
		while (timer.ElapsedMilliseconds <= timeToRun)
		{
			solutions.Add(randomSolution(g.Clone(), playerId, depth));
		}
		timer.Stop();

		//Console.Error.WriteLine(solutions.Count);
		BitSolution best = null;
		double maxScore = Double.NegativeInfinity;
		foreach (BitSolution s in solutions)
		{
			if (s.score > maxScore)
			{
				maxScore = s.score;
				best = s;
			}
		}

		return best;
	}

	public BitSolution Clone()
	{
		var clone = new BitSolution();
		clone.moves = this.moves.ConvertAll(m => new Move(m.type, m.direction));

		return clone;
	}

	public override string ToString()
	{
		var s = "***\n";
		foreach (Move m in this.moves)
		{
			s += m.ToString() + "\n";
		}

		s += "Score " + this.score + "\n";
		s += "***\n";

		return s;
	}

	private static BitSolution randomSolution(BitState g, int playerId, int depth)
	{
		var s = new BitSolution();

		for (int d = 0; d < depth; d++)
		{
			var moves = g.getMoves(playerId);
			var move = moves[BitSolution.rnd.Next(moves.Count)];
			g.play(move, playerId);
			s.moves.Add(move);

			if (!g.getBot(playerId).isAlive)
			{
				break;
			}
		}

		s.score = g.score(playerId);
		return s;
	}

	public static BitSolution bfs(BitState g, int playerId, int timeToRun)
	{
		var timer = new Stopwatch();
		timer.Start();

		var open = new List<Node>();
		var closed = new List<BitSolution>();

		open.Add(new Node(new BitSolution(), null, g.Clone()));
		while (timer.ElapsedMilliseconds <= timeToRun && open.Count > 0)
		{
			var sol = open[0].solution;
			var gs = open[0].gameState;

			if (open[0].parent != null)
			{
				closed.Remove(open[0].parent);
			}

			open.RemoveAt(0);

			foreach (Move m in gs.getMoves(playerId))
			{
				var solClone = sol.Clone();
				var gsClone = gs.Clone();
				solClone.moves.Add(m);
				gsClone.play(m, playerId);

				if (gsClone.getBot(playerId).isAlive)
				{
					open.Add(new Node(solClone, sol, gsClone));
				}
			}

			closed.Add(sol);
		}
		timer.Stop();

		BitSolution best = null;
		double maxScore = Double.NegativeInfinity;
		Console.Error.WriteLine(closed.Count);
		foreach (var s in closed)
		{
			if (s.score > maxScore)
			{
				maxScore = s.score;
				best = s;
			}
		}

		Console.Error.WriteLine(best);
		return best;
	}
}