using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrisCore;


namespace TetrisOOPConsole
{
    class TetrisUI : ITetrisUI
    {

        public TetrisUI()
        {
            EMPTY_COLOR = ConsoleColor.Black;
            _borderColor = ConsoleColor.White;
        }

        public void Initialize(IGame g)
        {
            if (_game != null)
            {
                _game.Dispose();
            }

            _game = g;
            SetLayout();

            _game.OnBeforeShapeChange += Game_BeforeShapeChange;
            _game.OnAfterShapeChange += Game_AfterShapeChange;
            _game.OnScoreChanged += Game_OnScoreChange;
            _game.OnLevelChanged += Game_OnLevelChange;
            _game.OnShapeMoved += Game_OnShapeMove;
            _game.OnRowsRemoved += Game_OnRowsRemove;


            _shapeColors = new ConsoleColor[_game.ShapeSet.ShapeKindCount + 1];
            _shapeColors[0] = EMPTY_COLOR;

            // палитра цветов для фигур
            ConsoleColor[] palette = { ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Blue, ConsoleColor.Magenta, ConsoleColor.White, ConsoleColor.Red };

            int shapeIndex = _game.ShapeSet.ShapeKindCount;
            int colorIndex = 0;
            int maxColorIndex = palette.Length - 1;

            // Assign colors to shape kinds
            do
            {
                _shapeColors[shapeIndex] = palette[colorIndex];
                --shapeIndex;
                colorIndex = (colorIndex == maxColorIndex) ? 0 : colorIndex + 1;
            } while (shapeIndex >= 1);

        }

        private void SetLayout()
        {
            int areasPadding = 2;
            _gameFieldLeft = 1;
            _gameFieldTop = 2;
            _nextShapeFieldLeft = _gameFieldLeft + _game.GameField.Width * POINT_WIDTH + 2 + areasPadding;
            _nextShapeFieldTop = _gameFieldTop;

            _nextShapeFieldWidth = (_game.ShapeSet.MaxShapeLength + 2) * POINT_WIDTH;
            _nextShapeFieldHeight = (_game.ShapeSet.MaxShapeLength + 2) * POINT_HEIGHT;

            _scoreLeft = _nextShapeFieldLeft;
            _scoreTop = _nextShapeFieldTop + _nextShapeFieldHeight + 2 + areasPadding;
            _scoreWidth = 14;

            _levelLeft = _scoreLeft;
            _levelTop = _scoreTop + 1;
        }

        public event UserEventHandler UserAction
        {
            add
            {
                _onUserAction += value;
            }
            remove
            {
                _onUserAction -= value;
            }
        }

        public void Show()
        {
            _game.Paused = true;
            DrawField();
            DrawCurrentShape();

            DrawBorder(_nextShapeFieldLeft, _nextShapeFieldTop, _nextShapeFieldWidth, _nextShapeFieldHeight, _borderColor);
            DrawNextShape();

            DrawBorder(_scoreLeft, _scoreTop, _scoreWidth, 2, ConsoleColor.Cyan);
            DrawLevel();
            DrawScore();
            _game.Paused = false;
            GameLoop();
        }

        #region Process user input
        private void GameLoop()
        {
            bool isQuitGame = false;
            do
            {
                if (Console.KeyAvailable)
                {
                    ReadUserInput(out isQuitGame);
                }
                System.Threading.Thread.Sleep(GAME_QUANTUM);

            } while (!isQuitGame && !_game.GameOver);

            if (_game.GameOver)
            {
                GameOver();
            }
        }

        private void ReadUserInput(out bool isQuitGame)
        {
            isQuitGame = false;
            ConsoleKey key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.Escape)
            {
                isQuitGame = true;
                _game.Paused = true;
            }
            else
            {
                GameCommand command = TranslateCommand(key);

                if (command != GameCommand.NoCommand && _onUserAction != null)
                {
                    _onUserAction(this, new UserEventArgs(command));
                }
            }
        }

