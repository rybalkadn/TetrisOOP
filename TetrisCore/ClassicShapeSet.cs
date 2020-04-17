using System.Drawing;

namespace TetrisCore
{
    public class ClassicShapeSet : IShapeSet
    {
        public ClassicShapeSet()
        {
            _shapes = new Shape[SHAPE_KIND_COUNT];
            Initialize();
        }

        #region IShapeSet Members

        public int MaxPointsInShape
        {
            get
            {
                return MAX_POINTS_IN_SHAPE;
            }
        }

        public int ShapeKindCount
        {
            get
            {
                return SHAPE_KIND_COUNT;
            }
        }

        public int MaxShapeLength
        {
            get
            {
                return MAX_SHAPE_LENGTH;
            }
        }

        public void CopyTo(int index, ref Shape sh)
        {
            _shapes[index].CopyTo(ref sh);
        }

        #endregion

        private void Initialize()
        {
            _shapes[0] = new Shape(position: new Point(0, 0), kind: 1, capacity: MAX_POINTS_IN_SHAPE, pointCount: MAX_POINTS_IN_SHAPE,
                offsets: new Point[] { new Point(-1, -1), new Point(0, -1), new Point(0, 0), new Point(0, 1) }); // rLeft

            _shapes[1] = new Shape(position: new Point(0, 0), kind: 2, capacity: MAX_POINTS_IN_SHAPE, pointCount: MAX_POINTS_IN_SHAPE,
                offsets: new Point[] { new Point(1, -1), new Point(0, -1), new Point(0, 0), new Point(0, 1) }); // rRight

            _shapes[2] = new Shape(position: new Point(0, 0), kind: 3, capacity: MAX_POINTS_IN_SHAPE, pointCount: MAX_POINTS_IN_SHAPE,
                offsets: new Point[] { new Point(-1, 0), new Point(0, 0), new Point(0, 1), new Point(1, 1) }); // SnakeLeft

            _shapes[3] = new Shape(position: new Point(0, 0), kind: 4, capacity: MAX_POINTS_IN_SHAPE, pointCount: MAX_POINTS_IN_SHAPE,
                offsets: new Point[] { new Point(-1, 1), new Point(0, 1), new Point(0, 0), new Point(1, 0) });  // SnakeRight

            _shapes[4] = new Shape(position: new Point(0, 0), kind: 5, capacity: MAX_POINTS_IN_SHAPE, pointCount: MAX_POINTS_IN_SHAPE,
                offsets: new Point[] { new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1) }); // Square

            _shapes[5] = new Shape(position: new Point(0, 0), kind: 6, capacity: MAX_POINTS_IN_SHAPE, pointCount: MAX_POINTS_IN_SHAPE,
                offsets: new Point[] { new Point(0, -1), new Point(0, 0), new Point(0, 1), new Point(0, 2) });  // Strait

            _shapes[6] = new Shape(position: new Point(0, 0), kind: 7, capacity: MAX_POINTS_IN_SHAPE, pointCount: MAX_POINTS_IN_SHAPE,
                offsets: new Point[] { new Point(-1, 0), new Point(0, 0), new Point(1, 0), new Point(0, -1) });  // TLetter
            /* Shapes:
             * 1 - **   2 - **  3 - **   4 -  **  5 - **  6 - *  7 -  *
             *      *       *        **      **       **      *      ***
             *      *       *                                 *
             *                                                *
             */
        }

        private const int SHAPE_KIND_COUNT = 7;
        private const int MAX_POINTS_IN_SHAPE = 4;
        private const int MAX_SHAPE_LENGTH = 4;
        private Shape[] _shapes;
    }
}
