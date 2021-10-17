using System;
using System.Collections.Generic;

namespace Quoridor
{
    class Quoridor
    {
        public struct Turn
        {
            public Turn(TurnStatus result, MoveChoice choice, MoveChoice next, Vec2<int> pos, List<Vec2<int>> ways = null)
            {
                this.result = result;
                this.ways = ways;
                this.pos = pos;
                this.next = next;
                this.choice = choice;
            }

            public TurnStatus result;
            public List<Vec2<int>> ways;
            public Vec2<int> pos;
            public MoveChoice next;
            public MoveChoice choice;
        }
        public enum MoveChoice { WALL, MOVE, JUMP, NONE }
        public enum TurnStatus { MOVED, PLACED, JUMPED, WRONG, STARTED, WON}

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

        private Quoridor() 
        {
            lastMove = new Turn(TurnStatus.WRONG, MoveChoice.NONE, MoveChoice.NONE, new Vec2<int>(-1, -1));
        }

        public void Start(int boardSize = 9, int playerCount = 2)
        {
            if (playerCount != 2 && playerCount != 4)
            {
                lastMove = new Turn(TurnStatus.WRONG, MoveChoice.NONE, MoveChoice.NONE, new Vec2<int>(-1, -1));
                return;
            }
            this.board = new Board(playerCount, boardSize);
            lastMove = new Turn(TurnStatus.STARTED, MoveChoice.NONE, MoveChoice.NONE, new Vec2<int>(-1, -1));
        }

        public void MakeMove(MoveChoice choice, dynamic[] args)
        {
            if (lastMove.result == TurnStatus.WON || lastMove.result == TurnStatus.WRONG && lastMove.choice == MoveChoice.NONE)
                return;

            if (choice == MoveChoice.MOVE) {
                if (lastMove.next == MoveChoice.JUMP)
                    lastMove = board.jumpOver(lastMove, args[0], currentPlayer);
                else
                    lastMove = board.movePiece(currentPlayer, args[0]);

            }
            else if (choice == MoveChoice.WALL)
            {
                lastMove = board.placeWall(args[0], args[1]);
            }
            
            if (lastMove.result == TurnStatus.WRONG || lastMove.result == TurnStatus.WON || lastMove.next == MoveChoice.JUMP)
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
