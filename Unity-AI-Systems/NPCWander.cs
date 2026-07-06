using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class NPCWander : MonoBehaviour
{
    private NavMeshAgent agent;
    private List<Transform> waypoints;
    private NPCManager manager;
    private Animator anim; 

    [Header("NPC Yaşam Döngüsü")]
    public int minHedefSayisi = 5;
    public int maxHedefSayisi = 12;
    private int toplamGezilecekNokta;
    private int gezilenNoktaSayisi = 0;

    [Header("Hareket Ayarları")]
    public float minBekleme = 1f;
    public float maxBekleme = 3f;

    private bool isWaiting = false;

   
    private string isWalking = "isWalking";

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>(); 
        
        toplamGezilecekNokta = Random.Range(minHedefSayisi, maxHedefSayisi);
    }

    public void Init(List<Transform> wpListesi, NPCManager managerRef)
    {
        waypoints = wpListesi;
        manager = managerRef;
        YeniHedefeGit();
    }

    void Update()
    {
        if (waypoints == null || isWaiting) 
        {
            
            if(anim != null) anim.SetBool(isWalking, false);
            return;
        }

       
        if (anim != null)
        {
            bool moving = agent.velocity.magnitude > 0.1f && agent.remainingDistance > agent.stoppingDistance;
            anim.SetBool(isWalking, moving);
            Debug.Log("Yürüme Durumu: " + moving);
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            gezilenNoktaSayisi++;

            if (gezilenNoktaSayisi >= toplamGezilecekNokta)
            {
                MahalledenAyril();
            }
            else
            {
                StartCoroutine(WaitAndMove());
            }
        }
    }

    IEnumerator WaitAndMove()
    {
        isWaiting = true;
       
        if(anim != null) anim.SetBool(isWalking, false);
        
        yield return new WaitForSeconds(Random.Range(minBekleme, maxBekleme));
        
        YeniHedefeGit();
        isWaiting = false;
    }

    void YeniHedefeGit()
    {
        if (waypoints.Count == 0) return;
        int rastgeleIndex = Random.Range(0, waypoints.Count);
        agent.SetDestination(waypoints[rastgeleIndex].position);
    }

    void MahalledenAyril()
    {
        manager.SpawnNPC();
        Destroy(gameObject);
    }
}