using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CustomerAI : MonoBehaviour
{
    [SerializeField, Min(0f)] private float eatingTime = 8f;
    [SerializeField, Min(0.05f)] private float arrivalTolerance = 0.25f;

    private NavMeshAgent agent;
    private SeatPoint assignedSeat;
    private Vector3 exitPosition;
    private bool returning;
    private bool routineStarted;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        exitPosition = transform.position;
    }

    private void Start()
    {
        if (SeatingManager.Instance == null)
        {
            Debug.LogWarning($"{name}: SeatingManager is missing. Customer will leave.", this);
            Destroy(gameObject);
            return;
        }

        assignedSeat = SeatingManager.Instance.GetAndReserveFreeSeat(gameObject);
        if (assignedSeat == null)
        {
            Destroy(gameObject);
            return;
        }

        MoveTo(assignedSeat.transform.position);
    }

    private void Update()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh || agent.pathPending)
            return;

        if (!HasReachedDestination())
            return;

        if (returning)
        {
            Destroy(gameObject);
            return;
        }

        if (!routineStarted)
        {
            routineStarted = true;
            StartCoroutine(EatAndReturn());
        }
    }

    private IEnumerator EatAndReturn()
    {
        agent.isStopped = true;

        if (assignedSeat != null)
        {
            transform.SetPositionAndRotation(assignedSeat.transform.position, assignedSeat.transform.rotation);
        }

        yield return new WaitForSeconds(eatingTime);

        if (assignedSeat != null)
        {
            SeatingManager.Instance?.ReleaseSeat(assignedSeat, gameObject);
            assignedSeat = null;
        }

        returning = true;
        agent.isStopped = false;
        MoveTo(exitPosition);
    }

    private bool HasReachedDestination()
    {
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
            return true;

        var threshold = Mathf.Max(agent.stoppingDistance, arrivalTolerance);
        return agent.remainingDistance <= threshold;
    }

    private void MoveTo(Vector3 destination)
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
            agent.SetDestination(destination);
    }

    private void OnDestroy()
    {
        if (assignedSeat != null)
            SeatingManager.Instance?.ReleaseSeat(assignedSeat, gameObject);
    }
}
