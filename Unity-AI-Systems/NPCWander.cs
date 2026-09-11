using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCWander : MonoBehaviour
{
    [Header("NPC Lifecycle")]
    [SerializeField, Min(1)] private int minHedefSayisi = 5;
    [SerializeField, Min(1)] private int maxHedefSayisi = 12;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float minBekleme = 1f;
    [SerializeField, Min(0f)] private float maxBekleme = 3f;
    [SerializeField, Min(0.05f)] private float arrivalTolerance = 0.2f;
    [SerializeField] private string walkingParameter = "isWalking";

    private NavMeshAgent agent;
    private Animator anim;
    private List<Transform> waypoints;
    private NPCManager manager;
    private int toplamGezilecekNokta;
    private int gezilenNoktaSayisi;
    private bool isWaiting;
    private bool initialized;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        var min = Mathf.Max(1, minHedefSayisi);
        var max = Mathf.Max(min, maxHedefSayisi);
        toplamGezilecekNokta = Random.Range(min, max + 1);
    }

    public void Init(List<Transform> wpListesi, NPCManager managerRef)
    {
        waypoints = wpListesi;
        manager = managerRef;
        initialized = waypoints != null && waypoints.Count > 0;

        if (initialized)
            YeniHedefeGit();
    }

    private void Update()
    {
        if (!initialized || isWaiting || agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            SetWalking(false);
            return;
        }

        var moving = !agent.pathPending && agent.velocity.sqrMagnitude > 0.01f && agent.remainingDistance > agent.stoppingDistance;
        SetWalking(moving);

        if (agent.pathPending)
            return;

        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            YeniHedefeGit();
            return;
        }

        var threshold = Mathf.Max(agent.stoppingDistance, arrivalTolerance);
        if (agent.remainingDistance > threshold)
            return;

        gezilenNoktaSayisi++;
        if (gezilenNoktaSayisi >= toplamGezilecekNokta)
            MahalledenAyril();
        else
            StartCoroutine(WaitAndMove());
    }

    private IEnumerator WaitAndMove()
    {
        isWaiting = true;
        SetWalking(false);

        var min = Mathf.Min(minBekleme, maxBekleme);
        var max = Mathf.Max(minBekleme, maxBekleme);
        if (max > 0f)
            yield return new WaitForSeconds(Random.Range(min, max));

        isWaiting = false;
        YeniHedefeGit();
    }

    private void YeniHedefeGit()
    {
        if (waypoints == null || waypoints.Count == 0 || agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        for (int attempt = 0; attempt < waypoints.Count; attempt++)
        {
            var target = waypoints[Random.Range(0, waypoints.Count)];
            if (target != null && agent.SetDestination(target.position))
                return;
        }
    }

    private void MahalledenAyril()
    {
        manager?.NotifyDespawned(gameObject);
        if (manager != null)
            manager.SpawnNPC();

        Destroy(gameObject);
    }

    private void SetWalking(bool value)
    {
        if (anim != null && !string.IsNullOrWhiteSpace(walkingParameter))
            anim.SetBool(walkingParameter, value);
    }
}
