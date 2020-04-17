using System;

namespace TetrisCore
{
    /// <summary>
    /// Shape Generator
    /// Generates random shapes
    /// </summary>
    public class RandomShapeGenerator : IShapeGenerator
    {
        public RandomShapeGenerator(IShapeSet shapeSet)
        {
            _shapeSet = shapeSet;
        }

        public void GetNext(ref Shape sh)
        {
            int kind = _random.Next(0, _shapeSet.ShapeKindCount);
            _shapeSet.CopyTo(kind, ref sh);
        }

        public IShapeSet ShapeSet
        {
            get { return _shapeSet; }
        }

        private static Random _random = new Random();
        private IShapeSet _shapeSet;
    }
}
