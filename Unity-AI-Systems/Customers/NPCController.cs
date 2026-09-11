using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    public enum NPCState
    {
        WalkingInStreet,
        GoingToShop,
        WaitingForOrder,
        Eating,
        Leaving
    }

    [Header("Customer")]
    [SerializeField, Min(0f)] private float eatingTime = 8f;
    [SerializeField, Min(0.05f)] private float arrivalTolerance = 0.35f;
    [SerializeField, Range(0f, 1f)] private float shopVisitChance = 0.5f;

    [Header("Animation")]
    [SerializeField] private string walkingParameter = "isWalking";
    [SerializeField] private string sitTrigger = "sittin";
    [SerializeField] private string standTrigger = "getup";
    [SerializeField] private string sittingIdleState = "Sitting idle";

    [Header("Order")]
    public OrderData myOrder;

    public NPCState currentState = NPCState.WalkingInStreet;

    private NavMeshAgent agent;
    private Animator anim;
    private List<Transform> streetWaypoints;
    private Vector3 spawnPoint;
    private Quaternion spawnRotation;
    private SeatPoint assignedSeat;
    private Coroutine activeRoutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        spawnPoint = transform.position;
        spawnRotation = transform.rotation;

        if (agent != null)
            agent.avoidancePriority = Random.Range(30, 70);
    }

    public void Init(List<Transform> waypoints)
    {
        streetWaypoints = waypoints;
        currentState = NPCState.WalkingInStreet;
        SetRandomStreetDestination();
    }

    private void Update()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh || agent.pathPending)
            return;

        SetWalking(agent.velocity.sqrMagnitude > 0.01f && !agent.isStopped);

        if (currentState == NPCState.WalkingInStreet || currentState == NPCState.GoingToShop || currentState == NPCState.Leaving)
        {
            if (HasReachedDestination() || agent.pathStatus == NavMeshPathStatus.PathInvalid)
                HandleArrival();
        }
    }

    private bool HasReachedDestination()
    {
        var threshold = Mathf.Max(agent.stoppingDistance, arrivalTolerance);
        return agent.remainingDistance <= threshold;
    }

    private void HandleArrival()
    {
        switch (currentState)
        {
            case NPCState.WalkingInStreet:
                if (Random.value <= shopVisitChance)
                    TryEnterShop();
                else
                    RestartRoutine(WaitAtWaypoint());
                break;

            case NPCState.GoingToShop:
                RestartRoutine(OrderEatAndLeave());
                break;

            case NPCState.Leaving:
                Destroy(gameObject);
                break;
        }
    }

    private void SetRandomStreetDestination()
    {
        if (streetWaypoints == null || streetWaypoints.Count == 0 || agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        for (int attempt = 0; attempt < streetWaypoints.Count; attempt++)
        {
            var waypoint = streetWaypoints[Random.Range(0, streetWaypoints.Count)];
            if (waypoint == null) continue;

            var randomCircle = Random.insideUnitCircle * 2.5f;
            var target = waypoint.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

            if (NavMesh.SamplePosition(target, out var hit, 3f, agent.areaMask) && agent.SetDestination(hit.position))
            {
                currentState = NPCState.WalkingInStreet;
                agent.isStopped = false;
                return;
            }
        }
    }

    private IEnumerator WaitAtWaypoint()
    {
        if (agent != null && agent.enabled)
            agent.isStopped = true;

        SetWalking(false);
        yield return new WaitForSeconds(Random.Range(1f, 3f));

        if (agent != null && agent.enabled)
            agent.isStopped = false;

        SetRandomStreetDestination();
        activeRoutine = null;
    }

    private void TryEnterShop()
    {
        if (SeatingManager.Instance == null)
        {
            SetRandomStreetDestination();
            return;
        }

        assignedSeat = SeatingManager.Instance.GetAndReserveFreeSeat(gameObject);
        if (assignedSeat == null)
        {
            SetRandomStreetDestination();
            return;
        }

        currentState = NPCState.GoingToShop;
        if (!MoveTo(assignedSeat.transform.position))
        {
            ReleaseSeat();
            SetRandomStreetDestination();
        }
    }

    private IEnumerator OrderEatAndLeave()
    {
        currentState = NPCState.WaitingForOrder;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }

        if (assignedSeat != null)
            transform.SetPositionAndRotation(assignedSeat.transform.position, assignedSeat.transform.rotation);

        SetWalking(false);
        TriggerAnimation(sitTrigger);

        if (OrderManager.Instance != null)
            myOrder = OrderManager.Instance.CreateRandomOrder();

        if (myOrder != null)
        {
            while (!myOrder.isCompleted && myOrder.remainingTime > 0f)
            {
                myOrder.remainingTime = Mathf.Max(0f, myOrder.remainingTime - Time.deltaTime);
                yield return null;
            }

            if (myOrder.isCompleted)
            {
                currentState = NPCState.Eating;
                if (anim != null && !string.IsNullOrWhiteSpace(sittingIdleState))
                    anim.Play(sittingIdleState);

                if (eatingTime > 0f)
                    yield return new WaitForSeconds(eatingTime);
            }
            else if (OrderManager.Instance != null)
            {
                OrderManager.Instance.activeOrders.Remove(myOrder);
            }
        }

        TriggerAnimation(standTrigger);
        yield return new WaitForSeconds(1.2f);

        ReleaseSeat();
        myOrder = null;
        currentState = NPCState.Leaving;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            if (!agent.SetDestination(spawnPoint))
                Destroy(gameObject);
        }
        else
        {
            transform.SetPositionAndRotation(spawnPoint, spawnRotation);
            Destroy(gameObject);
        }

        activeRoutine = null;
    }

    private bool MoveTo(Vector3 position)
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return false;

        agent.isStopped = false;
        return agent.SetDestination(position);
    }

    private void ReleaseSeat()
    {
        if (assignedSeat == null) return;

        SeatingManager.Instance?.ReleaseSeat(assignedSeat, gameObject);
        assignedSeat = null;
    }

    private void RestartRoutine(IEnumerator routine)
    {
        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(routine);
    }

    private void SetWalking(bool value)
    {
        if (anim != null && !string.IsNullOrWhiteSpace(walkingParameter))
            anim.SetBool(walkingParameter, value);
    }

    private void TriggerAnimation(string trigger)
    {
        if (anim != null && !string.IsNullOrWhiteSpace(trigger))
            anim.SetTrigger(trigger);
    }

    private void OnDestroy()
    {
        ReleaseSeat();
    }
}
