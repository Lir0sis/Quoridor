using System.Collections.Generic;

namespace Quoridor
{
    struct Vec2
    {
        public int x, y;
        public Vec2(int x, int y) { this.x = x; this.y = y; }
    }

    class Cell
    {
        public int x;
        public int y;
        public HashSet<Vec2> walls;
        public Piece player = null;
        public bool Occupied { get
            {
                return player != null;
            }
        }

        public Cell(int x, int y, HashSet<Vec2> walls)
        {
            this.x = x;
            this.y = y;
            this.walls = walls;
        }

        public Cell deepCopy()
        {
            var copy = (Cell)this.MemberwiseClone();
            HashSet<Vec2> walls = new HashSet<Vec2>();
            copy.player = new Piece(this.player.x, this.player.y, this.player.winDir);

            Vec2[] coppiedWalls = new Vec2[this.walls.Count];
            this.walls.CopyTo(coppiedWalls);
            
            foreach (Vec2 vec in coppiedWalls) 
                walls.Add(vec);
            copy.walls = walls;

            return copy;
        }
    }

    class Wall
    {
        public int x;
        public int y;
        public bool isVertical;
        public Wall(int x, int y, bool isVertical)
        {
            this.x = x;
            this.y = y;
            this.isVertical = isVertical;
        }
    }

    class Piece
    {
        public int x;
        public int y;
        public Vec2 winDir;
        private Board board;

        public Piece(int x, int y, Vec2 winDir)
        {
            this.x = x;
            this.y = y;
            this.winDir = winDir;
            board = Quoridor.getInstance().board;
        }
    }
}