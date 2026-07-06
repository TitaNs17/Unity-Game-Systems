

using UnityEngine;

public class CookableObject : MonoBehaviour
{
    public float cookTime = 10f; 
    public GameObject cookedPrefab; 
    
    private float timer = 0f;
    private bool isCooking = false;

    void Update()
    {
        if (!isCooking) return;
        
        timer += Time.deltaTime;

        if (timer >= cookTime)
        {
            FinishCooking();
        }
    }

    public void StartCooking() => isCooking = true;
    public void StopCooking() => isCooking = false;

    void FinishCooking()
    {
        if (cookedPrefab == null)
        {
         
            Debug.LogError("🚨 YAKALANDIM! cookedPrefab ataması yapılmamış. Hatalı Objenin Adı: " + gameObject.name, gameObject);
            
            isCooking = false; 
            return; 
        }

        GameObject spawned = Instantiate(cookedPrefab, transform.position, transform.rotation);
        
        if (transform.parent != null) 
        {
            spawned.transform.SetParent(transform.parent);
            
            GrillSlot slot = transform.parent.GetComponent<GrillSlot>();
            if (slot != null) {
                slot.currentObject = spawned;
            }
        }
        
        Destroy(gameObject); 
    }
}