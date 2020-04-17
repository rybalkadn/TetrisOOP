using System;
using System.Collections.Generic;

namespace TetrisCore
{
    public class MoveEventArgs : EventArgs
    {
        public IEnumerable<Cell> Cells;

        public MoveEventArgs(IEnumerable<Cell> cells)
        {
            Cells = cells;
        }
    }
}
