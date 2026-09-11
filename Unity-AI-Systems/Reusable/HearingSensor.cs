using System;
using UnityEngine;

namespace UnityGameSystems.AI
{
    [DisallowMultipleComponent]
    public sealed class HearingSensor : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float hearingMultiplier = 1f;
        [SerializeField] private LayerMask obstructionMask = ~0;
        [SerializeField] private bool checkObstruction;

        public Vector3 LastHeardPosition { get; private set; }
        public GameObject LastSource { get; private set; }
        public event Action<NoiseEvent, float> Heard;

        private void OnEnable()
        {
            NoiseSystem.Emitted += HandleNoise;
        }

        private void OnDisable()
        {
            NoiseSystem.Emitted -= HandleNoise;
        }

        private void HandleNoise(NoiseEvent noise)
        {
            var effectiveRadius = noise.Radius * Mathf.Max(0.01f, hearingMultiplier);
            var delta = noise.Position - transform.position;
            var distance = delta.magnitude;

            if (distance > effectiveRadius)
                return;

            if (checkObstruction && distance > 0.001f)
            {
                if (Physics.Raycast(transform.position, delta.normalized, distance, obstructionMask, QueryTriggerInteraction.Ignore))
                    return;
            }

            LastHeardPosition = noise.Position;
            LastSource = noise.Source;
            var strength = 1f - Mathf.Clamp01(distance / effectiveRadius);
            Heard?.Invoke(noise, strength);
        }
    }
}
