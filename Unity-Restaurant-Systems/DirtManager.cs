using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DirtManager : MonoBehaviour
{
    [Header("--- AYARLAR ---")]
    public GameObject dirtPrefab;
    public Collider[] spawnAreas;

    [Header("--- DİNAMİK KİRLENME ---")]
    [Tooltip("Dükkan boşken kirlenme süresi")]
    public float baseSpawnInterval = 10f;
    [Tooltip("Dükkan ağzına kadar doluyken bile en hızlı kaç saniyede bir kirlensin?")]
    public float minSpawnInterval = 2f;

    public int maxDirtCount = 10;
    public float spawnHeightOffset = 0.05f;

    [Header("--- UI ---")]
    public TextMeshProUGUI hygieneText;

    public List<GameObject> currentDirts = new List<GameObject>();
    public float currentShopHygiene = 100f;
    private float timer;

    void Start()
    {
        timer = baseSpawnInterval;
        UpdateUI();
    }

    void Update()
    {
        if (currentDirts.Count < maxDirtCount)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                SpawnDirt();

                // MÜŞTERİ POPÜLASYONUNA GÖRE YENİ SÜRE HESAPLA
                CalculateNextSpawnTime();
            }
        }
    }

    void CalculateNextSpawnTime()
    {
        // SOKAKTAKİLERİ DEĞİL, SADECE DÜKKANDA OTURANLARI SAY
        NPCController[] allNPCs = FindObjectsByType<NPCController>(FindObjectsSortMode.None);
        int eatingCustomerCount = 0;

        foreach (NPCController npc in allNPCs)
        {
            // Sadece masada yemek bekleyen/yiyenleri say
            if (npc.currentState == NPCController.NPCState.Eating)
            {
                eatingCustomerCount++;
            }
        }

        if (eatingCustomerCount == 0)
        {
            // Dükkan bomboşsa çok çok nadir kirlensin. 
            // (baseSpawnInterval 10 ise, 10 * 4 = 40 saniyede bir leke çıkar)
            timer = baseSpawnInterval * 4f;
        }
        else
        {
            // İçerideki adam sayısına göre kirlenme süresini böl
            float dynamicInterval = baseSpawnInterval / eatingCustomerCount;

            // Süre çok düşmesin diye sınır koy
            timer = Mathf.Max(dynamicInterval, minSpawnInterval);
        }
    }

    void SpawnDirt()
    {
        if (dirtPrefab == null || spawnAreas.Length == 0) return;

        Collider randomArea = spawnAreas[Random.Range(0, spawnAreas.Length)];
        Bounds bounds = randomArea.bounds;

        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        Vector3 spawnPos = new Vector3(randomX, bounds.center.y + spawnHeightOffset, randomZ);

        GameObject newDirt = Instantiate(dirtPrefab, spawnPos, Quaternion.Euler(0, 0, 0));
        currentDirts.Add(newDirt);

        CalculateHygiene();
    }

    public void RemoveDirt(GameObject dirtToRemove)
    {
        foreach (Collider area in spawnAreas)
        {
            if (area != null && dirtToRemove == area.gameObject) return;
        }

        if (currentDirts.Contains(dirtToRemove))
        {
            currentDirts.Remove(dirtToRemove);
            Destroy(dirtToRemove);
            CalculateHygiene();
        }
    }

    void CalculateHygiene()
    {
        float penalty = currentDirts.Count * 10f;
        currentShopHygiene = Mathf.Clamp(100f - penalty, 0f, 100f);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (hygieneText != null)
        {
            hygieneText.text = "Hijyen: %" + currentShopHygiene.ToString("F0");
            if (currentShopHygiene > 70) hygieneText.color = Color.green;
            else if (currentShopHygiene < 40) hygieneText.color = Color.red;
            else hygieneText.color = Color.yellow;
        }
    }
}