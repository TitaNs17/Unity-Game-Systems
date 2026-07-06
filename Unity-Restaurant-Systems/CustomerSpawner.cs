using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public Transform spawnPoint;

    [Header("Spawn Settings")]
    public float spawnDelay = 2f;      // Oyun başı ilk müşteri spawmı
    public float spawnInterval = 5f;   // Kaç saniyede bir yeni müşteri

    void Start()
    {
        InvokeRepeating(nameof(Spawn), spawnDelay, spawnInterval);
    }

    void Spawn()
    {
        Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
    }
}
