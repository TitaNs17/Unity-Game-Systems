using System;
using System.Collections.Generic;

namespace UnityGameSystems.Core
{
    /// <summary>
    /// Small typed event bus used by gameplay systems that should not know about each other.
    /// Call Clear() when leaving a game session if non-MonoBehaviour listeners are registered.
    /// </summary>
    public static class GameEventBus
    {
        private static readonly Dictionary<Type, Delegate> Listeners = new();

        public static void Subscribe<T>(Action<T> listener)
        {
            if (listener == null) return;

            var type = typeof(T);
            if (Listeners.TryGetValue(type, out var existing))
                Listeners[type] = Delegate.Combine(existing, listener);
            else
                Listeners[type] = listener;
        }

        public static void Unsubscribe<T>(Action<T> listener)
        {
            if (listener == null) return;

            var type = typeof(T);
            if (!Listeners.TryGetValue(type, out var existing)) return;

            var remaining = Delegate.Remove(existing, listener);
            if (remaining == null)
                Listeners.Remove(type);
            else
                Listeners[type] = remaining;
        }

        public static void Publish<T>(T message)
        {
            if (Listeners.TryGetValue(typeof(T), out var callback))
                (callback as Action<T>)?.Invoke(message);
        }

        public static void Clear() => Listeners.Clear();
    }
}
