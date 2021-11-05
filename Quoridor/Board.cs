using System.Collections.Generic;
using System;
using System.Linq;

//TODO: ASTAR + DIRECTION_HEURISTICS

namespace Quoridor
{
    class Board { 

        public int size;
        public Cell[,] board;
        public Wall[,] walls;
        public Piece[] players;

        Quoridor game;

        //starting sides
        static private readonly List<Vec2<int>> directions =
            new List<Vec2<int>>
            {
                new Vec2<int> (0, 1),
                new Vec2<int> (1, 0),
                new Vec2<int> (0, -1),
                new Vec2<int> (-1, 0)
            };

        public Board(int playerCount, int size)
        {
            game = Quoridor.getInstance();

            this.size = size;
            board = createBoard();
            walls = new Wall[size - 1, size - 1];//fillCenterWalls();
            players = createPieces(playerCount);
            game.players = players;
        }
        private void updatePlayerPos(ref Piece playerPiece, Vec2<int> nextPos)
        {
            int x = playerPiece.x,
                y = playerPiece.y;

            int next_x = nextPos.x,
                next_y = nextPos.y;

            playerPiece.x = next_x;
            playerPiece.y = next_y;

            board[next_x, next_y].player = playerPiece;
            board[x, y].player = null;
        }

        static public List<Vec2<int>> getWays(Cell[,] board, Vec2<int> pos, Vec2<int> dir)
        {
            var next_cell = board[pos.x, pos.y];
            var ways = new List<Vec2<int>>();

            if (!next_cell.walls.Contains(new Vec2<int>(dir.y, dir.x)) && board[next_cell.x + dir.y, next_cell.y + dir.x].player == null)
            {
                ways.Add(new Vec2<int>(dir.y, dir.x));
            }
            if (!next_cell.walls.Contains(new Vec2<int>(dir.y * -1, dir.x * -1)) && board[next_cell.x + dir.y * -1, next_cell.y + dir.x * -1].player == null)
            {
                ways.Add(new Vec2<int>(dir.y * -1, dir.x * -1));
            }
            return ways;
        }

        public Quoridor.Turn movePiece(int player, Vec2<int> coords)
        {
            var status = Quoridor.TurnStatus.MOVED;
            int next_x = coords.x,
                next_y = coords.y;

            var playerPiece = players[player];

            int x = playerPiece.x,
                y = playerPiece.y;

            var dir = new Vec2<int>(next_x - x, next_y - y);
            if (Math.Abs(dir.x) + Math.Abs(dir.y) != 1)
                return new Quoridor.Turn(
                    Quoridor.TurnStatus.WRONG,
                    Quoridor.MoveChoice.MOVE,
                    coords
                    );

            if (next_x >= size || next_x < 0 || next_y >= size || next_y < 0)
                return new Quoridor.Turn(
                    Quoridor.TurnStatus.WRONG,
                    Quoridor.MoveChoice.MOVE,
                    coords
                    );

            if (board[x, y].walls.Contains(dir) || board[next_x, next_y].Occupied)
                return new Quoridor.Turn(
                    Quoridor.TurnStatus.WRONG,
                    Quoridor.MoveChoice.MOVE,
                    coords
                    );

            // var next_cell = board[next_x, next_y];
            
            // if (next_cell.player != null)
            // {
            //     if (next_cell.walls.Contains(dir) || board[next_x + dir.x, next_y+ dir.y].player != null)
            //     {
            //         var ways = getWays(new Vec2<int>(next_x, next_y), dir);
               
            //         if (ways.Count == 2)
            //             return new Quoridor.Turn(
            //                 Quoridor.TurnStatus.MOVED,
            //                 Quoridor.MoveChoice.MOVE,
            //                 Quoridor.MoveChoice.JUMP,
            //                 new Vec2<int>(next_cell.x, next_cell.y),
            //                 ways
            //                 );
               
            //         else if (ways.Count == 1)
            //         {
            //             next_x += ways[0].x;
            //             next_y += ways[0].y;
               
            //             updatePlayerPos(ref playerPiece, new Vec2<int>(next_x, next_y));
               
            //             status = Quoridor.TurnStatus.JUMPED;
            //             if (hasWon(playerPiece))
            //                 status = Quoridor.TurnStatus.WON;
               
            //             return new Quoridor.Turn(
            //                 status,
            //                 Quoridor.MoveChoice.MOVE,
            //                 Quoridor.MoveChoice.NONE,
            //                 new Vec2<int>(next_x, next_y)
            //                 );
            //         }
               
            //         return new Quoridor.Turn(
            //             Quoridor.TurnStatus.WRONG,
            //             Quoridor.MoveChoice.MOVE,
            //             Quoridor.MoveChoice.NONE,
            //             new Vec2<int>(next_x, next_y)
            //             );
            //     }
            //     else
            //     {
            //         next_x += dir.x; 
            //         next_y += dir.y;
            //     }
            // }
            
            updatePlayerPos(ref playerPiece, coords);
            
            if (hasWon(playerPiece))
                status = Quoridor.TurnStatus.WON;

            return new Quoridor.Turn(
                status,
                Quoridor.MoveChoice.MOVE,
                coords
                ); 
        }
        
