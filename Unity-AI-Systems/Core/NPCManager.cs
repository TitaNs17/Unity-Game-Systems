using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField] private GameObject[] npcPrefabs = System.Array.Empty<GameObject>();
    [SerializeField, Min(0)] private int maxNpcSayisi = 12;
    [SerializeField] private Transform waypointParent;
    [SerializeField] private bool spawnOnStart = true;

    private readonly List<Transform> waypoints = new List<Transform>();
    private readonly HashSet<GameObject> activeNpcs = new HashSet<GameObject>();

    public int ActiveCount => activeNpcs.Count;
    public IReadOnlyList<Transform> Waypoints => waypoints;

    private void Start()
    {
        RebuildWaypoints();

        if (!spawnOnStart)
            return;

        for (int i = activeNpcs.Count; i < maxNpcSayisi; i++)
            SpawnNPC();
    }

    public void RebuildWaypoints()
    {
        waypoints.Clear();
        if (waypointParent == null) return;

        foreach (Transform child in waypointParent)
        {
            if (child != null)
                waypoints.Add(child);
        }
    }

    public bool SpawnNPC()
    {
        CleanupDestroyed();

        if (activeNpcs.Count >= maxNpcSayisi || npcPrefabs == null || npcPrefabs.Length == 0 || waypoints.Count == 0)
            return false;

        var prefab = GetRandomValidPrefab();
        if (prefab == null)
            return false;

        var spawn = waypoints[Random.Range(0, waypoints.Count)];
        var instance = Instantiate(prefab, spawn.position, spawn.rotation);
        activeNpcs.Add(instance);

        var controller = instance.GetComponent<NPCController>();
        if (controller != null)
            controller.Init(waypoints);

        var wander = instance.GetComponent<NPCWander>();
        if (wander != null)
            wander.Init(waypoints, this);

        return true;
    }

    public void NotifyDespawned(GameObject npc)
    {
        if (npc != null)
            activeNpcs.Remove(npc);
    }

    private GameObject GetRandomValidPrefab()
    {
        var valid = new List<GameObject>();
        foreach (var prefab in npcPrefabs)
            if (prefab != null) valid.Add(prefab);

        return valid.Count == 0 ? null : valid[Random.Range(0, valid.Count)];
    }

    private void CleanupDestroyed()
    {
        activeNpcs.RemoveWhere(item => item == null);
    }
}
