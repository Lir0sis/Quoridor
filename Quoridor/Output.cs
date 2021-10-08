using System;
using System.Collections.Generic;
using System.Text;

namespace Quoridor
{
    class ConsoleOutput
    {
        Quoridor game = Quoridor.getInstance();

        public void Run()
        {
            Console.Clear();
            Piece currentPlayer = game.players[Quoridor.currentPlayer];

            for (int i = 0; i < 17; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    if (i % 2 == 0 && j % 2 == 0)
                    {
                        if (game.board.board[i / 2, j / 2].Occupied)
                        {
                            if (currentPlayer.x == i / 2 && currentPlayer.y == j / 2)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("\u25A0");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else
                            {
                                Console.Write("\u25A0");
                            }

                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write("\u25A0");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    else
                    {
                        if (i % 2 == 1 && j % 2 == 1)
                        {
                            Console.Write("+");
                        }
                        else
                        {
                            Console.Write("#");
                        }

                    }
                }
                Console.WriteLine();
            }
        }
    }
}
