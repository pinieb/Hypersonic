namespace Hypersonic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class BitState
    {
        public BitArray[] boxes = new BitArray[Constants.BoxTypes];
        public BitArray[] items = new BitArray[Constants.ItemTypes];
        //public BitArray[] playerMaps = new BitArray[MaxPlayers];
        public BitArray[] bombMap = new BitArray[Constants.MaxPlayers];
        public List<Robot> bots;
        public List<Bomb> bombs;
        public int turnNumber;
        private double lastComputedScore;
        private int lastScoreComputation;

        private BitState()
        {
        }

        public BitState(char[,] map, List<Robot> bots, List<Bomb> bombs, int turnNumber) : this()
        {
            this.turnNumber = turnNumber;
            this.bots = bots;
            this.bombs = bombs;

            for (int i = 0; i < Constants.BoxTypes; i++)
            {
                boxes[i] = new BitArray(Constants.PlayableSquares);
            }

            for (int i = 0; i < Constants.ItemTypes; i++)
            {
                items[i] = new BitArray(Constants.PlayableSquares);
            }

            for (int i = 0; i < Constants.MaxPlayers; i++)
            {
                //this.playerMaps[i] = new BitArray(PlayableSquares);
                bombMap[i] = new BitArray(Constants.PlayableSquares);
            }

            foreach (Bomb bomb in bombs)
            {
                bombMap[bomb.owner][bomb.position] = true;
            }

            int index = 0;
            for (int j = 0; j < Constants.BoardHeight; j++)
            {
                for (int i = 0; i < Constants.BoardWidth; i++)
                {
                    if (i % 2 == 1 && j % 2 == 1)
                    {
                        // skip the walls
                        continue;
                    }

                    if (map[i, j] == Constants.MAP_BOX)
                    {
                        boxes[0][GetBitIndex(i, j)] = true;
                    }
                    else if (map[i, j] == Constants.MAP_BOX_RANGE)
                    {
                        boxes[1][GetBitIndex(i, j)] = true;
                    }
                    else if (map[i, j] == Constants.MAP_BOX_BOMB)
                    {
                        boxes[2][GetBitIndex(i, j)] = true;
                    }
                    else if (map[i, j] == Constants.MAP_ITEM_RANGE)
                    {
                        items[0][GetBitIndex(i, j)] = true;
                    }
                    else if (map[i, j] == Constants.MAP_ITEM_BOMB)
                    {
                        items[1][GetBitIndex(i, j)] = true;
                    }

                    index++;
                }
            }
        }

        public BitState Clone()
        {
            var clone = new BitState
            {
                bots = bots.ConvertAll(e => e.Clone()),
                bombs = bombs.ConvertAll(b => b.Clone())
            };

            for (int i = 0; i < Constants.BoxTypes; i++)
            {
                clone.boxes[i] = new BitArray(boxes[i]);
            }

            for (int i = 0; i < Constants.ItemTypes; i++)
            {
                clone.items[i] = new BitArray(items[i]);
            }

            for (int i = 0; i < Constants.MaxPlayers; i++)
            {
                //clone.playerMaps[i] = new BitArray(this.playerMaps[i]);
                clone.bombMap[i] = new BitArray(bombMap[i]);
            }

            clone.turnNumber = turnNumber;
            clone.lastComputedScore = lastComputedScore;
            clone.lastScoreComputation = lastScoreComputation;

            return clone;
        }

        public Robot getBot(int id)
        {
            return bots.FirstOrDefault(x => x.owner == id);
        }

        public void play(BitSolution s, int playerId)
        {
            foreach (Move m in s.moves)
            {
                play(m, playerId);
            }
        }

        public void play(Move move, int playerId)
        {
            tick();

            var bot = getBot(playerId);

            handleExplosions();

            // place bomb if necessary
            if (move.type == MoveType.Bomb)
            {
                bombMap[playerId][bot.position] = true;
                bombs.Add(bot.makeBomb());
            }

            // move
            int newPos = -1;
            switch (move.direction)
            {
                case MoveDirection.Right:
                    newPos = bot.right();
                    break;

                case MoveDirection.Left:
                    newPos = bot.left();
                    break;

                case MoveDirection.Up:
                    newPos = bot.up();
                    break;

                case MoveDirection.Down:
                    newPos = bot.down();
                    break;
            }

            if (newPos != -1)
            {
                //this.playerMaps[playerId][newPos] = true;
                //this.playerMaps[playerId][bot.position] = false;
                bot.position = newPos;
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
            for (int j = 0; j < Constants.BoardHeight; j++)
            {
                for (int i = 0; i < Constants.BoardWidth; i++)
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

            for (int j = 0; j < Constants.BoardHeight && index >= 0; j++)
            {
                for (int i = 0; i < Constants.BoardWidth && index >= 0; i++)
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
            for (int j = 0; j < Constants.BoardHeight; j++)
            {
                for (int i = 0; i < Constants.BoardWidth; i++)
                {
                    if (i % 2 == 0 || j % 2 == 0)
                    {
                        if (boxes[0][index])
                        {
                            Console.Error.Write(Constants.MAP_BOX);
                        }
                        else if (boxes[1][index])
                        {
                            Console.Error.Write(Constants.MAP_BOX_RANGE);
                        }
                        else if (boxes[2][index])
                        {
                            Console.Error.Write(Constants.MAP_BOX_BOMB);
                        }
                        else if (items[0][index])
                        {
                            Console.Error.Write(Constants.MAP_ITEM_RANGE);
                        }
                        else if (items[1][index])
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

        public void handleExplosions()
        {
            var timer = new Stopwatch();
            timer.Start();
            var movesTimer = new Stopwatch();
            var exploded = new List<Bomb>();
            var destroyedBoxes = new List<BitArray>();
            var destroyedItems = new List<BitArray>();

            List<Bomb> toExplode = bombs.Where(bomb => bomb.countDown <= 0).ToList();

            while (toExplode.Count > 0)
            {
                var b = toExplode[0];
                var owner = getBot(b.owner);
                if (exploded.Contains(b))
                {
                    toExplode.RemoveAt(0);
                    continue;
                }

                foreach (Robot r in bots)
                {
                    if (r.position == b.position)
                    {
                        r.hitBy = b;
                        r.isAlive = false;
                    }
                }

                bool continueLeft = true;
                bool continueRight = true;
                bool continueUp = true;
                bool continueDown = true;
                for (int i = 0; i < b.range; i++)
                {
                    if (continueLeft)
                    {
                        movesTimer.Start();
                        var l = BitMaps.bombLeft[i, b.position];
                        movesTimer.Stop();

                        var hits = getBombHits(l);
                        if (hits.Count > 0)
                        {
                            toExplode.AddRange(hits);
                            continueLeft = false;
                        }

                        var boxHits = getBoxHits(l);
                        if (boxHits.Count > 0)
                        {
                            if (owner != null)
                            {
                                owner.boxesDestroyed += boxHits.Count;
                            }
                            destroyedBoxes.AddRange(boxHits);
                            continueLeft = false;
                        }

                        var itemHits = getItemHits(l);
                        if (itemHits.Count > 0)
                        {
                            destroyedItems.AddRange(itemHits);
                            continueLeft = false;
                        }

                        foreach (var bot in getPlayerHits(l))
                        {
                            bot.hitBy = b;
                            bot.isAlive = false;
                        }
                    }

                    if (continueRight)
                    {
                        movesTimer.Start();
                        var r = BitMaps.bombRight[i, b.position];
                        movesTimer.Stop();
                        var hits = getBombHits(r);
                        if (hits.Count > 0)
                        {
                            toExplode.AddRange(hits);
                            continueRight = false;
                        }

                        var boxHits = getBoxHits(r);
                        if (boxHits.Count > 0)
                        {
                            if (owner != null)
                            {
                                owner.boxesDestroyed += boxHits.Count;
                            }
                            destroyedBoxes.AddRange(boxHits);
                            continueRight = false;
                        }

                        var itemHits = getItemHits(r);
                        if (itemHits.Count > 0)
                        {
                            destroyedItems.AddRange(itemHits);
                            continueRight = false;
                        }

                        foreach (var bot in getPlayerHits(r))
                        {
                            bot.hitBy = b;
                            bot.isAlive = false;
                        }
                    }

                    if (continueUp)
                    {
                        movesTimer.Start();
                        var u = BitMaps.bombUp[i, b.position];
                        movesTimer.Stop();
                        var hits = getBombHits(u);
                        if (hits.Count > 0)
                        {
                            toExplode.AddRange(hits);
                            continueUp = false;
                        }

                        var boxHits = getBoxHits(u);
                        if (boxHits.Count > 0)
                        {
                            if (owner != null)
                            {
                                owner.boxesDestroyed += boxHits.Count;
                            }
                            destroyedBoxes.AddRange(boxHits);
                            continueUp = false;
                        }

                        var itemHits = getItemHits(u);
                        if (itemHits.Count > 0)
                        {
                            destroyedItems.AddRange(itemHits);
                            continueUp = false;
                        }

                        foreach (var bot in getPlayerHits(u))
                        {
                            bot.hitBy = b;
                            bot.isAlive = false;
                        }
                    }

                    if (continueDown)
                    {
                        movesTimer.Start();
                        var d = BitMaps.bombDown[i, b.position];
                        movesTimer.Stop();
                        var hits = getBombHits(d);
                        if (hits.Count > 0)
                        {
                            toExplode.AddRange(hits);
                            continueDown = false;
                        }

                        var boxHits = getBoxHits(d);
                        if (boxHits.Count > 0)
                        {
                            if (owner != null)
                            {
                                owner.boxesDestroyed += boxHits.Count;
                            }
                            destroyedBoxes.AddRange(boxHits);
                            continueDown = false;
                        }

                        var itemHits = getItemHits(d);
                        if (itemHits.Count > 0)
                        {
                            destroyedItems.AddRange(itemHits);
                            continueDown = false;
                        }

                        foreach (var bot in getPlayerHits(d))
                        {
                            bot.hitBy = b;
                            bot.isAlive = false;
                        }
                    }
                }

                toExplode.RemoveAt(0);
                exploded.Add(b);
            }

            foreach (Bomb b in exploded)
            {
                bombs.Remove(b);

                foreach (Robot r in bots)
                {
                    if (b.owner == r.owner)
                    {
                        r.bombs++;
                    }
                }

                bombMap[b.owner][b.position] = false;
            }

            foreach (BitArray b in destroyedBoxes)
            {
                for (int i = 0; i < Constants.BoxTypes; i++)
                {
                    var temp = boxes[i];
                    temp.And(b);
                    boxes[i].Xor(b);
                    if (i != 0)
                    {
                        items[i - 1].Or(temp);
                    }
                }
            }

            foreach (BitArray i in destroyedItems)
            {
                foreach (BitArray itemMap in items)
                {
                    itemMap.Xor(i);
                }
            }

            timer.Stop();
            //Console.Error.WriteLine("{0} ticks spent generating moves", movesTimer.ElapsedTicks);
            //Console.Error.WriteLine("{0} ticks total", timer.ElapsedTicks);
        }

        public List<Move> getMoves(int playerId)
        {
            var bot = this.getBot(playerId);
            var moves = new List<Move>();

            var directions = new BitArray[] { BitMaps.moveUp[bot.position], BitMaps.moveDown[bot.position], BitMaps.moveLeft[bot.position], BitMaps.moveRight[bot.position] };

            for (int i = 0; i < 4; i++)
            {
                if (directions[i].Cast<bool>().Where(x => x == true).Count() != 2)
                {
                    continue;
                }

                if (getBombHits(directions[i]).Count > 0)
                {
                    continue;
                }

                if (getBoxHits(directions[i]).Count > 0)
                {
                    continue;
                }

                if (bot.bombs > 0)
                {
                    moves.Add(new Move(MoveType.Bomb, (MoveDirection)i));
                }

                moves.Add(new Move(MoveType.Move, (MoveDirection)i));
            }


            if (bot.bombs > 0)
            {
                moves.Add(new Move(MoveType.Bomb, MoveDirection.Stay));
            }

            moves.Add(new Move(MoveType.Move, MoveDirection.Stay));

            return moves;
        }

        public List<Move> getMoves2(int playerId)
        {
            var moves = new List<Move>();
            var bot = getBot(playerId);

            var directions = new int[] { bot.up(), bot.down(), bot.left(), bot.right() };

            for (int i = 0; i < 4; i++)
            {
                if (directions[i] == -1)
                {
                    continue;
                }

                bool hitBomb = false;
                for (int p = 0; p < Constants.MaxPlayers; p++)
                {
                    // check player bombs
                    if (bombMap[p][directions[i]])
                    {
                        hitBomb = true;
                    }
                }

                if (hitBomb)
                {
                    continue;
                }

                bool hitBox = false;
                for (int p = 0; p < Constants.BoxTypes; p++)
                {
                    // check player bombs
                    if (boxes[p][directions[i]])
                    {
                        hitBox = true;
                    }
                }

                if (hitBox)
                {
                    continue;
                }

                if (bot.bombs > 0)
                {
                    moves.Add(new Move(MoveType.Bomb, (MoveDirection)i));
                }

                moves.Add(new Move(MoveType.Move, (MoveDirection)i));
            }

            if (bot.bombs > 0)
            {
                moves.Add(new Move(MoveType.Bomb, MoveDirection.Stay));
            }

            moves.Add(new Move(MoveType.Move, MoveDirection.Stay));

            return moves;
        }

        private List<Bomb> getBombHits(BitArray mask)
        {
            var hits = new List<Bomb>();

            for (int p = 0; p < Constants.MaxPlayers; p++)
            {
                // check player bombs
                var hit = new BitArray(mask).And(bombMap[p]);
                if (hit.Cast<bool>().Contains(true))
                {
                    // find bombs that got hit
                    hits.AddRange(bombs.Where(b => b.position == getPosition(hit)));
                }
            }

            return hits;
        }

        private List<BitArray> getBoxHits(BitArray mask)
        {
            var hits = new List<BitArray>();

            for (int p = 0; p < Constants.BoxTypes; p++)
            {
                var hit = new BitArray(mask).And(boxes[p]);
                if (hit.Cast<bool>().Contains(true))
                {
                    hits.Add(hit);
                    return hits;
                }
            }

            return hits;
        }

        private List<BitArray> getItemHits(BitArray mask)
        {
            var hits = new List<BitArray>();

            for (int p = 0; p < Constants.ItemTypes; p++)
            {
                var hit = new BitArray(mask).And(items[p]);
                if (hit.Cast<bool>().Contains(true))
                {
                    hits.Add(hit);
                    return hits;
                }
            }

            return hits;
        }

        private List<Robot> getPlayerHits(BitArray mask)
        {
            var hits = new List<Robot>();

            foreach (Robot r in bots)
            {
                if (mask[r.position])
                {
                    hits.Add(r);
                }
            }

            return hits;
        }

        private void tick()
        {
            foreach (Bomb b in bombs)
            {
                b.countDown--;
            }

            turnNumber++;
        }

        private int getPosition(BitArray array)
        {
            int[] intArray = (int[])array.GetType().GetField("m_array", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(array);

            for (var i = 0; i < intArray.Length; i++)
            {
                var b = intArray[i];
                if (b != 0)
                {
                    var pos = (i << 5) + 31;
                    for (int bit = 31; bit >= 0; bit--)
                    {
                        if ((b & (1 << bit)) != 0)
                            return pos;

                        pos--;
                    }

                    return pos;
                }
            }

            return -1;
        }

        private static BitArray GenerateMoveMap(int position, MoveDirection direction, int range)
        {
            var map = new BitArray(Constants.PlayableSquares);

            if (direction == MoveDirection.Stay)
            {
                return map;
            }

            map[position] = true;

            var mapPos = GetMapIndex(position);

            switch (direction)
            {
                case MoveDirection.Right:
                    if (mapPos.Item1 + range < Constants.BoardWidth && mapPos.Item2 % 2 == 0)
                    {
                        map[position + range] = true;
                    }
                    break;

                case MoveDirection.Left:
                    if (mapPos.Item1 - range >= 0 && mapPos.Item2 % 2 == 0)
                    {
                        map[position - range] = true;
                    }
                    break;

                case MoveDirection.Up:
                    if (mapPos.Item2 - range >= 0 && mapPos.Item1 % 2 == 0)
                    {
                        map[GetBitIndex(mapPos.Item1, mapPos.Item2 - range)] = true;
                    }
                    break;

                case MoveDirection.Down:
                    if (mapPos.Item2 + range < Constants.BoardHeight && mapPos.Item1 % 2 == 0)
                    {
                        map[GetBitIndex(mapPos.Item1, mapPos.Item2 + range)] = true;
                    }
                    break;

                default:
                    break;
            }

            return map;
        }

        public double score(int playerId)
        {
            //var bot = getBot(playerId);
            if (lastScoreComputation == turnNumber)
            {
                return lastComputedScore;
            }

            var lookahead = Clone();
            var stay = new Move(MoveType.Move, MoveDirection.Stay);
            for (int i = 0; i < 8; i++)
            {
                lookahead.play(stay, playerId);
            }

            var bot = lookahead.getBot(playerId);

            if (!bot.isAlive)
            {
                //Console.Error.WriteLine("Hit by bomb @ {0}", bot.hitBy.position);
                lastScoreComputation = turnNumber;
                lastComputedScore = -8000000;
                return lastComputedScore;
            }

            double s = 0;

            foreach (var b in bots)
            {
                if (b.owner == playerId)
                {
                    s += 7 * b.boxesDestroyed;
                }
                else
                {
                    s -= 7 * b.boxesDestroyed;
                }
            }

            s += bot.bombRange + bot.maxBombs;

            lastScoreComputation = turnNumber;
            lastComputedScore = s;
            return lastComputedScore;
        }
    }
}
