using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    private NavMeshAgent agent;
    public float eatingTime = 8f;

    private SeatPoint assignedSeat;
    private bool hasArrived = false;
    private Vector3 exitPosition; // Çıkış noktası
    private bool returning = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Spawn noktasını çıkış olarak belirle
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
            // Çıkış noktasına ulaştı mı?
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                Debug.Log("Müşteri dükkandan çıktı.");
                Destroy(gameObject);
            }
            return; // Çıkış yolundaysa aşağıyı kontrol etme
        }

        // Masaya varış kontrolü
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

        // Koltuğu boşalt
        if (assignedSeat != null) assignedSeat.isOccupied = false;

        // Çıkışa yönlendir
        returning = true;
        agent.isStopped = false;
        agent.SetDestination(exitPosition);

        Debug.Log("Yemek bitti, çıkışa gidiliyor: " + exitPosition);
    }
}