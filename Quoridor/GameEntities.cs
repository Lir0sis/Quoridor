using System.Collections.Generic;

namespace Quoridor
{
    struct Vec2<T>
    {
        public T x, y;
        public Vec2(T x, T y) { this.x = x; this.y = y; }
        public override bool Equals(object obj)
        {
            var vec2 = (Vec2<T>)obj;
            if (this.x.Equals(vec2.x) && this.y.Equals(vec2.y))
                return true;
            return false;
        }

        public static Vec2<T> operator +(Vec2<T> vec1, Vec2<T> vec2) 
        {
            dynamic a = vec1;
            dynamic b = vec2;
            return new Vec2<T>(a.x + b.x, a.y + b.y);
        }

        public static Vec2<T> operator -(Vec2<T> vec1, Vec2<T> vec2)
        {
            dynamic a = vec1;
            dynamic b = vec2;
            return new Vec2<T>(a.x - b.x, a.y - b.y);
        }

        public static Vec2<T> operator -(Vec2<T> vec)
        {
            dynamic a = vec;
            return new Vec2<T>(-a.x, -a.y);
        }
    }

    class Cell
    {
        public int x;
        public int y;
        public HashSet<Vec2<int>> walls;
        //public Piece player = null;
        public bool Occupied;

        public Cell(int x, int y, HashSet<Vec2<int>> walls)
        {
            this.x = x;
            this.y = y;
            this.walls = walls;
        }

        public Cell Clone()
        {
            var copy = (Cell)this.MemberwiseClone();
            var walls = new HashSet<Vec2<int>>();
            // Piece player = null;
            // if (this.player != null)
            //     player = new Piece(this.player.x, this.player.y, this.player.winDir, this.player.wallsLeft);

            Vec2<int>[] coppiedWalls = new Vec2<int>[this.walls.Count];
            this.walls.CopyTo(coppiedWalls);
            
            foreach (Vec2<int> vec in coppiedWalls) 
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

        public Wall Clone()
        {
            return (Wall)this.MemberwiseClone();
        }
    }

    class Piece
    {
        public int x;
        public int y;
        public Vec2<int> winDir;
        public int wallsLeft;
        // private Board board;

        public Piece(int x, int y, Vec2<int> winDir, int wallsCount)
        {
            wallsLeft = wallsCount;
            this.x = x;
            this.y = y;
            this.winDir = winDir;
            // board = Quoridor.getInstance().board;
        }

        public Piece Clone()
        {
            return (Piece)this.MemberwiseClone();
        }
    }
}