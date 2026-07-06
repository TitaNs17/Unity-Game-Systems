// using UnityEngine;

// public enum CookState { Raw, Cooked, Burnt }

// public class CookableObject : MonoBehaviour
// {
//     public float cookTime = 12f;
//     public float burnTime = 18f;

//     private float timer = 0f;
//     private bool isCooking = false;

//     public CookState state = CookState.Raw;

//     private Renderer rend;

//     void Start()
//     {
//         rend = GetComponent<Renderer>();
//         UpdateColor();
//     }

//     void Update()
//     {
//         if (!isCooking) return;

//         timer += Time.deltaTime;

//         if (timer >= burnTime)
//             SetState(CookState.Burnt);
//         else if (timer >= cookTime)
//             SetState(CookState.Cooked);
//     }

//     public void StartCooking()
//     {
//         isCooking = true;
//     }

//     public void StopCooking()
//     {
//         isCooking = false;
//     }

//     void SetState(CookState newState)
//     {
//         if (state == newState) return;

//         state = newState;
//         UpdateColor();
//     }

//     void UpdateColor()
//     {
//         if (rend == null) return;

//         switch (state)
//         {
//             case CookState.Raw:
//                 rend.material.color = Color.gray;
//                 break;
//             case CookState.Cooked:
//                 rend.material.color = new Color(0.8f, 0.5f, 0.2f);
//                 break;
//             case CookState.Burnt:
//                 rend.material.color = Color.black;
//                 break;
//         }
//     }
// }

using UnityEngine;

public class CookableObject : MonoBehaviour
{
    public float cookTime = 10f; // Pişme süresi
    public GameObject cookedPrefab; // Piştiğinde dönüşeceği prefab (Kesme sistemli olan)
    
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
        // --- TUZAK BURADA BAŞLIYOR ---
        if (cookedPrefab == null)
        {
            // Unity'nin o çirkin kırmızı hatası yerine bizim yakalama mesajımız çıkacak.
            Debug.LogError("🚨 YAKALANDIM! cookedPrefab ataması yapılmamış. Hatalı Objenin Adı: " + gameObject.name, gameObject);
            
            // Oyunu çökertmemesi için pişmeyi durdurup işlemi iptal ediyoruz
            isCooking = false; 
            return; 
        }
        // --- TUZAK BURADA BİTİYOR ---

        // 1. Pişmiş kokoreci tam çiğ olanın yerinde oluştur
        GameObject spawned = Instantiate(cookedPrefab, transform.position, transform.rotation);
        
        // 2. Izgaranın içine hapset (Eğer ızgaradaysa)
        if (transform.parent != null) 
        {
            spawned.transform.SetParent(transform.parent);
            
            // 3. GrillSlot scriptine "Artık yeni objemiz bu" diye haber ver
            GrillSlot slot = transform.parent.GetComponent<GrillSlot>();
            if (slot != null) {
                slot.currentObject = spawned;
            }
        }
        
        // 4. Çiğ kokoreci yok et
        Destroy(gameObject); 
    }
}