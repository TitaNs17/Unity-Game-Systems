using System;
using UnityEngine;

namespace UnityGameSystems.AI
{
    public readonly struct NoiseEvent
    {
        public Vector3 Position { get; }
        public float Radius { get; }
        public GameObject Source { get; }

        public NoiseEvent(Vector3 position, float radius, GameObject source)
        {
            Position = position;
            Radius = Mathf.Max(0f, radius);
            Source = source;
        }
    }

    public static class NoiseSystem
    {
        public static event Action<NoiseEvent> Emitted;

        public static void Emit(Vector3 position, float radius, GameObject source = null)
        {
            if (radius <= 0f) return;
            Emitted?.Invoke(new NoiseEvent(position, radius, source));
        }
    }

    public sealed class NoiseEmitter : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float radius = 8f;

        public void Emit()
        {
            NoiseSystem.Emit(transform.position, radius, gameObject);
        }

        public void Emit(float customRadius)
        {
            NoiseSystem.Emit(transform.position, customRadius, gameObject);
        }
    }
}