        /// <summary>
        /// Преобразовать ввод пользователя в команду игры
        /// </summary>
        /// <param name="key">Нажатая клавиша</param>
        /// <returns>Команда, соответствующая заданной клавише</returns>
        private static GameCommand TranslateCommand(ConsoleKey key)
        {
            GameCommand command;

            switch (key)
            {
                //case ConsoleKey.Escape:
                //    command = GameCommand.QuitToMenu;
                //    break;
                case ConsoleKey.Spacebar:
                    command = GameCommand.Land;
                    break;
                case ConsoleKey.LeftArrow:
                    command = GameCommand.MoveLeft;
                    break;
                case ConsoleKey.RightArrow:
                    command = GameCommand.MoveRight;
                    break;
                case ConsoleKey.UpArrow:
                    command = GameCommand.Rotate;
                    break;
                case ConsoleKey.DownArrow:
                    command = GameCommand.MoveDown;
                    break;
                default:
                    command = GameCommand.NoCommand;
                    break;
            }

            return command;
        }

        #endregion

        #region Output methods

        private void DrawCells(IEnumerable<Cell> cells)
        {
            DrawCells(_gameFieldLeft, _gameFieldTop, cells);
        }

        private void DrawCells(int x0, int y0, IEnumerable<Cell> cells)
        {
            foreach (var cell in cells)
            {
                DrawPoint(x0 + cell.X * POINT_WIDTH, y0 + cell.Y * POINT_HEIGHT, _shapeColors[cell.Kind]);
            }
        }

        private void DrawCells(int x0, int y0, IEnumerable<Cell> cells, ConsoleColor color)
        {
            foreach (var cell in cells)
            {
                DrawPoint(x0 + cell.X * POINT_WIDTH, y0 + cell.Y * POINT_HEIGHT, color);
            }
        }

        private void DrawField()
        {
            IGameFieldView field = _game.GameField;

            DrawBorder(_gameFieldLeft, _gameFieldTop, field.Width * POINT_WIDTH, field.Height * POINT_HEIGHT, _borderColor);

            for (int deltaX = field.Width - 1; deltaX >= 0; deltaX--)
            {
                for (int deltaY = field.Height - 1; deltaY >= 0; deltaY--)
                {
                    DrawPoint(_gameFieldLeft + deltaX * POINT_WIDTH, _gameFieldTop + deltaY * POINT_HEIGHT, _shapeColors[field[deltaX, deltaY]]);
                }
            }
        }

        private void DrawLevel()
        {
            Console.SetCursorPosition(_levelLeft, _levelTop);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("LEVEL:{0,7}", _game.Level);
        }

        private void DrawScore()
        {
            Console.SetCursorPosition(_scoreLeft, _scoreTop);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("SCORE:{0,7}", _game.Score);
        }

        private void DrawNextShape(bool isErase = false)
        {
            // начальная координата - относительно центра области NextShape
            int left = _nextShapeFieldLeft + (_game.ShapeSet.MaxShapeLength + 1) / 2 * POINT_WIDTH;
            int top = _nextShapeFieldTop + (_game.ShapeSet.MaxShapeLength + 1) / 2 * POINT_HEIGHT;
            if (isErase)
            {
                DrawCells(left, top, _game.Next, EMPTY_COLOR);
            }
            else
            {
                DrawCells(left, top, _game.Next);
            }
        }

        private void DrawCurrentShape()
        {
            DrawCells(_gameFieldLeft, _gameFieldTop, _game.Current);
        }

        private void RemoveRows(IEnumerable<int> rows)
        {
            _game.Paused = true;

            foreach (var row in rows)
            {
                DrawEmptyRow(row);
                System.Threading.Thread.Sleep(FALL_ANIMATE_QUANTUM);

                DrawRows(_game.GameField.FirstEmptyRow, row);
                System.Threading.Thread.Sleep(FALL_ANIMATE_QUANTUM);
            }

            _game.Paused = false;
        }

        private void DrawEmptyRow(int row)
        {
            IGameFieldView field = _game.GameField;
            for (int column = field.Width - 1; column >= 0; column--)
            {
                DrawPoint(_gameFieldLeft + column * POINT_WIDTH, _gameFieldTop + row * POINT_HEIGHT, EMPTY_COLOR);
            }
        }

        private void DrawRows(int firstRow, int lastRow)
        {
            IGameFieldView field = _game.GameField;
            for (int row = lastRow; row >= firstRow; row--)
            {
                for (int column = field.Width - 1; column >= 0; column--)
                {
                    DrawPoint(_gameFieldLeft + column * POINT_WIDTH, _gameFieldTop + row * POINT_HEIGHT, _shapeColors[field[column, row]]);
                }
            }
        }

