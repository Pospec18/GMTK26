namespace Pospec.Common
{
    // https://unity.com/resources/level-up-your-code-with-game-programming-patterns
    public interface IState
    {
        void Enter();
        void Execute();
        void Exit();
    }
}
