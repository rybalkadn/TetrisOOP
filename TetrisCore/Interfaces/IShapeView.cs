using System.Collections.Generic;

namespace TetrisCore
{
    public interface IShapeView : IEnumerable<Cell>
    {
        int Kind { get; }
    }
}
