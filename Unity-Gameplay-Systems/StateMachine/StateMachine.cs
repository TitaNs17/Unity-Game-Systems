using System;

namespace UnityGameSystems.StateMachine
{
    public sealed class StateMachine
    {
        public IState Current { get; private set; }
        public event Action<IState, IState> StateChanged;

        public void SetState(IState next)
        {
            if (next == null || ReferenceEquals(Current, next))
                return;

            var previous = Current;
            previous?.Exit();
            Current = next;
            Current.Enter();
            StateChanged?.Invoke(previous, Current);
        }

        public void Tick(float deltaTime)
        {
            Current?.Tick(deltaTime);
        }

        public void Stop()
        {
            if (Current == null) return;

            var previous = Current;
            Current.Exit();
            Current = null;
            StateChanged?.Invoke(previous, null);
        }
    }
}
