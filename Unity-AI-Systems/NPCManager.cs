using UnityEngine;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{
    public GameObject[] npcPrefabs;
    public int maxNpcSayisi = 12;
    public Transform waypointParent;
    private List<Transform> waypoints = new List<Transform>();

    void Start()
    {
        foreach (Transform child in waypointParent) waypoints.Add(child);

        if (npcPrefabs.Length == 0 || waypoints.Count == 0) return;

        for (int i = 0; i < maxNpcSayisi; i++)
        {
            SpawnNPC();
        }
    }

    public void SpawnNPC()
    {
        Transform spawnPos = waypoints[Random.Range(0, waypoints.Count)];
        GameObject prefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];

        GameObject go = Instantiate(prefab, spawnPos.position, spawnPos.rotation);

        NPCController controller = go.GetComponent<NPCController>();
        if (controller != null) controller.Init(waypoints);
    }
}