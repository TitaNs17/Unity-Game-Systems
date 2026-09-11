using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public Transform spawnPoint;

    [Header("Spawn Settings")]
    public float spawnDelay = 2f;     
    public float spawnInterval = 5f;  

    void Start()
    {
        InvokeRepeating(nameof(Spawn), spawnDelay, spawnInterval);
    }

    void Spawn()
    {
        Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
    }
}
