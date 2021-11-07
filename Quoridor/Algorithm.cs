using System;
using System.Collections.Generic;
using System.Linq;

namespace Quoridor
{
    class Algorithm
    {
        //Quoridor game = Quoridor.getInstance();

        static public readonly List<Vec2<int>> directions =
            new List<Vec2<int>>
            {
                new Vec2<int> (0, 1),
                new Vec2<int> (-1, 0),
                new Vec2<int> (0, -1),
                new Vec2<int> (1, 0)
            };
        static public int getWinDistance(Vec2<int> pos, Vec2<int> winDir)
        {
            int half = (9 - 1) / 2;
            return Math.Abs(winDir.x * (pos.x - (half * (winDir.x + 1))) + winDir.y * (pos.y - (half * (winDir.y + 1))));
        }

        static public List<Vec2<int>> getWays(Cell[,] board, Vec2<int> pos, Vec2<int> dir)
        {
            var next_cell = board[pos.x, pos.y];
            var ways = new List<Vec2<int>>();

            if (!next_cell.walls.Contains(new Vec2<int>(dir.y, dir.x)) && !board[next_cell.x + dir.y, next_cell.y + dir.x].Occupied)
            {
                ways.Add(new Vec2<int>(dir.y, dir.x));
            }
            if (!next_cell.walls.Contains(new Vec2<int>(dir.y * -1, dir.x * -1)) && !board[next_cell.x + dir.y * -1, next_cell.y + dir.x * -1].Occupied)
            {
                ways.Add(new Vec2<int>(dir.y * -1, dir.x * -1));
            }
            return ways;
        }

        static public List<Node> getNonWallNeighbours(Cell[,] board, float cost, Vec2<int> pos, int rotation)
        {
            var neighbours = new List<Node>();
            foreach (var direction in directions)
            {
                var dir = new Vec2<int>(direction.x * rotation, direction.y * rotation);
                if (board[pos.x, pos.y].walls.Contains(dir))
                    continue;

                var cellPos = new Vec2<int>(pos.x + dir.x, pos.y + dir.y);
                if (cellPos.x < 0 || cellPos.x >= 9 || cellPos.y < 0 || cellPos.y >= 9)
                    continue;

                var cell = board[cellPos.x, cellPos.y];

                if (!cell.Occupied)
                    neighbours.Add(new Node(cellPos, new Vec2<float>(cost, 0)));
                else
                {
                    var next_pos = cellPos + dir;
                    if (cell.walls.Contains(dir) || board[next_pos.x, next_pos.y].Occupied)
                    {
                        var ways = getWays(board, new Vec2<int>(cellPos.x, cellPos.y), dir);

                        if (ways.Count > 0 && ways.Count < 3)
                            foreach(var way in ways)
                                neighbours.Add(new Node(way + cellPos, new Vec2<float>(cost, 0)));
                        continue;
                    }
                    neighbours.Add(new Node(new Vec2<int>(next_pos.x, next_pos.y), new Vec2<float>(cost, 0)));

                }
            }


            return neighbours;
        }

        public struct Node
        {
            public Vec2<int> pos;
            public Vec2<float> value;
            public Node(Vec2<int> pos, Vec2<float> val)
            {
                this.pos = pos;
                value = val;
            }
        }
        static public List<Vec2<int>> aStar(Cell[,] board, Vec2<int> start, Vec2<int> winDir)
        {
            var node_path = new Dictionary<Vec2<int>, Vec2<int>?>();
            node_path[start] = null;
            Vec2<int>? end = null;
            int rot = winDir.x + winDir.y;

            var closed = new HashSet<Vec2<int>>();
            var opend = new List<Node>();
            opend.Add(new Node(start, new Vec2<float>(1, 0)));

            while (opend.Count > 0)
            {
                var curr_node = opend[0];
                opend.RemoveAt(0);

                closed.Add(curr_node.pos);

                if (getWinDistance(curr_node.pos, winDir) == 0)
                {
                    end = curr_node.pos;
                    break;
                }
                else
                {
                    var neighbours = getNonWallNeighbours(board, curr_node.value.x, curr_node.pos, rot);
                    for (int i = 0; i < neighbours.Count; i++)
                    {
                        var node = neighbours[i];
                        node.value.y = node.value.x + getWinDistance(node.pos, winDir);

                        if (closed.Contains(node.pos))
                            continue;

                        node_path[node.pos] = curr_node.pos;
                        if (opend.Contains(node) && opend.Find(x => x.pos.Equals(node.pos)).value.x <= node.value.x)
                            continue;

                        opend.Add(node);
                        opend = opend.OrderBy(x => x.value.y).ToList();
                    }
                }
            }
            if(end == null)
                return null;

            var path = new List<Vec2<int>>();
            path.Add((Vec2<int>)end);

            var pos = end;
            while (pos != null)
            {
                var next_pos = node_path[(Vec2<int>)pos];
                if (next_pos == null)
                    break;

                path.Add((Vec2<int>)next_pos);
                pos = next_pos;
            }
            path.Reverse();
            return path;
        }

