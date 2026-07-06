using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    private NavMeshAgent agent;
    public float eatingTime = 8f;

    private SeatPoint assignedSeat;
    private bool hasArrived = false;
    private Vector3 exitPosition; 
    private bool returning = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        
        exitPosition = transform.position;

        assignedSeat = SeatingManager.Instance.GetAndReserveFreeSeat();

        if (assignedSeat == null)
        {
            Debug.Log("Yer yok, gidiyorum.");
            Destroy(gameObject);
            return;
        }

        agent.SetDestination(assignedSeat.transform.position);
    }

    void Update()
    {
        if (returning)
        {
           
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                Debug.Log("Müşteri dükkandan çıktı.");
                Destroy(gameObject);
            }
            return; 
        }

        
        if (!hasArrived && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            hasArrived = true;
            StartCoroutine(EatAndReturn());
        }
    }

    private IEnumerator EatAndReturn()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(eatingTime);

      
        if (assignedSeat != null) assignedSeat.isOccupied = false;

       
        returning = true;
        agent.isStopped = false;
        agent.SetDestination(exitPosition);

        Debug.Log("Yemek bitti, çıkışa gidiliyor: " + exitPosition);
    }
}