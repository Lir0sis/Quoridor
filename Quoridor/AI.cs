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
            var choice = (Quoridor.MoveChoice) App.rand.Next(0, 1);
            if (choice == Quoridor.MoveChoice.MOVE)
            {
                int x = App.rand.Next(0, game.board.size);
                int y = App.rand.Next(0, game.board.size);
                args.Add(new Vec2<int>(x, y));
            }
            else if (choice == Quoridor.MoveChoice.WALL)
            {
                int x = App.rand.Next(0, game.board.size);
                int y = App.rand.Next(0, game.board.size);
                
                bool isVertical = Convert.ToBoolean(App.rand.Next(0, 1));
                args.Add(isVertical);
                args.Add(new Vec2<int>(x, y));
            }
            game.MakeMove(choice, args.ToArray());
        }
    }
}
