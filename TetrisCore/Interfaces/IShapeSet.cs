
namespace TetrisCore
{
    /// <summary>
    /// Defines a set of Shapes
    /// </summary>
    public interface IShapeSet
    {
        int ShapeKindCount { get; } // number of shape kinds in the set
        int MaxPointsInShape { get; }
        int MaxShapeLength { get; }
        void CopyTo(int index, ref Shape sh);
    }
}
