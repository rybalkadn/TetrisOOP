using System.Drawing;


namespace TetrisCore
{ 
    public struct Cell
    {
        public int X;
        public int Y;
        public int Kind;

        public Cell(int left, int top, int kind)
        {
            X = left;
            Y = top;
            Kind = kind;
        }

        public Cell(Point p, int kind)
        {
            X = p.X;
            Y = p.Y;
            Kind = kind;
        }
    }
}
