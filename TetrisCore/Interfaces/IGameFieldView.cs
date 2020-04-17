namespace TetrisCore
{
    public interface IGameFieldView
    {
        int Height { get; }
        int Width { get; }

        /// <summary>
        /// Lowest empty row in the game field
        /// </summary>
        int FirstEmptyRow { get; }

        /// <summary>
        /// Indexer for cells of the game field
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        int this[int left, int top] { get; }
    }
}
