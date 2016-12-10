using System;
using System.Collections.Generic;

public class Solution
{
	public static Random rnd = new Random();
	public List<Move> moves;
	public double score;

	public static Solution generateSolution(GameState g, int depth)
	{
		var s = new Solution();
		s.moves = new List<Move>();

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
}
