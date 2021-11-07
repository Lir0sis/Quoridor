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
        static public readonly List<Vec2<int>> directions =
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

        public Board Clone()
        {
            var copy = (Board)this.MemberwiseClone();
            copy.board = new Cell[9, 9];
            copy.walls = new Wall[8, 8];
            copy.players = new Piece[players.Length];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    copy.board[i, j] = board[i, j].Clone();
                    if (j >= size - 1 || i >= size - 1)
                        continue;
                    copy.walls[i, j] = walls[i, j] != null ? walls[i, j].Clone() : null;
                }

            for (int i = 0; i < players.Length; i++)
            {
                copy.players[i] = players[i].Clone();
            }
            return copy;
        }
        private void updatePlayerPos(ref Piece playerPiece, Vec2<int> nextPos)
        {
            int x = playerPiece.x,
                y = playerPiece.y;

            int next_x = nextPos.x,
                next_y = nextPos.y;

            playerPiece.x = next_x;
            playerPiece.y = next_y;

            board[next_x, next_y].Occupied = true;
            board[x, y].Occupied = false;
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
        
        public bool hasWon(Piece player)
        {
            int half = (this.size - 1) / 2;
            return player.x * Math.Abs(player.winDir.x) == (player.winDir.x + 1) * Math.Abs(player.winDir.x) * half
                && player.y * Math.Abs(player.winDir.y) == (player.winDir.y + 1) * Math.Abs(player.winDir.y) * half;
        }

        public List<Vec2<int>> getPossibleJumps(int player)
        {
            var playerPiece = players[player];
            int x = playerPiece.x,
                y = playerPiece.y;
            var possibleJumps = new List<Vec2<int>>();
            foreach (var dir in directions)
            {
                if (board[x, y].walls.Contains(dir)
                    || !board[x + dir.x, y + dir.y].Occupied) continue;

                var oponentPos = new Vec2<int>(x + dir.x, y + dir.y);
                var endPos = oponentPos + dir;
                if (!(endPos.x > 8 || endPos.x < 0 || endPos.y > 8 || endPos.y < 0))  
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

            return possibleJumps;

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

            var possibleJumps = getPossibleJumps(player);

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

                    saved_cells.Add(this.board[i, j].Clone());
                    this.board[i, j].walls.Add(new Vec2<int>(dirX, dirY));
                }

            bool isPassable = true;
            for (int i = 0; i < players.Length; i++)
            {
                List<Vec2<int>> path = Algorithm.aStar(board, new Vec2<int>(players[i].x, players[i].y), players[i].winDir);
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
                board[x, y].Occupied = true;
            }
            return pieces;
        }

        public List<(Quoridor.MoveChoice, dynamic[])> getActions(int player)
        {
            var actions = new List<(Quoridor.MoveChoice, dynamic[])>();

            var playerPiece = players[player];
            int rot = playerPiece.winDir.x + playerPiece.winDir.y;
            var pos = new Vec2<int>(playerPiece.x, playerPiece.y);
            
            foreach(var neighbour in Algorithm.getNonWallNeighbours(board, 0, pos, rot))
            {
                var choice = Quoridor.MoveChoice.MOVE;
                var dir = neighbour.pos - pos;
                if (Math.Abs(dir.x) + Math.Abs(dir.y) > 1)
                    choice = Quoridor.MoveChoice.JUMP;
                actions.Add((choice, new dynamic[] { neighbour.pos }));
            }
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++) 
                {
                    var wall = walls[i, j];
                    if (wall != null) continue;
                    var canBePlaced = true;
                    for (int k = j - 1; k <= j + 1; k++)
                    {
                        if (k >= 9 - 1 || k < 0)
                            continue;
                        var next_wall = this.walls[i, k];
                        if (next_wall != null && next_wall.isVertical)
                        {
                            canBePlaced = false;
                            break;
                        }
                    }
                    if (canBePlaced)
                        actions.Add((Quoridor.MoveChoice.WALL, new dynamic[] { new Vec2<int>(i,j), true }));

                    canBePlaced = true;
                    for (int k = i - 1; k <= i + 1; k++)
                    {
                        if (k >= 9 - 1 || k < 0)
                            continue;
                        var next_wall = this.walls[k, j];
                        if (next_wall != null && !next_wall.isVertical)
                        {
                            canBePlaced = false;
                            break;
                        }
                    }
                    if (canBePlaced)
                        actions.Add((Quoridor.MoveChoice.WALL, new dynamic[] { new Vec2<int>(i, j), false }));

                }

            return actions;
        }
        public float getScore(int player)
        {
            int enemy = Math.Abs(player - 1);
            var playerPos = new Vec2<int>(players[player].x, players[player].y);
            var enemyPos = new Vec2<int>(players[enemy].x, players[enemy].y);
            // var windDist = Algorithm.getWinDistance(pos, players[player].winDir);

            var playerPath = Algorithm.aStar(board, playerPos, players[player].winDir);
            var enemyPath = Algorithm.aStar(board, enemyPos, players[enemy].winDir);

            return enemyPath.Count - playerPath.Count;
        }
    }
}