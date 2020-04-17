using System;

namespace TetrisCore
{
    public class UserEventArgs : EventArgs
    {
        public GameCommand Action { get; private set; }

        public UserEventArgs(GameCommand action)
        {
            Action = action;
        }
    }
}
