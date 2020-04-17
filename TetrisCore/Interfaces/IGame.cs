using System;

namespace TetrisCore
{
    public interface IGame : IDisposable
    {
        IShapeView Current { get; }
        IGameFieldView GameField { get; }
        bool GameOver { get; }
        int Level { get; }
        IShapeView Next { get; }
        bool Paused { get; set; }
        int Score { get; }
        IShapeSet ShapeSet { get; }

        event EventHandler OnAfterShapeChange;
        event EventHandler OnBeforeShapeChange;
        event EventHandler OnGameOver;
        event EventHandler OnLevelChanged;
        event RowsEventHandler OnRowsRemoved;
        event EventHandler OnScoreChanged;
        event MoveEventHandler OnShapeMoved;

        void Resume();
        void Start();
    }
}