        private bool hasWon(Piece player)
        {
            int half = (this.size - 1) / 2;
            return player.x * Math.Abs(player.winDir.x) == (player.winDir.x + 1) * Math.Abs(player.winDir.x) * half
                && player.y * Math.Abs(player.winDir.y) == (player.winDir.y + 1) * Math.Abs(player.winDir.y) * half;
        }

        public Quoridor.Turn jumpOver(int player, Vec2<int> coords)
        {
            // var crossingPos = turn.pos;
            // var ways = turn.ways;
            // 
            // var next_cell = new Vec2<int>(crossingPos.x + dir.x, crossingPos.y + dir.y);
            // if (!ways.Contains(dir))
            //     return new Quoridor.Turn(
            //         Quoridor.TurnStatus.WRONG,
            //         Quoridor.MoveChoice.JUMP,
            //         Quoridor.MoveChoice.NONE,
            //         new Vec2<int>(next_cell.x, next_cell.y)
            //         );

            var possibleJumps = new List<Vec2<int>>();
            var playerPiece = players[player];
            int x = playerPiece.x, 
                y = playerPiece.y;

            foreach (var dir in directions) {
                if (!board[x + dir.x, y + dir.y].Occupied
                    || board[x, y].walls.Contains(dir)) continue;

                var oponentPos = new Vec2<int>(playerPiece.x + dir.x, playerPiece.y + dir.y);
                var endPos = oponentPos + dir;
                if (!(board[endPos.x, endPos.y].Occupied || board[oponentPos.x, oponentPos.y].walls.Contains(dir)))
                {
                    possibleJumps.Add(endPos);
                    continue;
                }

                var perpDir = new Vec2<int>(Math.Abs(dir.y), Math.Abs(dir.x));
                var cell1 = oponentPos - perpDir;
                if (!(board[cell1.x, cell1.y].Occupied || board[oponentPos.x, oponentPos.y].walls.Contains(-perpDir)))
                    possibleJumps.Add(cell1);

                var cell2 = oponentPos + perpDir;
                if (!(board[cell2.x, cell2.y].Occupied || board[oponentPos.x, oponentPos.y].walls.Contains(perpDir)))
                    possibleJumps.Add(cell2);
            }

            if (!possibleJumps.Contains(coords))
                return new Quoridor.Turn(
                    Quoridor.TurnStatus.WRONG,
                    Quoridor.MoveChoice.JUMP,
                    coords
                    );

            updatePlayerPos(ref players[player], coords);

            var status = Quoridor.TurnStatus.JUMPED;
            if (hasWon(players[player]))
                status = Quoridor.TurnStatus.WON;

            return new Quoridor.Turn(
                    status,
                    Quoridor.MoveChoice.JUMP,
                    coords
                    );
        }

        public Quoridor.Turn placeWall(Vec2<int> target, bool isVertical)
        {
            if (players[Quoridor.currentPlayer].wallsLeft == 0 || !(0 <= target.x && target.x < size-1) ||
                !(0 <= target.y && target.y < size-1))
                return new Quoridor.Turn(
                    Quoridor.TurnStatus.WRONG,
                    Quoridor.MoveChoice.WALL,
                    target
                    );

            if (isVertical)
            {
                for (int i = target.y - 1; i <= target.y + 1; i++)
                {
                    if (i >= 9 - 1 || i < 0)
                        continue;
                    var wall = this.walls[target.x, i];
                    if (wall != null && wall.isVertical)
                        return new Quoridor.Turn(
                            Quoridor.TurnStatus.WRONG,
                            Quoridor.MoveChoice.WALL,
                            target
                            );
                }
            }
            else
            {
                for (int i = target.x - 1; i <= target.x + 1; i++)
                {
                    if (i >= 9 - 1 || i < 0)
                        continue;
                    var wall = this.walls[i, target.y];
                    if (wall != null && !wall.isVertical)
                        return new Quoridor.Turn(
                            Quoridor.TurnStatus.WRONG,
                            Quoridor.MoveChoice.WALL,
                            target
                            );
                }
            }

            this.walls[target.x, target.y] = new Wall(target.x, target.y, isVertical);
            List<Cell> saved_cells = new List<Cell>();

            for (int i = target.x; i <= target.x + 1; i++)
                for (int j = target.y; j <= target.y + 1; j++)
                {
                    int dirX = Convert.ToInt32(isVertical) * ((i - target.x) * -2 + 1);
                    int dirY = Convert.ToInt32(!isVertical) * ((j - target.y) * -2 + 1);

                    saved_cells.Add(this.board[i, j].deepCopy());
                    this.board[i, j].walls.Add(new Vec2<int>(dirX, dirY));
                }

            bool isPassable = true;
            for (int i = 0; i < players.Length; i++)
            {
                List<Vec2<int>> path = aStar(board, new Vec2<int>(players[i].x, players[i].y), players[i].winDir);
                if (path == null || !(path.Count > 0))
                    isPassable = false;
            }

            if (isPassable)
            {
                players[Quoridor.currentPlayer].wallsLeft--;
                return new Quoridor.Turn(
                    Quoridor.TurnStatus.PLACED,
                    Quoridor.MoveChoice.WALL,
                    target
                    );
            }
            else
            {
                int index = 0;
                for (int i = target.x - 1; i <= target.x; i++)
                    for (int j = target.y - 1; j <= target.y; j++)
                    {
                        this.board[i, j] = saved_cells[index];
                        index++;
                    }
                return new Quoridor.Turn(
                    Quoridor.TurnStatus.WRONG,
                    Quoridor.MoveChoice.WALL,
                    target
                    );
            }
        }

