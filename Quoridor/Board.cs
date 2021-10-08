using System.Collections.Generic;
using System;

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
        static private readonly List<Vec2> directions =
            new List<Vec2>
            {
                new Vec2 (0, 1),
                new Vec2 (0, -1),
                new Vec2 (1, 0),
                new Vec2 (-1, 0)
            };

        public Board(int playerCount, int size)
        {
            game = Quoridor.getInstance();

            this.size = size;
            board = createBoard();
            walls = fillCenterWalls();
            players = createPieces(playerCount);
            game.players = players;
        }
        private void updatePlayerPos(ref Piece playerPiece, Vec2 nextPos)
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

        public Quoridor.Turn movePiece(int player, Vec2 dir)
        {
            var status = Quoridor.TurnStatus.MOVED;

            var playerPiece = players[player];
            int x = playerPiece.x, 
                y = playerPiece.y;

            int next_x = x + dir.x,
                next_y = y + dir.y;

            if (board[x, y].walls.Contains(dir))
                return new Quoridor.Turn(
                    Quoridor.TurnStatus.WRONG,
                    Quoridor.MoveChoice.MOVE,
                    Quoridor.MoveChoice.NONE,
                    new Vec2(next_x, next_y)
                    );

            var next_cell = board[next_x, next_y];
            if (next_cell.player != null)
            {
                if (next_cell.walls.Contains(dir) || board[next_x + dir.x, next_y+ dir.y].player != null)
                {
                    List<Vec2> ways = new List<Vec2>();

                    if (!next_cell.walls.Contains(new Vec2(dir.y, dir.x)) && board[next_x + dir.y, next_y + dir.x].player == null)
                    { 
                        ways.Add(new Vec2(dir.y, dir.x)); 
                    }
                    if (!next_cell.walls.Contains(new Vec2(dir.y * -1, dir.x * -1)) && board[next_x + dir.y * -1, next_y + dir.x * -1].player == null)
                    {
                        ways.Add(new Vec2(dir.y * -1, dir.x * -1));
                    }

                    if (ways.Count == 2)
                        return new Quoridor.Turn(
                            Quoridor.TurnStatus.MOVED,
                            Quoridor.MoveChoice.MOVE,
                            Quoridor.MoveChoice.JUMP,
                            new Vec2(next_cell.x, next_cell.y),
                            ways
                            );

                    else if (ways.Count == 1)
                    {
                        next_x += ways[0].x;
                        next_y += ways[0].y;

                        updatePlayerPos(ref playerPiece, new Vec2(next_x, next_y));

                        status = Quoridor.TurnStatus.JUMPED;
                        if (hasWon(playerPiece))
                            status = Quoridor.TurnStatus.WON;

                        return new Quoridor.Turn(
                            status,
                            Quoridor.MoveChoice.MOVE,
                            Quoridor.MoveChoice.NONE,
                            new Vec2(next_x, next_y)
                            );
                    }

                    return new Quoridor.Turn(
                        Quoridor.TurnStatus.WRONG,
                        Quoridor.MoveChoice.MOVE,
                        Quoridor.MoveChoice.NONE,
                        new Vec2(next_x, next_y)
                        );
                }
                else
                {
                    next_x += dir.x; 
                    next_y += dir.y;
                }
            }

            updatePlayerPos(ref playerPiece, new Vec2(next_x, next_y));

            
            if (hasWon(playerPiece))
                status = Quoridor.TurnStatus.WON;

            return new Quoridor.Turn(
                status,
                Quoridor.MoveChoice.MOVE,
                Quoridor.MoveChoice.NONE,
                new Vec2(next_x, next_y)
                ); 
        }
        
        private bool hasWon(Piece player)
        {
            return player.x * Math.Abs(player.winDir.x) == player.winDir.x * (size - 1) 
                && player.y * Math.Abs(player.winDir.y) == player.winDir.y * (size - 1);
        }

        public Quoridor.Turn jumpOver(Quoridor.Turn turn, Vec2 dir, int player)
        {
            var crossingPos = turn.pos;
            var ways = turn.ways;

            Vec2 next_cell = new Vec2(crossingPos.x + dir.x, crossingPos.y + dir.y);
            if (!ways.Contains(dir))
                return new Quoridor.Turn(
                    Quoridor.TurnStatus.WRONG,
                    Quoridor.MoveChoice.JUMP,
                    Quoridor.MoveChoice.NONE,
                    new Vec2(next_cell.x, next_cell.y)
                    );

            updatePlayerPos(ref players[player], next_cell);

            var status = Quoridor.TurnStatus.JUMPED;
            if (hasWon(players[player]))
                status = Quoridor.TurnStatus.WON;

            return new Quoridor.Turn(
                    status,
                    Quoridor.MoveChoice.JUMP,
                    Quoridor.MoveChoice.NONE,
                    new Vec2(next_cell.x, next_cell.y)
                    );
        }

        public Quoridor.Turn placeWall(bool isVertical, Vec2 target)
        {
            if (!(1 <= target.x && target.x < size) ||
                !(1 <= target.y && target.y < size))
                return new Quoridor.Turn(
                    Quoridor.TurnStatus.WRONG,
                    Quoridor.MoveChoice.WALL,
                    Quoridor.MoveChoice.NONE,
                    new Vec2(target.x, target.y)
                    );

            if (isVertical)
            {
                for (int i = target.y - 1; i <= target.y + 1; i++)
                {
                    var wall = this.walls[target.x, i];
                    if (wall != null && wall.isVertical)
                        return new Quoridor.Turn(
                            Quoridor.TurnStatus.WRONG,
                            Quoridor.MoveChoice.WALL,
                            Quoridor.MoveChoice.NONE,
                            new Vec2(target.x, target.y)
                            );
                }
            }
            else
            {
                for (int i = target.x - 1; i <= target.x + 1; i++)
                {
                    var wall = this.walls[i, target.y];
                    if (wall != null && wall.isVertical)
                        return new Quoridor.Turn(
                            Quoridor.TurnStatus.WRONG,
                            Quoridor.MoveChoice.WALL,
                            Quoridor.MoveChoice.NONE,
                            new Vec2(target.x, target.y)
                            );
                }
            }

            this.walls[target.x, target.y] = new Wall(target.x, target.y, isVertical);
            List<Cell> saved_cells = new List<Cell>();

            for (int i = target.x - 1; i <= target.x; i++)
                for (int j = target.y - 1; j <= target.y; j++)
                {
                    int dirX = Convert.ToInt32(isVertical) * ((target.x * 2 - 1) - i * 2);
                    int dirY = Convert.ToInt32(!isVertical) * ((target.y * 2 - 1) - j * 2);

                    saved_cells.Add(this.board[i, j].deepCopy());
                    this.board[i, j].walls.Add(new Vec2(dirX, dirY));
                }

            if (aStar())
                return new Quoridor.Turn(
                    Quoridor.TurnStatus.PLACED,
                    Quoridor.MoveChoice.WALL,
                    Quoridor.MoveChoice.NONE,
                    new Vec2(target.x, target.y)
                    );
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
                    Quoridor.MoveChoice.NONE,
                    new Vec2(target.x, target.y)
                    );
            }
            // return new Game.LastMove(
            //         Game.TurnStatus.PLACED,
            //         Game.MoveChoice.WALL,
            //         Game.MoveChoice.NONE,
            //         new Vec2(target.x, target.y)
            //         );
        }

        private Cell[,] createBoard()
        {
            Cell[,] mat = new Cell[size, size];
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    HashSet<Vec2> walls = new HashSet<Vec2>();
                    if (i == 0)
                        walls.Add(new Vec2(-1, 0));
                    if (j == 0)
                        walls.Add(new Vec2(0, -1));
                    if (i == this.size - 1)
                        walls.Add(new Vec2(1, 0));
                    if (j == this.size - 1)
                        walls.Add(new Vec2(0, 1));

                    mat[i, j] = new Cell(i, j, walls);
                }
            }
            return mat;
        }

        private Wall[,] fillCenterWalls()
        {
            Wall[,] walls = new Wall[this.size - 1, this.size - 1];
            int half = (this.size + 1) / 2;
            directions.ForEach(vec =>
            {
                for (int i = 0; i < this.size - 1; i++)
                {
                    int x = vec.x == 0 ? i : (vec.x + 1) * half;
                    int y = vec.y == 0 ? i : (vec.y + 1) * half;
                    walls[x, y] = new Wall(x, y,
                        !Convert.ToBoolean(Math.Abs(vec.y)));
                }
            });

            return walls;
            
        }

        private Piece[] createPieces(int n)
        {
            int half = (this.size - 1) / 2;

            Piece[] pieces = new Piece[n];
            for (int i = 0; i < n; i++)
            {
                int x = (directions[i].x + 1) * half;
                int y = (directions[i].y + 1) * half;
                pieces[i] = new Piece(x, y, directions[i]);
                board[x, y].player = pieces[i];
            }

            return pieces;
        }

        public List<Vec2> aStar(Vec2 start, Vec2 end, Func<Piece, float> heuristic)
        {
            return null;
        }


    }
}