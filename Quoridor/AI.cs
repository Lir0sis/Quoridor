using System;
using System.Collections.Generic;
using System.Text;

namespace Quoridor
{
    class RandomAI
    {
        static public Quoridor game = Quoridor.getInstance();
        private RandomAI()
        {
        }

        static public void MakeMove()
        {

            List<dynamic> args = new List<dynamic>();

            Quoridor.MoveChoice choice = Quoridor.MoveChoice.MOVE;

            if (game.players[Quoridor.currentPlayer].wallsLeft != 0)
                choice = (Quoridor.MoveChoice)App.rand.Next(0, 2);

            if (choice == Quoridor.MoveChoice.MOVE)
            {
                int x = 0,
                    y = 0;
                while (Math.Abs(x) + Math.Abs(y) != 1)
                {
                    x = App.rand.Next(-1, 2);
                    y = App.rand.Next(-1, 2);
                }
                args.Add(new Vec2<int>(x, y));
            }
            else if (choice == Quoridor.MoveChoice.WALL)
            {
                int x = App.rand.Next(0, game.board.size - 2);
                int y = App.rand.Next(0, game.board.size - 2);

                bool isVertical = Convert.ToBoolean(App.rand.Next(0, 2));
                args.Add(isVertical);
                args.Add(new Vec2<int>(x, y));
            }
            game.MakeMove(choice, args.ToArray());
        }
    }

    class aStarAI
    {
        static public Quoridor game = Quoridor.getInstance();
        private aStarAI()
        {
        }
        static public void MakeMove()
        {
            List<dynamic> args = new List<dynamic>();
            var playerPos = new Vec2<int>(game.players[Quoridor.currentPlayer].x, game.players[Quoridor.currentPlayer].y);
            var choice = Quoridor.MoveChoice.MOVE;
            var path = Algorithm.aStar(game.board.board,
                playerPos,
                game.players[Quoridor.currentPlayer].winDir);

            // args.Add(path[1]);
            var cell = game.board.board[path[1].x, path[1].y];
            if (cell.Occupied)
            {
                var dir = path[1] - playerPos;
                if (cell.walls.Contains(dir))
                {
                    var jumps = new List<Vec2<int>>();
                    var perpDir = new Vec2<int>(Math.Abs(dir.y), Math.Abs(dir.x));
                    var cell1 = path[1] - perpDir;
                    if (!(game.board.board[cell1.x, cell1.y].Occupied || game.board.board[path[1].x, path[1].y].walls.Contains(-perpDir)))
                        jumps.Add(cell1);

                    var cell2 = path[1] + perpDir;
                    if (!(game.board.board[cell2.x, cell2.y].Occupied || game.board.board[path[1].x, path[1].y].walls.Contains(perpDir)))
                        jumps.Add(cell2);

                    choice = Quoridor.MoveChoice.JUMP;
                    if (jumps.Count == 2)
                        if (path.Contains(jumps[0]))
                            args.Add(jumps[0]);
                        else if (path.Contains(jumps[1]))
                            args.Add(jumps[1]);
                        else
                            args.Add(jumps[App.rand.Next(0, 2)]);
                    else
                        args.Add(jumps[0]);
                }
                else
                {
                    choice = Quoridor.MoveChoice.JUMP;
                    args.Add(path[1] + dir);
                }
            }
            else
            {
                args.Add(path[1]);
            }
            game.MakeMove(choice, args.ToArray());
        }
    }
    class MiniMaxAI
    {
        static public Quoridor game = Quoridor.getInstance();
        private MiniMaxAI()
        {
        }

        static public void MakeMove()
        {
            //var playerPos = new Vec2<int>(game.players[Quoridor.currentPlayer].x, game.players[Quoridor.currentPlayer].y);
            var action = Algorithm.miniMax(5, Quoridor.currentPlayer, game.board);

            game.MakeMove(action.Item1, action.Item2);
        }
    }
}