        static public Quoridor.TurnStatus applyAction(Board board, int player, Quoridor.MoveChoice choice, dynamic[] args)
        {
            var turnStatus = Quoridor.TurnStatus.STARTED;

            if (choice == Quoridor.MoveChoice.MOVE)
            {
                turnStatus = board.movePiece(player, args[0]).result;
            }
            else if (choice == Quoridor.MoveChoice.JUMP)
            {
                turnStatus = board.jumpOver(player, args[0]).result;
            }
            else if (choice == Quoridor.MoveChoice.WALL)
            {
                turnStatus = board.placeWall(args[0], args[1]).result;
            }

            return turnStatus;
        }
        static public (Quoridor.MoveChoice, dynamic[]) miniMax(int maxDepth, int player, Board board)
        {
            (Quoridor.MoveChoice, dynamic[]) nextMove = (Quoridor.MoveChoice.NONE, null);
            var game = Quoridor.getInstance();
            var player1 = game.players[player];
            var player2 = game.players[Math.Abs(player-1)];

            float maximize(ref Vec2<float> alphaBeta, Board board, int depth)
            {
                if (depth == 0)
                    return board.getScore(player); // todo

                var actions = board.getActions(player);
                var best = -999f;
                float score;
                foreach(var action in actions)
                {
                    var new_board = board.Clone();
                    var turn = applyAction(new_board, player, action.Item1, action.Item2);
                    if (turn == Quoridor.TurnStatus.WON)
                    {
                        if (depth == maxDepth)
                            nextMove = action;
                        return new_board.getScore(player);
                    }
                    else if (turn == Quoridor.TurnStatus.WRONG)
                        continue;

                    score = minimize(ref alphaBeta, new_board, depth - 1);

                    best = Math.Max(score, best);
                    alphaBeta.x = Math.Max(alphaBeta.x, best);

                    if (depth == maxDepth && alphaBeta.x == score)
                        nextMove = action;

                    if (alphaBeta.x >= alphaBeta.y)
                        return best;
                }

                return best;
            }

            float minimize(ref Vec2<float> alphaBeta, Board board,int depth)
            {
                if (depth == 0)
                    return board.getScore(player); //todo

                var actions = board.getActions(Math.Abs(player - 1));
                var best = 999f;
                float score;
                foreach (var action in actions)
                {
                    var new_board = board.Clone();
                    var turn = applyAction(new_board, Math.Abs(player - 1), action.Item1, action.Item2);
                    if (turn == Quoridor.TurnStatus.WON)
                        return new_board.getScore(player);
                    else if (turn == Quoridor.TurnStatus.WRONG)
                        continue;

                    score = maximize(ref alphaBeta, new_board, depth - 1);

                    best = Math.Min(score, best);
                    alphaBeta.y = Math.Min(alphaBeta.y, best);

                    if (alphaBeta.x >= alphaBeta.y)
                        return best;
                }

                return best;
            }

            var alphaBeta = new Vec2<float>(-999f, 999f);
            maximize(ref alphaBeta, board, maxDepth);

            return nextMove;
        }
    }
}
