using System;
using System.Collections.Generic;
using System.Threading;

namespace Quoridor
{
    class App
    {
        enum PlayerType { HUMAN, AI }

        Quoridor game;
        PlayerType[] playOrder;
        int botsCount;
        ConsoleInput input;
        ConsoleOutput output;

        static public Random rand = new Random();
        public App() 
        {
            game = Quoridor.getInstance();
            input = new ConsoleInput();
            output = new ConsoleOutput();
        }

        public void Run()
        {
            if (game.lastMove.result == Quoridor.TurnStatus.WRONG
                && game.lastMove.choice == Quoridor.MoveChoice.NONE 
                || game.lastMove.result == Quoridor.TurnStatus.WON)
            {
                Vec2<int> @params = input.Run();
                if (@params.y != -1)
                {
                    playOrder = new PlayerType[@params.x];
                    for (int n = 0; n < @params.y; n++)
                    {
                        while (true)
                        {
                            var i = rand.Next(0, @params.x);
                            if (playOrder[i] == PlayerType.HUMAN)
                            {
                                playOrder[i] = PlayerType.AI;
                                break;
                            }
                        }
                    }
                    this.botsCount = @params.y;
                }
                return;
            }

            output.Run();

            if (game.lastMove.result == Quoridor.TurnStatus.WON)
                return;

            if (playOrder[Quoridor.currentPlayer] == PlayerType.HUMAN)
                input.Run();
            else if (playOrder[Quoridor.currentPlayer] == PlayerType.AI)
            {
                Thread.Sleep(100);
                AI.MakeMove();
            }

            if (game.lastMove.result == Quoridor.TurnStatus.WON)
                output.Run();
        }

        static public void Main()
        {
            App app = new App();
            while (true)
                app.Run();
        }
    }
}
