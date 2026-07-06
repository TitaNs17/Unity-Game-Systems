using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class NPCController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private List<Transform> streetWaypoints;
    private Vector3 spawnPoint;

    public enum NPCState { WalkingInStreet, GoingToShop, Eating, Leaving }
    public NPCState currentState = NPCState.WalkingInStreet;

    [Header("Müþteri Ayarlarý")]
    public float eatingTime = 8f;
    private SeatPoint assignedSeat;
    private bool actionTriggered = false;

    [Header("Sipariþ")]
    public OrderData myOrder;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        spawnPoint = transform.position;
        gameObject.tag = "NPC";

        if (agent != null)
            agent.avoidancePriority = Random.Range(30, 70);
    }

    public void Init(List<Transform> waypoints)
    {
        streetWaypoints = waypoints;
        SetRandomStreetDestination();
    }

    void Update()
    {
        if (currentState == NPCState.Eating) return;

        if (currentState == NPCState.GoingToShop && assignedSeat != null && !actionTriggered)
        {
            float mesafe = Vector3.Distance(transform.position, assignedSeat.transform.position);

            if (mesafe <= 2.5f)
            {
                actionTriggered = true;
                HandleArrival();
            }
        }
        else if (currentState == NPCState.WalkingInStreet)
        {
            if (agent != null && !agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance + 0.2f)
                {
                    HandleArrival();
                }
                else if (agent.pathStatus == NavMeshPathStatus.PathPartial)
                {
                    HandleArrival();
                }
            }
        }
    }

    void HandleArrival()
    {
        switch (currentState)
        {
            case NPCState.WalkingInStreet:
                if (Random.value < 0.5f) TryEnterShop();
                else StartCoroutine(WaitAtWaypoint());
                break;

            case NPCState.GoingToShop:
                currentState = NPCState.Eating;
                StartCoroutine(EatAndLeave());
                break;

            case NPCState.Leaving:
                Destroy(gameObject);
                break;
        }
    }

    void SetRandomStreetDestination()
    {
        if (streetWaypoints == null || streetWaypoints.Count == 0) return;
        if (agent == null) return;

        Vector3 targetPos = streetWaypoints[Random.Range(0, streetWaypoints.Count)].position;

        Vector2 randomCircle = Random.insideUnitCircle * 2.5f;
        Vector3 randomOffset = new Vector3(randomCircle.x, 0, randomCircle.y);

        agent.SetDestination(targetPos + randomOffset);

        if (anim != null)
            anim.SetBool("isWalking", true);
    }

    IEnumerator WaitAtWaypoint()
    {
        if (anim != null)
            anim.SetBool("isWalking", false);

        yield return new WaitForSeconds(Random.Range(1f, 3f));

        SetRandomStreetDestination();
    }

    void TryEnterShop()
    {
        if (SeatingManager.Instance == null)
        {
            Debug.LogError("SeatingManager.Instance NULL! Sahnede SeatingManager yok.");
            SetRandomStreetDestination();
            return;
        }

        assignedSeat = SeatingManager.Instance.GetAndReserveFreeSeat();

        if (assignedSeat != null)
        {
            currentState = NPCState.GoingToShop;
            actionTriggered = false;

            if (agent != null)
                agent.SetDestination(assignedSeat.transform.position);

            if (anim != null)
                anim.SetBool("isWalking", true);
        }
        else
        {
            SetRandomStreetDestination();
        }
    }

    IEnumerator EatAndLeave()
    {
        if (agent != null)
        {
            agent.ResetPath();
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (assignedSeat != null)
        {
            transform.position = assignedSeat.transform.position;
            transform.rotation = assignedSeat.transform.rotation;
        }

        if (anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.ResetTrigger("getup");
            anim.SetTrigger("sittin");
        }

        if (OrderManager.Instance == null)
        {
            Debug.LogError("OrderManager.Instance NULL! Sahnede OrderManager yok.");
            yield break;
        }

        myOrder = OrderManager.Instance.CreateRandomOrder();

        if (myOrder == null)
        {
            Debug.LogError("myOrder NULL! Sipariþ oluþturulamadý.");
            yield break;
        }

        while (myOrder != null && !myOrder.isCompleted && myOrder.remainingTime > 0)
        {
            myOrder.remainingTime -= Time.deltaTime;
            yield return null;
        }

        if (myOrder != null && myOrder.isCompleted)
        {
            if (anim != null)
                anim.Play("Sitting idle");

            yield return new WaitForSeconds(eatingTime);
        }
        else if (myOrder != null && myOrder.remainingTime <= 0)
        {
            Debug.Log("NPC Sinirlendi ve kalkýyor!");

            if (OrderManager.Instance != null && OrderManager.Instance.activeOrders.Contains(myOrder))
            {
                OrderManager.Instance.activeOrders.Remove(myOrder);
            }
        }

        if (anim != null)
        {
            anim.ResetTrigger("sittin");
            anim.SetTrigger("getup");
        }

        yield return new WaitForSeconds(1.5f);

        if (assignedSeat != null)
        {
            assignedSeat.isOccupied = false;
            assignedSeat = null;
        }

        currentState = NPCState.Leaving;

        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
            agent.SetDestination(spawnPoint);
        }

        if (anim != null)
            anim.SetBool("isWalking", true);
    }
}