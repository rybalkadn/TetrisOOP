
namespace TetrisCore
{
    public interface IShapeGenerator
    {
        void GetNext(ref Shape sh);
        IShapeSet ShapeSet { get; }
    }
}
