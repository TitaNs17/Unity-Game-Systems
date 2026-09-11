using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameSystems.AI
{
    [DisallowMultipleComponent]
    public sealed class VisionSensor : MonoBehaviour
    {
        [SerializeField] private Transform eye;
        [SerializeField, Min(0.1f)] private float range = 12f;
        [SerializeField, Range(1f, 360f)] private float fieldOfView = 110f;
        [SerializeField] private LayerMask targetMask = ~0;
        [SerializeField] private LayerMask obstructionMask = ~0;
        [SerializeField, Min(0.02f)] private float scanInterval = 0.15f;
        [SerializeField, Min(1)] private int bufferSize = 16;

        private Collider[] buffer;
        private float nextScanTime;
        private Transform currentTarget;

        public Transform CurrentTarget => currentTarget;
        public bool HasTarget => currentTarget != null;

        public event Action<Transform> TargetAcquired;
        public event Action<Transform> TargetLost;

        private void Awake()
        {
            if (eye == null)
                eye = transform;

            buffer = new Collider[Mathf.Max(1, bufferSize)];
        }

        private void Update()
        {
            if (Time.time < nextScanTime)
                return;

            nextScanTime = Time.time + scanInterval;
            Scan();
        }

        public Transform Scan()
        {
            var found = FindBestTarget();
            if (found == currentTarget)
                return currentTarget;

            var previous = currentTarget;
            currentTarget = found;

            if (previous != null)
                TargetLost?.Invoke(previous);

            if (currentTarget != null)
                TargetAcquired?.Invoke(currentTarget);

            return currentTarget;
        }

        public bool CanSee(Transform target)
        {
            if (target == null || eye == null)
                return false;

            var origin = eye.position;
            var toTarget = target.position - origin;
            var distance = toTarget.magnitude;

            if (distance <= 0.001f || distance > range)
                return false;

            var direction = toTarget / distance;
            if (Vector3.Angle(eye.forward, direction) > fieldOfView * 0.5f)
                return false;

            if (Physics.Raycast(origin, direction, out var hit, distance, obstructionMask, QueryTriggerInteraction.Ignore))
            {
                return hit.transform == target || hit.transform.IsChildOf(target) || target.IsChildOf(hit.transform);
            }

            return true;
        }

        private Transform FindBestTarget()
        {
            if (eye == null)
                return null;

            var count = Physics.OverlapSphereNonAlloc(eye.position, range, buffer, targetMask, QueryTriggerInteraction.Collide);
            Transform best = null;
            var bestScore = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                var candidateCollider = buffer[i];
                if (candidateCollider == null)
                    continue;

                var candidate = candidateCollider.transform;
                if (candidate == transform || candidate.IsChildOf(transform))
                    continue;

                if (!CanSee(candidate))
                    continue;

                var score = (candidate.position - eye.position).sqrMagnitude;
                if (score < bestScore)
                {
                    bestScore = score;
                    best = candidate;
                }
            }

            Array.Clear(buffer, 0, buffer.Length);
            return best;
        }

        private void OnValidate()
        {
            range = Mathf.Max(0.1f, range);
            scanInterval = Mathf.Max(0.02f, scanInterval);
            bufferSize = Mathf.Max(1, bufferSize);
        }
    }
}