        private Cell[,] createBoard()
        {
            Cell[,] mat = new Cell[size, size];
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    var walls = new HashSet<Vec2<int>>();
                    if (i == 0)
                        walls.Add(new Vec2<int>(-1, 0));
                    if (j == 0)
                        walls.Add(new Vec2<int>(0, -1));
                    if (i == this.size - 1)
                        walls.Add(new Vec2<int>(1, 0));
                    if (j == this.size - 1)
                        walls.Add(new Vec2<int>(0, 1));

                    mat[i, j] = new Cell(i, j, walls);
                }
            }
            return mat;
        }

        private Wall[,] fillCenterWalls()
        {
            Wall[,] walls = new Wall[this.size - 1, this.size - 1];
            int half = (this.size - 1) / 2;
            directions.ForEach(vec =>
            {
                for (int i = 0; i < this.size - 1; i++)
                {
                    int x = vec.x == 0 ? i : (vec.x + 1 == 0 ? (vec.x + 1) * half : (vec.x + 1) * half - 1);
                    int y = vec.y == 0 ? i : (vec.y + 1 == 0 ? (vec.y + 1) * half : (vec.y + 1) * half - 1);
                    walls[x, y] = new Wall(x, y,
                        !Convert.ToBoolean(Math.Abs(vec.y)));
                }
            });

            return walls;
            
        }

        private Piece[] createPieces(int n)
        {
            int half = (this.size - 1) / 2;
            int a = n == 2 ? 2 : 1;
            Piece[] pieces = new Piece[n];
            for (int i = 0; i < directions.Count; i+=1*a)
            {
                int x = (directions[i].x + 1) * half;
                int y = (directions[i].y + 1) * half;

                var winDir = new Vec2<int>(directions[i].x * -1, directions[i].y * -1);

                var piece = new Piece(x, y, winDir, 20 / n);
                pieces[i/a] = piece;
                board[x, y].player = piece;
            }
            return pieces;
        }

        static public int getWinDistance(Vec2<int> pos, Vec2<int> winDir)
        {
            int half = (9 - 1) / 2;
            return Math.Abs(winDir.x * (pos.x - (half * (winDir.x + 1))) + winDir.y * (pos.y - (half * (winDir.y + 1))));
        }

        public static List<Node> getNeighbours(Cell[,] board, float cost, Vec2<int> pos)
        {
            var neighbours = new List<Node>();
            foreach(var dir in directions) {
                if (board[pos.x, pos.y].walls.Contains(dir))
                    continue;

                var cellPos = new Vec2<int>(pos.x + dir.x, pos.y + dir.y);
                if (cellPos.x < 0 || cellPos.x >= 9 || cellPos.y < 0 || cellPos.y >= 9)
                    continue;

                var cell = board[cellPos.x, cellPos.y];

                if (cell.player == null)
                    neighbours.Add(new Node(cellPos, new Vec2<float>(cost, 0)));
                else
                {
                    var next_cell = board[cellPos.x, cellPos.y];

                    if (next_cell.walls.Contains(dir) || board[cellPos.x + dir.x, cellPos.y + dir.y].player != null)
                    {
                        var ways = getWays(board, new Vec2<int>(cellPos.x, cellPos.y), dir);

                        if (ways.Count > 0 && ways.Count < 3)
                            neighbours.Add(new Node(cellPos, new Vec2<float>(cost, 0)));
                    }
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
                    var neighbours = getNeighbours(board, curr_node.value.x, curr_node.pos);
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


    }
}