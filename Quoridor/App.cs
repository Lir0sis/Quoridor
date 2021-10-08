using System;
using System.Collections.Generic;
using System.Text;

namespace Quoridor
{
    class App
    {
        enum PlayerType { HUMAN, AI }

        Quoridor game;
        PlayerType[] playOrder;
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
            var bots = input.Run();
            // var status = output.Run();
            if (bots.y != -1)
            {
                playOrder = new PlayerType[bots.x];
                for (int n = 0; n < bots.x; n++)
                {
                    int i;
                    do
                    {
                        i = rand.Next(0, bots.x);
                    }
                    while (playOrder[i] == 0);

                    playOrder[i] = PlayerType.AI;
                }
            }

            while (playOrder[Quoridor.currentPlayer] != PlayerType.HUMAN)
            {
                AI.MakeMove();
            }
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
