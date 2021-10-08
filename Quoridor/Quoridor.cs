using System;
using System.Collections.Generic;

namespace Quoridor
{
    class Quoridor
    {
        public struct Turn
        {
            public Turn(TurnStatus result, MoveChoice choice, MoveChoice next, Vec2 pos, List<Vec2> ways = null)
            {
                this.result = result;
                this.ways = ways;
                this.pos = pos;
                this.next = next;
                this.choice = choice;
            }

            public TurnStatus result;
            public List<Vec2> ways;
            public Vec2 pos;
            public MoveChoice next;
            public MoveChoice choice;
        }
        public enum MoveChoice { WALL, MOVE, JUMP, NONE }
        public enum TurnStatus { MOVED, PLACED, JUMPED, WRONG, WON}

        public Turn lastMove;
        public Piece[] players;
        public Board board;
        static public int currentPlayer = 0;

        static private Quoridor instance = null;
        static public Quoridor getInstance()
        {
            if (instance == null)
                instance = new Quoridor();
            return instance;
        }

        private Quoridor() {}

        public void Start(int boardSize = 9, int playerCount = 2)
        {
            if (playerCount > 4 && playerCount % 2 != 0)
            {
                lastMove = new Turn(TurnStatus.WRONG, MoveChoice.NONE, MoveChoice.NONE, new Vec2(0, 0));
                return;
            }
            this.board = new Board(playerCount, boardSize);
        }

        public void MakeMove(MoveChoice choice, dynamic[] args)
        {
            if (choice == MoveChoice.MOVE) {
                if (lastMove.next == MoveChoice.JUMP)
                    lastMove = board.jumpOver(lastMove, args[0], currentPlayer);
                lastMove = board.movePiece(currentPlayer, args[0]);
            }
            else if (choice == MoveChoice.WALL)
            {
                lastMove = board.placeWall(args[1], args[0]);
            }
            if (lastMove.result == TurnStatus.WON)
                return;

            switchPlayer();
        }

        private void switchPlayer()
        {
            currentPlayer++;
            if (currentPlayer >= players.Length)
                currentPlayer = 0;
        }

    }
}
