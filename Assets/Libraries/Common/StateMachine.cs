using System;

namespace Pospec.Common
{
    // https://unity.com/resources/level-up-your-code-with-game-programming-patterns
    public class StateMachine<T> where T : IState
    {
        public T CurrentState { get; private set; }
        // event to notify other objects of the state change
        public event Action<T> stateChanged;

        // exit this state and enter another
        public void TransitionTo(T nextState)
        {
            if (CurrentState != null)
                CurrentState.Exit();
            CurrentState = nextState;
            if (CurrentState != null)
                nextState.Enter();

            // notify other objects that state has changed
            stateChanged?.Invoke(nextState);
        }

        // allow the StateMachine to update this state
        public void Execute()
        {
            if (CurrentState != null)
            {
                CurrentState.Execute();
            }
        }
    }
}
