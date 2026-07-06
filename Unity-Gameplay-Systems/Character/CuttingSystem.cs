using UnityEngine;

public class CuttingSystem : MonoBehaviour
{
    public float cutRange = 10f; 
    public LayerMask cutLayer;
    public KeyCode cutKey = KeyCode.Mouse0;

    private Camera cam;
    private AlmaBirakmaSistemi almaSistemi;

    void Start()
    {
        cam = Camera.main;
        almaSistemi = FindFirstObjectByType<AlmaBirakmaSistemi>();
        
        if (cam == null) Debug.LogError("DİKKAT: Sahnede 'MainCamera' Tag'li bir kamera bulunamadı!");
    }

    void Update()
    {
        if (Input.GetKeyDown(cutKey))
        {
            Debug.Log("1. ADIM: Sol tık algılandı.");

            if (almaSistemi != null && almaSistemi.tutulanObje != null)
            {
                Debug.Log("2. ADIM: Elinde bir obje var: " + almaSistemi.tutulanObje.name);
                
                if (almaSistemi.tutulanObje.CompareTag("Bicak"))
                {
                    Debug.Log("3. ADIM: Bıçak onaylandı! Işın gönderiliyor...");
                    TryCut();
                }
                else
                {
                    Debug.LogWarning("HATA: Elindeki objenin Tag'i 'Bicak' değil! Mevcut Tag: " + almaSistemi.tutulanObje.tag);
                }
            }
            else
            {
                Debug.Log("HATA: Elin boş olduğu için kesme başlamadı.");
            }
        }
    }

    void TryCut()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        
        Debug.DrawRay(ray.origin, ray.direction * cutRange, Color.red, 2f);

        if (Physics.Raycast(ray, out hit, cutRange, cutLayer))
        {
            Debug.Log("4. ADIM: Işın bir şeye çarptı: " + hit.collider.name);
            
            SliceableObject sliceable = hit.collider.GetComponent<SliceableObject>();
            if (sliceable != null)
            {
                Debug.Log("5. ADIM: SliceableObject bulundu! Kesiliyor...");
                sliceable.SliceOnce();
            }
            else
            {
                Debug.LogWarning("HATA: Çarptığın objede 'SliceableObject' kodu bulunamadı!");
            }
        }
        else
        {
            Debug.LogWarning("HATA: Işın hiçbir şeye çarpmadı. Menzil kısa olabilir veya Layer yanlış!");
        }
    }
}