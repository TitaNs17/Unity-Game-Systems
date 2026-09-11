using System;
using UnityEngine;

namespace UnityGameSystems.AI
{
    [DisallowMultipleComponent]
    public sealed class PerceptionController : MonoBehaviour
    {
        [SerializeField] private VisionSensor vision;
        [SerializeField] private HearingSensor hearing;
        [SerializeField] private SuspicionMeter suspicion = new SuspicionMeter();
        [SerializeField, Min(0f)] private float hearingSuspicion = 30f;

        public Transform VisualTarget => vision != null ? vision.CurrentTarget : null;
        public Vector3 LastInterestingPosition { get; private set; }
        public bool IsAlerted => suspicion != null && suspicion.IsAlerted;
        public float SuspicionNormalized => suspicion != null ? suspicion.Normalized : 0f;

        public event Action<Vector3> InterestChanged;
        public event Action Alerted;

        private void Reset()
        {
            vision = GetComponent<VisionSensor>();
            hearing = GetComponent<HearingSensor>();
        }

        private void OnEnable()
        {
            if (vision != null)
                vision.TargetAcquired += HandleTargetAcquired;

            if (hearing != null)
                hearing.Heard += HandleHeard;

            if (suspicion != null)
                suspicion.Alerted += HandleAlerted;
        }

        private void OnDisable()
        {
            if (vision != null)
                vision.TargetAcquired -= HandleTargetAcquired;

            if (hearing != null)
                hearing.Heard -= HandleHeard;

            if (suspicion != null)
                suspicion.Alerted -= HandleAlerted;
        }

        private void Update()
        {
            if (suspicion == null) return;

            var hasVisual = vision != null && vision.HasTarget;
            suspicion.Tick(hasVisual, Time.deltaTime);

            if (hasVisual)
            {
                LastInterestingPosition = vision.CurrentTarget.position;
                InterestChanged?.Invoke(LastInterestingPosition);
            }
        }

        private void HandleTargetAcquired(Transform target)
        {
            if (target == null) return;
            LastInterestingPosition = target.position;
            InterestChanged?.Invoke(LastInterestingPosition);
        }

        private void HandleHeard(NoiseEvent noise, float strength)
        {
            LastInterestingPosition = noise.Position;
            suspicion?.Add(hearingSuspicion * Mathf.Clamp01(strength));
            InterestChanged?.Invoke(LastInterestingPosition);
        }

        private void HandleAlerted()
        {
            Alerted?.Invoke();
        }
    }
}