        /// <summary>
        /// Вывести на экран одну "точку" игрового поля
        /// </summary>
        /// <param name="x">Левая координата точки</param>
        /// <param name="y">Верхняя координата точки</param>
        /// <param name="color">Цвет точки</param>
        private void DrawPoint(int x, int y, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            for (int i = 0; i < POINT_HEIGHT; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(POINT);
            }
        }

        /// <summary>
        /// Отобразить сообщение "Игра окончена"
        /// </summary>
        private void GameOver()
        {
            string s1 = "G A M E";
            string s2 = "O V E R";

            int left = _gameFieldLeft + _game.GameField.Width * POINT_WIDTH / 2 - s1.Length / 2;
            int top = _gameFieldTop + _game.GameField.Height * POINT_HEIGHT / 2;
            DrawBorder(left, top, s1.Length + 2, 4, ConsoleColor.Red);
            Console.SetCursorPosition(left + 1, top + 1);
            Console.Write(s1);
            Console.SetCursorPosition(left + 1, top + 2);
            Console.Write(s2);

            DateTime start = DateTime.Now;

            do
            {
                System.Threading.Thread.Sleep(1);
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    break;
                }
            } while (DateTime.Now < (start + MAX_DELAY));
        }

        /// <summary>
        /// Нарисовать рамку вокруг заданной области экрана
        /// </summary>
        /// <param name="left">Левая координата поля</param>
        /// <param name="top">Верхняя координата поля</param>
        /// <param name="width">Ширина поля</param>
        /// <param name="height">Высота поля</param>
        /// <param name="borderColor">Цвет рамки</param>
        private void DrawBorder(int left, int top, int width, int height, ConsoleColor borderColor)
        {
            int padWidth = 1 + width; // один символ левого угла рамки плюс width символов горизонтальной рамки
            string up = "\u2554".PadRight(padWidth, '\u2550') + '\u2557';
            string middle = "\u2551".PadRight(padWidth) + '\u2551';
            string bottom = "\u255A".PadRight(padWidth, '\u2550') + '\u255D';

            Console.ForegroundColor = borderColor;

            Console.SetCursorPosition(left - 1, top - 1);
            Console.WriteLine(up);

            for (int i = 0; i < height; i++)
            {
                Console.SetCursorPosition(left - 1, Console.CursorTop);
                Console.WriteLine(middle);
            }

            Console.SetCursorPosition(left - 1, Console.CursorTop);
            Console.WriteLine(bottom);
        } 

        #endregion

        #region Event Handlers

        private void Game_BeforeShapeChange(object sender, EventArgs args)
        {
            DrawNextShape(true);
        }

        private void Game_AfterShapeChange(object sender, EventArgs args)
        {
            DrawNextShape();
            DrawCurrentShape();
        }

        private void Game_OnScoreChange(object sender, EventArgs args)
        {
            DrawScore();
        }

        private void Game_OnLevelChange(object sender, EventArgs args)
        {
            DrawLevel();
        }

        private void Game_OnShapeMove(object sender, MoveEventArgs args)
        {
            DrawCells(args.Cells);
        }

        private void Game_OnRowsRemove(object sender, RowsEventArgs args)
        {
            RemoveRows(args.Rows);
        }

        #endregion

        private const int POINT_WIDTH = 4; // ширина одной ячейки фигурки/ одной ячейки игрового поля
        private const int POINT_HEIGHT = 2; // высота одной ячейки фигурки/ одной ячейки игрового поля
        private const string POINT = "\u2588\u2588\u2588\u2588"; // залитый квадрат для отображения одной ячейки игрового поля / фигуры // (char)0x + (char)0x2588; // 
        private int FALL_ANIMATE_QUANTUM = 150; // длительность задержки при анимации удаления строк. в миллисекундах
        private const int GAME_QUANTUM = 5; // минимальный квант времени в игре. в миллисекундах
        private TimeSpan MAX_DELAY = new TimeSpan(0, 0, 10);

        private IGame _game;
        private UserEventHandler _onUserAction;
        private ConsoleColor[] _shapeColors;
        private readonly ConsoleColor EMPTY_COLOR;
        private readonly ConsoleColor _borderColor;

        private int _gameFieldLeft;
        private int _gameFieldTop;
        private int _nextShapeFieldLeft;
        private int _nextShapeFieldTop;
        private int _nextShapeFieldWidth;
        private int _nextShapeFieldHeight;

        private int _scoreLeft;
        private int _scoreTop;
        private int _levelLeft;
        private int _levelTop;
        private int _scoreWidth;
    }
}

