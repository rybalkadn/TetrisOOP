using System;

namespace TetrisCore
{
    public struct GameLevel
    {
        public GameLevel(int level, int nextScore, TimeSpan fallDelay)
        {
            _level = level;
            _nextLevelScore = nextScore;
            _fallDelay = fallDelay;
        }

        public TimeSpan FallDelay
        {
            get { return _fallDelay; }
            set { _fallDelay = value; }
        }

        public int NextLevelScore
        {
            get { return _nextLevelScore; }
            set { _nextLevelScore = value; }
        }

        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }

        private int _level;
        private int _nextLevelScore;
        private TimeSpan _fallDelay;
    }
}
