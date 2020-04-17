using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;

namespace TetrisCore
{
    public class Shape : IShapeView
    {
        public Shape(int capacity)
        {
            _offsets = new Point[capacity];
        }

        public Shape(Point position, int kind, int capacity, int pointCount, Point[] offsets)
        {
            _kind = kind;
            _position = position;
            _pointCount = pointCount;
            _offsets = new Point[capacity];
            offsets.CopyTo(_offsets, 0);
        }

        public int Kind
        {
            get
            {
                return _kind;
            }
        }

        public Point Position
        {
            get { return _position; }
            set { _position = value; }
        }


        public void CopyTo(ref Shape destination)
        {
            destination._kind = _kind;
            destination._position = _position;
            destination._pointCount = _pointCount;
            _offsets.CopyTo(destination._offsets, 0);
        }

        public void Rotate()
        {
            for (int i = _pointCount - 1; i >= 0; i--)
            {
                // Вращаем против часовой стрелки
                int temp = _offsets[i].X;
                _offsets[i].X = -_offsets[i].Y;
                _offsets[i].Y = temp;
            }
        }

        public void Move(int dx, int dy)
        {
            _position.Offset(dx, dy);
        }

        public static Cell[] operator -(Shape sh1, Shape sh2)
        {

            Point[] fromPoints = sh2.ToFieldPoints();
            Point[] toPoints = sh1.ToFieldPoints();

            Point[] makeEmpty = Shape.SubtractPointSet(fromPoints, toPoints);
            Point[] makeFilled = Shape.SubtractPointSet(toPoints, fromPoints);

            Cell[] changes = new Cell[makeEmpty.Length + makeFilled.Length];

            if (changes.Length > 0)
            {
                for (int i = makeEmpty.Length - 1; i >= 0; i--)
                {
                    changes[i] = new Cell(makeEmpty[i], 0);
                }

                int index0 = makeEmpty.Length;
                for (int i = makeFilled.Length - 1; i >= 0; i--)
                {
                    changes[i + index0] = new Cell(makeFilled[i], sh1.Kind);
                }
            }

            return changes;
        }

        /// <summary>
        /// Преобразовать массив точек фигуры в массив точек с координатами игрового поля
        /// </summary>
        /// <param name="sh">Исходная фигура</param>
        /// <returns>Массив точек с координатами игрового поля</returns>
        private Point[] ToFieldPoints()
        {
            Point[] p = (Point[])_offsets.Clone();

            for (int i = 0; i < p.Length; i++)
            {
                p[i].Y += Position.Y;
                p[i].X += Position.X;
            }

            return p;
        }

        /// <summary>
        /// Из одного множества точек вычесть второе
        /// </summary>
        /// <param name="original">Исходное множество точек</param>
        /// <param name="shapeToSubtract">Вычитаемое множество точек</param>
        /// <returns>Массив точек, относящихся к первому множеству, которые отсутствуют в вычитаемом множестве</returns>
        private static Point[] SubtractPointSet(Point[] original, Point[] setToSubtract)
        {
            Point[] result = new Point[original.Length];
            int found = 0;

            for (int i = 0; i < original.Length; i++)
            {
                bool pointExists = false;

                for (int j = 0; j < setToSubtract.Length; j++)
                {
                    if (original[i].X == setToSubtract[j].X
                        && original[i].Y == setToSubtract[j].Y)
                    {
                        pointExists = true;
                        break;
                    }
                }

                if (!pointExists)
                {
                    result[found++] = original[i];
                }
            }

            // Если результирующее множество точек меньше оригинального, то уменьшаем размер результирующего массива точек
            if (found < original.Length)
            {
                Array.Resize(ref result, found);
            }

            return result;
        }

        public int MinY
        {
            get {
                int min = _offsets[_pointCount - 1].Y;
                for (int i = _pointCount - 2; i >= 0; i--)
                {
                    int y = _offsets[i].Y;
                    if ( y < min)
                    {
                        min = y;
                    }

                }

                return min + _position.Y;
            }
        }

        public int MaxY
        {
            get
            {
                int max = _offsets[_pointCount - 1].Y;
                for (int i = _pointCount - 2; i >= 0; i--)
                {
                    int y = _offsets[i].Y;
                    if (y > max)
                    {
                        max = y;
                    }

                }

                return max + _position.Y;
            }
        }

        #region IEnumerator<Cell>
        public IEnumerator<Cell> GetEnumerator()
        {
            return new CellEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CellEnumerator(this);
        }

        private struct CellEnumerator : IEnumerator<Cell>
        {
            private int _pos;
            private Shape _sh;

            public CellEnumerator(Shape sh)
            {
                _sh = sh;
                _pos = -1;
            }

            public Cell Current
            {
                get
                {
                    // receive GameField coordinate by adding Shape position and point offset
                    Point p0 = _sh._position;
                    Cell p = new Cell(_sh._offsets[_pos], _sh.Kind);
                    p.X += p0.X;
                    p.Y += p0.Y;
                    return p;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_pos >= _sh._pointCount - 1)
                {
                    return false;
                }

                _pos++;
                return true;
            }

            public void Reset()
            {
                _pos = -1;
            }
        }
        #endregion

        private int _kind;
        private Point _position;
        private int _pointCount;
        private Point[] _offsets;
    }
}
