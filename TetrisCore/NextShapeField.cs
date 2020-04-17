using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisCore
{
    public class NextShapeField : IGameFieldView
    {
        private Shape _current;
        private Shape _modified;
        private int _width;
        private int _height;
        private ShapeKind[,] _cells;

        public NextShapeField(int width, int height)
        {
            _width = width;
            _height = height;
            _cells = new ShapeKind[Width, Height];
        }

        public void Initialize()
        {
            _current.Kind = TetrisShapeGenerator.Generate();
            // TODO Может, генератор сразу Shape возвращать будет?
        }
        public ShapeKind Kind
        {
            get
            {
                return _current.Kind;
            }
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

        public IEnumerable<Cell> GetCells()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Cell> GetStepDifference()
        {
            throw new NotImplementedException();
        }
    }
}
