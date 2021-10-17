using System;
using System.Collections.Generic;
using System.Text;

namespace Quoridor
{
    class ConsoleOutput
    {
        Quoridor game = Quoridor.getInstance();
        Vec2<int> offset = new Vec2<int>(7, 3);

        public ConsoleOutput()
        {
            Console.OutputEncoding = Encoding.UTF8;
        }

        public void Run()
        {
            Console.Clear();
            Piece currentPlayer = game.players[Quoridor.currentPlayer];

            for (int i = 0; i < game.board.size; i++)
            {
                for (int j = 0; j < game.board.size; j++)
                {
                    var cell = game.board.board[j, i];

                    Console.SetCursorPosition(j * 2 + offset.x, i * 2 + offset.y);
                    if (cell.player == currentPlayer)
                        Console.ForegroundColor = ConsoleColor.Red;
                    else if (cell.Occupied)
                        Console.ForegroundColor = ConsoleColor.White;
                    else
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("\u25A0");
                    Console.ForegroundColor = ConsoleColor.White;

                    //foreach (var wall in cell.walls)
                    //{
                    //    Console.SetCursorPosition(j * 2 + wall.x + offset.x, i * 2 + wall.y + offset.y);
                    //    if (Math.Abs(wall.x) == 1)
                    //        Console.Write('║');
                    //    else
                    //        Console.Write('═');
                    //}
                    #region commented
                    //if (i % 2 == 0 && j % 2 == 0)
                    //{
                    //    if (game.board.board[j / 2, i / 2].Occupied)
                    //    {
                    //        if (currentPlayer.x == j / 2 && currentPlayer.y == i / 2)
                    //        {
                    //            Console.ForegroundColor = ConsoleColor.Red;
                    //            Console.Write("\u25A0");
                    //            Console.ForegroundColor = ConsoleColor.White;
                    //        }
                    //        else
                    //        {
                    //            Console.Write("\u25A0");
                    //        }
                    //
                    //    }
                    //    else
                    //    {
                    //        Console.ForegroundColor = ConsoleColor.DarkGray;
                    //        Console.Write("\u25A0");
                    //        Console.ForegroundColor = ConsoleColor.White;
                    //    }
                    //}
                    //else
                    //{
                    //    if (i % 2 == 1 && j % 2 == 1)
                    //    {
                    //        Console.Write("+");
                    //    }
                    //    else
                    //    {
                    //        Console.Write("#");
                    //    }
                    //
                    //}
                    #endregion
                }
                //Console.WriteLine();
            }

            for (int i = 0; i < game.board.walls.GetLength(1); i++)

                for (int j = 0; j < game.board.walls.GetLength(0); j++)
                {
                    var wall = game.board.walls[j, i];
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(j * 2 + 1 + offset.x, i * 2 + 1 + offset.y);
                    if (wall == null)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write('╬');
                    }
                    else 
                        for (int o = -1; o < 2; o++)
                            if (wall.isVertical)
                            {
                                Console.SetCursorPosition(j * 2 + 1 + offset.x, i * 2 + 1 + o + offset.y);
                                Console.Write('‖');
                            }
                            else
                            {
                                Console.SetCursorPosition(j * 2 + 1 + o + offset.x, i * 2 + 1 + offset.y);
                                Console.Write('═');
                            }
                }

            Console.SetCursorPosition(0, game.board.size * 2 + 2 + offset.y);
            if (game.lastMove.result == Quoridor.TurnStatus.WON)
                Console.WriteLine($"Player {Quoridor.currentPlayer} has won!");
            else
                Console.WriteLine($"Current player is {Quoridor.currentPlayer}");

            if (game.lastMove.next == Quoridor.MoveChoice.JUMP)
                Console.WriteLine("JUMP!");
            else
                Console.WriteLine($"Walls left: {game.players[Quoridor.currentPlayer].wallsLeft}");

            Console.SetCursorPosition(0, game.board.size * 2 + 4 + offset.y);
        }
    }
}
