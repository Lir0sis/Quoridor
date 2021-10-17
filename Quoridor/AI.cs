using System;
using System.Collections.Generic;
using System.Text;

namespace Quoridor
{
    class AI
    {
        static public Quoridor game = Quoridor.getInstance();
        private AI()
        {
        }

        static public void MakeMove()
        {

            List<dynamic> args = new List<dynamic>();

            Quoridor.MoveChoice choice = Quoridor.MoveChoice.MOVE;

            if (game.players[Quoridor.currentPlayer].wallsLeft != 0)
                choice = (Quoridor.MoveChoice) App.rand.Next(0, 2);

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
                int x = App.rand.Next(0, game.board.size-2);
                int y = App.rand.Next(0, game.board.size-2);
                
                bool isVertical = Convert.ToBoolean(App.rand.Next(0, 2));
                args.Add(isVertical);
                args.Add(new Vec2<int>(x, y));
            }
            game.MakeMove(choice, args.ToArray());
        }
    }
}
