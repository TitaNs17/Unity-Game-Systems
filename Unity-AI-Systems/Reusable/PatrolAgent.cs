using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace UnityGameSystems.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class PatrolAgent : MonoBehaviour
    {
        [SerializeField] private Transform[] points = Array.Empty<Transform>();
        [SerializeField] private bool randomOrder = true;
        [SerializeField, Min(0f)] private float waitMin = 0.5f;
        [SerializeField, Min(0f)] private float waitMax = 2f;
        [SerializeField, Min(0.05f)] private float arrivalTolerance = 0.2f;
        [SerializeField] private bool startOnEnable = true;

        private NavMeshAgent agent;
        private Coroutine routine;
        private int sequentialIndex;

        public bool IsRunning => routine != null;
        public event Action<Transform> PointReached;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            if (startOnEnable)
                StartPatrol();
        }

        private void OnDisable()
        {
            StopPatrol();
        }

        public bool StartPatrol()
        {
            if (routine != null || agent == null || !agent.enabled || !agent.isOnNavMesh || !HasValidPoint())
                return false;

            routine = StartCoroutine(PatrolRoutine());
            return true;
        }

        public void StopPatrol()
        {
            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }

            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }
        }

        public void SetPoints(Transform[] patrolPoints)
        {
            points = patrolPoints ?? Array.Empty<Transform>();
            sequentialIndex = 0;
        }

        private IEnumerator PatrolRoutine()
        {
            while (enabled)
            {
                var point = GetNextPoint();
                if (point == null)
                {
                    routine = null;
                    yield break;
                }

                agent.isStopped = false;
                if (!agent.SetDestination(point.position))
                {
                    yield return null;
                    continue;
                }

                while (agent.enabled && agent.isOnNavMesh)
                {
                    if (!agent.pathPending)
                    {
                        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
                            break;

                        var threshold = Mathf.Max(agent.stoppingDistance, arrivalTolerance);
                        if (agent.remainingDistance <= threshold)
                            break;
                    }

                    yield return null;
                }

                PointReached?.Invoke(point);
                agent.isStopped = true;

                var min = Mathf.Min(waitMin, waitMax);
                var max = Mathf.Max(waitMin, waitMax);
                if (max > 0f)
                    yield return new WaitForSeconds(UnityEngine.Random.Range(min, max));
                else
                    yield return null;
            }

            routine = null;
        }

        private Transform GetNextPoint()
        {
            if (points == null || points.Length == 0)
                return null;

            if (randomOrder)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    var candidate = points[UnityEngine.Random.Range(0, points.Length)];
                    if (candidate != null)
                        return candidate;
                }

                return null;
            }

            for (int i = 0; i < points.Length; i++)
            {
                var candidate = points[sequentialIndex % points.Length];
                sequentialIndex++;
                if (candidate != null)
                    return candidate;
            }

            return null;
        }

        private bool HasValidPoint()
        {
            if (points == null) return false;
            foreach (var point in points)
                if (point != null) return true;
            return false;
        }
    }
}
