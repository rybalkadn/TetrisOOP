using System;
using System.Collections.Generic;

namespace TetrisCore
{
    public class RowsEventArgs : EventArgs
    {
        public IEnumerable<int> Rows;

        public RowsEventArgs(IEnumerable<int> rows)
        {
            Rows = rows;
        }
    }
}
