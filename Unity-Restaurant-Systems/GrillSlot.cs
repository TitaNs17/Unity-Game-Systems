using UnityEngine;

public class GrillSlot : MonoBehaviour, IInteractable
{
    public GameObject currentObject; 
    public GameObject pismisSistemPrefab; 
    public float pismeSuresi = 10f;
    public Vector3 pismisRotasyon = new Vector3(90, 0, 0); 

    private float timer = 0f;
    private bool isCooking = false;

    public string GetInteractionText()
    {
        if (currentObject == null) return "Kokoreci Koy";
        if (isCooking) return "Pişiyor... " + Mathf.RoundToInt(pismeSuresi - timer) + "s";
        return "Pişmiş Kokoreci Al";
    }

    public void Interact()
    {
        if (isCooking) return;

        AlmaBirakmaSistemi almaSistemi = Object.FindFirstObjectByType<AlmaBirakmaSistemi>();
        if (almaSistemi == null) return;

        if (currentObject == null && almaSistemi.tutulanObje != null)
        {
            if (almaSistemi.tutulanObje.GetComponent<CigItem>() != null)
            {
                currentObject = almaSistemi.tutulanObje;
                currentObject.transform.SetParent(this.transform);
                currentObject.transform.localPosition = Vector3.zero;
                currentObject.transform.localRotation = Quaternion.identity;
                
                // KOYARKEN FİZİĞİ KAPATIYORUZ
                Rigidbody rb = currentObject.GetComponent<Rigidbody>();
                if (rb != null) {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }

                almaSistemi.tutulanObje = null; 
                timer = 0f;
                isCooking = true;
            }
        }
        else if (currentObject != null && !isCooking && almaSistemi.tutulanObje == null)
        {
            almaSistemi.Al(currentObject);
            currentObject = null;
        }
    }

    void Update()
    {
        if (isCooking && currentObject != null)
        {
            timer += Time.deltaTime;
            if (timer >= pismeSuresi) PisirmeyiBitir();
        }
    }

    void PisirmeyiBitir()
    {
        isCooking = false;

        // 1. ESKİSİNİ SİL
        Destroy(currentObject);

        // 2. YENİSİNİ OLUŞTUR (Dünya koordinatlarını umursama)
        GameObject pismisSistem = Instantiate(pismisSistemPrefab);
        
        // 3. FİZİĞİ ANINDA ÖLDÜR (Fırlamayı durduran ana yer)
        Rigidbody rb = pismisSistem.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero; // Eğer bir hız kazandıysa sıfırla
            rb.angularVelocity = Vector3.zero;
        }

        // 4. ŞİMDİ YERİNE KOY
        pismisSistem.transform.SetParent(this.transform);
        pismisSistem.transform.localPosition = Vector3.zero; 
        pismisSistem.transform.localRotation = Quaternion.Euler(pismisRotasyon);

        currentObject = pismisSistem;
    }
}