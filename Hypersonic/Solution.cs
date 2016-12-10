using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Solution
{
	class Node
	{
		public Solution solution;
		public Solution parent;
		public GameState gameState;

		public Node(Solution solution, Solution parent, GameState gameState)
		{
			this.solution = solution;
			this.parent = parent;
			this.gameState = gameState;
		}
	}

	public static Random rnd = new Random();
	public List<Move> moves = new List<Move>();
	public double score;

	public static Solution generateBestRandomSolution(GameState g, int depth, int timeToRun)
	{
		var solutions = new List<Solution>();
		var timer = new Stopwatch();
		timer.Start();
		while (timer.ElapsedMilliseconds <= timeToRun)
		{
			solutions.Add(randomSolution(g.Clone(), depth));
		}
		timer.Stop();

		Console.Error.WriteLine(solutions.Count);
		Solution best = null;
		double maxScore = Double.NegativeInfinity;
		foreach (Solution s in solutions)
		{
			if (s.score > maxScore)
			{
				maxScore = s.score;
				best = s;
			}
		}

		return best;
	}

	public Solution Clone()
	{
		var clone = new Solution();
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

	private static Solution randomSolution(GameState g, int depth)
	{
		var s = new Solution();

		for (int d = 0; d < depth; d++)
		{
			var moves = g.self.getMoves(g.map);
			var move = moves[Solution.rnd.Next(moves.Count)];
			g.play(move);
			s.moves.Add(move);

			if (!g.self.isAlive)
			{
				break;
			}
		}

		s.score = g.score();
		return s;
	}

	public static Solution bfs(GameState g, int timeToRun)
	{
		var timer = new Stopwatch();
		timer.Start();

		var open = new List<Node>();
		var closed = new List<Solution>();

		open.Add(new Node(new Solution(), null, g.Clone()));
		while (timer.ElapsedMilliseconds <= timeToRun && open.Count > 0)
		{
			var sol = open[0].solution;
			var gs = open[0].gameState;

			if (open[0].parent != null)
			{
				closed.Remove(open[0].parent);
			}

			open.RemoveAt(0);

			foreach (Move m in gs.self.getMoves(gs.map))
			{
				var solClone = sol.Clone();
				var gsClone = gs.Clone();
				solClone.moves.Add(m);
				gsClone.play(m);

				if (gsClone.self.isAlive)
				{
					open.Add(new Node(solClone, sol, gsClone));
				}
			}

			closed.Add(sol);
		}
		timer.Stop();

		Solution best = null;
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
