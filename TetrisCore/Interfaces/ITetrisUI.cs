namespace TetrisCore
{
    public interface ITetrisUI
    {
        void Show();
        event UserEventHandler UserAction;
    }
}
