using System;
using System.Collections.Generic;

namespace Quoridor
{
    class ConsoleInput
    {
        Quoridor game = Quoridor.getInstance();
        // App app = App.getInstance();

        static private readonly Dictionary<string, Vec2<int>> directions =
            new Dictionary<string, Vec2<int>>
            {
                {"up", new Vec2<int>(0, -1)},
                {"down", new Vec2<int>(0, 1)},
                {"left", new Vec2<int>(-1, 0)},
                {"right", new Vec2<int>(1, 0)}
            };

        public Vec2<int> Run()
        {
            var splitCommand = Console.ReadLine().Split(new char[0]);

            switch (splitCommand[0].ToLower())
            {
                case "start":
                    if (splitCommand.Length == 3)
                    {
                        var players = int.Parse(splitCommand[1]);
                        var bots = int.Parse(splitCommand[2]);
                        game.Start(playerCount: players);
                        return new Vec2<int>(players, bots);
                    }
                    else
                        game.Start();
                    break;
                case "exit":
                    Environment.Exit(0);
                    break;
                case "move":
                    var dir = splitCommand[1].ToLower();
                    game.MakeMove(Quoridor.MoveChoice.MOVE, new dynamic[] { directions[dir] });
                    break;
                case "place":
                    if (splitCommand.Length == 4)
                    {
                        var x = int.Parse(splitCommand[1]);
                        var y = int.Parse(splitCommand[2]);
                        var isVertical = Convert.ToBoolean(int.Parse(splitCommand[3]));
                        game.MakeMove(Quoridor.MoveChoice.WALL, new dynamic[] { new Vec2<int>(x, y), isVertical });
                    }
                    break;
            }

            return new Vec2<int>(-1,-1);

        }
    }

    class TesterInput
    {
        static private readonly Dictionary<string, Quoridor.MoveChoice> moves =
            new Dictionary<string, Quoridor.MoveChoice>
            {
                {"move", Quoridor.MoveChoice.MOVE },
                {"jump", Quoridor.MoveChoice.JUMP },
                {"wall", Quoridor.MoveChoice.WALL },
            };

        Quoridor game = Quoridor.getInstance();

        public int? Run()
        {
            var splitCommand = Console.ReadLine().Split(new char[0]);
            if (splitCommand[0] == "white" || splitCommand[0] == "black")
            {
                game.Start();
                return splitCommand[0].ToLower() == "white" ? 1 : 0;
            }
            var moveType = moves[splitCommand[0].ToLower()];

            if (moveType == Quoridor.MoveChoice.WALL) 
            {
                var x = splitCommand[1][0] - 'S';
                var y = splitCommand[1][1] - '1';
                var isVertical = splitCommand[1][2] == 'v';
                game.MakeMove(moveType, new dynamic[] { new Vec2<int>(x, y), isVertical });
            }
            else
            {
                var x = splitCommand[1][0] - 'A';
                var y = splitCommand[1][1] - '1';
                game.MakeMove(moveType, new dynamic[] { new Vec2<int>(x, y) });
            }

            return null;
        }
    }
}