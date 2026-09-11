using System;
using System.Collections.Generic;

namespace UnityGameSystems.AI
{
    public interface IAIState
    {
        void Enter();
        void Tick(float deltaTime);
        void Exit();
    }

    public sealed class AIStateMachine
    {
        private readonly Dictionary<Type, IAIState> states = new Dictionary<Type, IAIState>();

        public IAIState Current { get; private set; }
        public event Action<IAIState, IAIState> Changed;

        public void Register<T>(T state) where T : class, IAIState
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            states[typeof(T)] = state;
        }

        public bool Change<T>() where T : class, IAIState
        {
            if (!states.TryGetValue(typeof(T), out var next) || ReferenceEquals(Current, next))
                return false;

            var previous = Current;
            previous?.Exit();
            Current = next;
            Current.Enter();
            Changed?.Invoke(previous, Current);
            return true;
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
            Changed?.Invoke(previous, null);
        }
    }
}
