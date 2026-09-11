using UnityEngine;
using UnityEngine.AI;

namespace UnityGameSystems.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class CrowdAgentTuner : MonoBehaviour
    {
        [SerializeField] private ObstacleAvoidanceType avoidanceQuality = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        [SerializeField, Range(0, 99)] private int minPriority = 25;
        [SerializeField, Range(0, 99)] private int maxPriority = 75;
        [SerializeField, Min(0f)] private float personalSpace = 0.35f;

        private NavMeshAgent agent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            Apply();
        }

        public void Apply()
        {
            if (agent == null)
                agent = GetComponent<NavMeshAgent>();

            if (agent == null) return;

            var low = Mathf.Min(minPriority, maxPriority);
            var high = Mathf.Max(minPriority, maxPriority);
            agent.avoidancePriority = Random.Range(low, high + 1);
            agent.obstacleAvoidanceType = avoidanceQuality;

            if (personalSpace > 0f)
                agent.radius = Mathf.Max(agent.radius, personalSpace);
        }

        private void OnValidate()
        {
            minPriority = Mathf.Clamp(minPriority, 0, 99);
            maxPriority = Mathf.Clamp(maxPriority, 0, 99);
            personalSpace = Mathf.Max(0f, personalSpace);
        }
    }
}
