using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;
using System.ComponentModel;

namespace TetrisCore
{
    public class Game : IGame
    {
        public Game(int width, int height, IShapeGenerator generator, GameLevel[] levels, ITetrisUI ui, ISynchronizeInvoke syncObj)
        {
            _isInProgress = false;
            _gameField = new GameField(width, height);
            _generator = generator;
            _UI = ui;
            _UI.UserAction += OnUserAction;
            _shapeStartPosition = new Point(width / 2, 2);
            _current = new Shape(_generator.ShapeSet.MaxPointsInShape);
            _next = new Shape(_generator.ShapeSet.MaxPointsInShape);
            _modified = new Shape(_generator.ShapeSet.MaxPointsInShape);
            _levels = (GameLevel[])levels.Clone();
            _fallTimer = new Timer();
            _fallTimer.Interval = _levels[_levelIndex].FallDelay.TotalMilliseconds;
            _fallTimer.Elapsed += OnTimeToFall;
            if (syncObj != null)
            {
                _fallTimer.SynchronizingObject = syncObj;
            }
        }

        #region Properties

        private GameField _gameField;
        private Shape _current;
        private Shape _next;
        private bool _gameOver;
        private int _score;
        private int _levelIndex;
        private GameLevel[] _levels;

        public IGameFieldView GameField
        {
            get
            {
                return _gameField;
            }
        }

        public bool Paused
        {
            get { return !_fallTimer.Enabled; }
            set { _fallTimer.Enabled = !value; }
        }

        public bool GameOver
        {
            get { return _gameOver; }
            private set { _gameOver = value; }
        }

        public IShapeView Current
        {
            get
            {
                return _current;
            }
        }

        public IShapeView Next
        {
            get
            {
                return _next;
            }
        }

        public int Score
        {
            get
            {
                return _score;
            }
        }

        public int Level
        {
            get
            {
                return _levels[_levelIndex].Level;
            }
        }

        public IShapeSet ShapeSet
        {
            get
            {
                return _generator.ShapeSet;
            }
        }

        #endregion

        public void Start()
        {
            _generator.GetNext(ref _next);
            AssignNext();
            _score = 0;
            _levelIndex = 0;
            _isInProgress = true;

            Paused = false;
            _UI.Show();
        }

        public void Resume()
        {
            Paused = false;
            _UI.Show();
        }


        #region Events

        private EventHandler _onGameOver;
        private EventHandler _onBeforeShapeChange;
        private EventHandler _onAfterShapeChange;
        private EventHandler _onScoreChange;
        private EventHandler _onLevelChange;
        private MoveEventHandler _onShapeMove;
        private RowsEventHandler _onRowsRemove;

        public event EventHandler OnGameOver
        {
            add
            {
                _onGameOver += value;
            }
            remove
            {
                _onGameOver -= value;
            }
        }

        public event EventHandler OnBeforeShapeChange
        {
            add
            {
                _onBeforeShapeChange += value;
            }
            remove
            {
                _onBeforeShapeChange -= value;
            }
        }

        public event EventHandler OnAfterShapeChange
        {
            add
            {
                _onAfterShapeChange += value;
            }
            remove
            {
                _onAfterShapeChange -= value;
            }
        }

        public event EventHandler OnScoreChanged
        {
            add
            {
                _onScoreChange += value;
            }
            remove
            {
                _onScoreChange -= value;
            }
        }

        public event EventHandler OnLevelChanged
        {
            add
            {
                _onLevelChange += value;
            }
            remove
            {
                _onLevelChange -= value;
            }
        }

        public event MoveEventHandler OnShapeMoved
        {
            add
            {
                _onShapeMove += value;
            }
            remove
            {
                _onShapeMove -= value;
            }
        }

        public event RowsEventHandler OnRowsRemoved
        {
            add
            {
                _onRowsRemove += value;
            }
            remove
            {
                _onRowsRemove -= value;
            }
        }

        #endregion

