using System;

namespace TetrisCore
{
    public class GameField : IGameFieldView
    {
        private readonly int _width;
        private readonly int _height;

        public GameField(int width, int height)
        {
            _width = width;
            _height = height;
            _firstEmptyRow = _height - 1;
            _cells = new int[_width, _height];
        }

        public int FirstEmptyRow
        {
            get { return _firstEmptyRow; }
            private set { _firstEmptyRow = value; }
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
        }

        public int this[int left, int top]
        {
            get
            {
                return _cells[left, top];
            }
        }

        public void AppendShape(Shape sh)
        {
            foreach (var item in sh)
            {
                _cells[item.X, item.Y] = sh.Kind;
                if (item.Y <= _firstEmptyRow)
                {
                    _firstEmptyRow = item.Y - 1;
                }
            }
        }

        /// <summary>
        /// Вычислить, приземлилась ли фигура
        /// </summary>
        /// <returns>Истина, если фигура приземлилась</returns>
        public bool IsShapeLanded(Shape sh)
        {
            bool isLanded = false;

            foreach (var item in sh)
            {
                if (item.Y == _height - 1
                    || _cells[item.X, item.Y + 1] != 0)
                {
                    // если текущая точка фигуры достигла дна или под ней находится непустая точка поля,
                    // то это значит, что фигура "приземлилась"
                    isLanded = true;
                    break;
                }
            }

            return isLanded;
        }

        public bool HasFilledRows(int minShapeRow, int maxShapeRow, out int[] filledRows)
        {
            int filledRowsCount = 0;

            filledRows = new int[maxShapeRow - minShapeRow + 1];

            for (int row = minShapeRow; row <= maxShapeRow; row++)
            {
                if (!RowHasEmpty(row))
                {
                    filledRows[filledRowsCount++] = row;
                }
            }

            if (filledRowsCount < filledRows.Length)
            {
                Array.Resize(ref filledRows, filledRowsCount);
            }

            return (filledRowsCount > 0);
        }

        private bool RowHasEmpty(int row)
        {
            bool hasEmpty = false;

            for (int column = _width - 1; column >= 0; --column)
            {
                if (_cells[column, row] == 0)
                {
                    hasEmpty = true;
                    break;
                }
            }

            return hasEmpty;
        }

        public void RemoveFilledRow(int rowToRemove)
        {
            int firstRow = _firstEmptyRow + 1;
            for (int row = rowToRemove; row >= firstRow; row--)
            {
                for (int column = 0; column < _width; column++)
                {
                    _cells[column, row] = _cells[column, row - 1];
                }
            }
            ++_firstEmptyRow;
        }

        public bool IsPositionPossible(Shape sh)
        {
            bool positionPossible = true;

            foreach (var item in sh)
            {
                var left = item.X;
                var top = item.Y;

                if (left >= _width
                    || left < 0
                    || top >= _height
                    || _cells[left, top] != 0)
                {
                    positionPossible = false;
                    break;
                }

            }

            return positionPossible;
        }

        private int[,] _cells;
        private int _firstEmptyRow;
    }
}
