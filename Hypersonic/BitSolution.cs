namespace Hypersonic
{
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
            BitSolution best = null;
            double maxScore = double.NegativeInfinity;
            int count = 0;

            var timer = new Stopwatch();
            timer.Start();
            while (timer.ElapsedMilliseconds <= timeToRun)
            {
                //var time = timer.ElapsedMilliseconds;
                //Console.Error.WriteLine("Time elapsed: {0} ms", time);
                var solution = randomSolution(g.Clone(), playerId, depth);
                if (solution.score > maxScore)
                {
                    maxScore = solution.score;
                    best = solution;
                }
                count++;
                //time = timer.ElapsedMilliseconds;
                //Console.Error.WriteLine("Time elapsed: {0} ms", time);
            }
            timer.Stop();

            Console.Error.WriteLine("{0} solutions considered", count);

            return best;
        }

        public BitSolution Clone()
        {
            var clone = new BitSolution
            {
                moves = moves.ConvertAll(m => new Move(m.type, m.direction))
            };

            return clone;
        }

        public override string ToString()
        {
            var s = "***\n";
            foreach (Move m in moves)
            {
                s += m.ToString() + "\n";
            }

            s += "Score " + score + "\n";
            s += "***\n";

            return s;
        }

        private static BitSolution randomSolution(BitState g, int playerId, int depth)
        {
            var movesTimer = new Stopwatch();
            //var playTimer = new Stopwatch();
            //var scoreTimer = new Stopwatch();

            movesTimer.Start();
            var s = new BitSolution();
            movesTimer.Stop();
            //Console.Error.WriteLine("Time to construct BitSolution: {0}", movesTimer.ElapsedMilliseconds);

            for (int d = 0; d < depth; d++)
            {
                //movesTimer.Start();
                var moves = g.getMoves(playerId);
                //movesTimer.Stop();
                var move = moves[rnd.Next(moves.Count)];
                //playTimer.Start();
                g.play(move, playerId);
                //playTimer.Stop();
                s.moves.Add(move);

                if (!g.getBot(playerId).isAlive)
                {
                    break;
                }
            }

            //scoreTimer.Start();
            s.score = g.score(playerId);
            //scoreTimer.Stop();

            //Console.Error.WriteLine("Moves: {0} ms", movesTimer.ElapsedMilliseconds);
            //Console.Error.WriteLine("Play: {0} ms", playTimer.ElapsedMilliseconds);
            //Console.Error.WriteLine("Score: {0} ms", scoreTimer.ElapsedMilliseconds);
            //Console.Error.WriteLine();

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
            double maxScore = double.NegativeInfinity;
            foreach (var s in closed)
            {
                if (s.score > maxScore)
                {
                    maxScore = s.score;
                    best = s;
                }
            }

            return best;
        }
    }
}
