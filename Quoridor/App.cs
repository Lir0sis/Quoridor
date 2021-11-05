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
        TesterInput input;
        TesterOutput output;

        static public Random rand = new Random();
        public App() 
        {
            game = Quoridor.getInstance();
            input = new TesterInput();
            output = new TesterOutput();
        }

        public void Run()
        {
            if (playOrder == null)
            {
                var val = input.Run();
                if (val != null)
                {
                    playOrder = new PlayerType[] { (PlayerType)val, (PlayerType)Math.Abs((int)val - 1) };
                    if (playOrder[0] != PlayerType.AI)
                    {
                        // Quoridor.currentPlayer = 1;
                        // MiniMaxAI.MakeMove();
                        // output.Run();
                    }
                }

                return;
            }

            if (game.lastMove.result == Quoridor.TurnStatus.WON)
            {
                playOrder = null;
                return;
            }

            if (playOrder[Quoridor.currentPlayer] == PlayerType.HUMAN)
                input.Run();
            else if (playOrder[Quoridor.currentPlayer] == PlayerType.AI)
            {
                MiniMaxAI.MakeMove();
                output.Run();
            }

        }

        static public void Main()
        {
            App app = new App();
            while (true)
                app.Run();
        }
    }
}