        #region Event handlers
        private void OnUserAction(object sender, UserEventArgs args)
        {
            if (!_isInProgress)
            {
                throw new InvalidOperationException();
            }

            lock (this)
            {
                bool needRedraw = false;
                switch (args.Action)
                {
                    case GameCommand.MoveLeft:
                        needRedraw = true;
                        _current.CopyTo(ref _modified);
                        _modified.Move(dx: -1, dy: 0);
                        break;
                    case GameCommand.MoveRight:
                        needRedraw = true;
                        _current.CopyTo(ref _modified);
                        _modified.Move(dx: 1, dy: 0);
                        break;
                    case GameCommand.MoveDown:
                        needRedraw = true;
                        _current.CopyTo(ref _modified);
                        _modified.Move(dx: 0, dy: 1);
                        break;
                    case GameCommand.Rotate:
                        needRedraw = true;
                        _current.CopyTo(ref _modified);
                        _modified.Rotate();
                        break;
                    case GameCommand.Land:
                        needRedraw = true;
                        _current.CopyTo(ref _modified);
                        // UNDONE: Sacrificed landing animation (in comparision with ConsoleTetris)
                        do
                        {
                            _modified.Move(dx: 0, dy: 1);
                        } while (_gameField.IsPositionPossible(_modified));

                        // HACK: return to last possible position. Could modify IsPositionPossible to check position with offset (like in ConsoleTetris)
                        _modified.Move(dx: 0, dy: -1);
                        break;
                    default:
                        break;
                }

                if (needRedraw && _gameField.IsPositionPossible(_modified))
                {
                    _onShapeMove(this, new MoveEventArgs(_modified - _current));
                    _modified.CopyTo(ref _current);
                }
            }
        }

        private void OnTimeToFall(Object source, ElapsedEventArgs e)
        {
            if (!_isInProgress)
            {
                throw new InvalidOperationException();
            }

            lock (this)
            {
                if (CheckShapeLanded(_current))
                {
                    if (_onBeforeShapeChange != null)
                    {
                        _onBeforeShapeChange(this, new EventArgs());
                    }

                    AssignNext();

                    if (_gameField.IsPositionPossible(_current))
                    {
                        if (_onAfterShapeChange != null)
                        {
                            _onAfterShapeChange(this, new EventArgs());
                        }
                    }
                    else
                    {
                        _gameOver = true;
                        Paused = true;
                        _isInProgress = false;

                        if (_onGameOver != null)
                        {
                            _onGameOver(this, new EventArgs());
                        }
                    }
                }
                else
                {
                    _current.CopyTo(ref _modified);
                    _modified.Move(dx: 0, dy: 1);

                    _onShapeMove(this, new MoveEventArgs(_modified - _current));
                    _modified.CopyTo(ref _current);
                }
            }
        }
        #endregion

        #region Utility methods

        /// <summary>
        /// Check if the shape is landed then do appropriate actions
        /// </summary>
        /// <param name="sh">Shape to check</param>
        /// <returns>true if the shape has landed, otherwise - false</returns>
        private bool CheckShapeLanded(Shape sh)
        {
            bool isLanded = false;
            if (_gameField.IsShapeLanded(_current))
            {
                isLanded = true;
                int[] filledRows;

                _gameField.AppendShape(_current);

                if (_gameField.HasFilledRows(_current.MinY, _current.MaxY, out filledRows))
                {
                    _score += CalcScore(filledRows.Length);
                    _onScoreChange(this, new EventArgs());
                    _onRowsRemove(this, new RowsEventArgs(RemoveRows(filledRows)));


                    if (_levelIndex < (_levels.Length - 1) && _score >= _levels[_levelIndex].NextLevelScore)
                    {
                        ++_levelIndex;
                        _fallTimer.Interval = _levels[_levelIndex].FallDelay.TotalMilliseconds;
                        _onLevelChange(this, new EventArgs());
                    }
                }
            }

            return isLanded;
        }

        private IEnumerable<int> RemoveRows(int[] filledRows)
        {
            for (int i = 0; i < filledRows.Length; i++)
            {
                _gameField.RemoveFilledRow(filledRows[i]);
                yield return filledRows[i];
            }
        }

        private int CalcScore(int removedRowsCount)
        {
            int score = 0;

            //1 линия — 100 очков, 2 линии — 300 очков, 3 линии — 700 очков, 4 линии — 1500 очков
            switch (removedRowsCount)
            {
                case 1:
                    score = 100;
                    break;
                case 2:
                    score = 300;
                    break;
                case 3:
                    score = 700;
                    break;
                case 4:
                    score = 1500;
                    break;
            }

            return score;
        }

        private void AssignNext()
        {
            _next.CopyTo(ref _current);
            _current.Position = _shapeStartPosition;
            _generator.GetNext(ref _next);
        }

        #endregion
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _UI.UserAction -= OnUserAction;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        private readonly Point _shapeStartPosition; // position where new shapes appear on the field
        private bool _isInProgress; // is game started and not yet finished
        private Shape _modified; //buffer shape for calculations before the real shape movement
        private ITetrisUI _UI;
        private IShapeGenerator _generator;
        private Timer _fallTimer;
    }
}
