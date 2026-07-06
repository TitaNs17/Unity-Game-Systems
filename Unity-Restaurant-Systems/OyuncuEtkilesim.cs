using UnityEngine;

public class OyuncuEtkilesim : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform kamera;
    public Transform elNoktasi;
    public float mesafe = 3f;

    [Header("Tuş Ayarları")]
    public KeyCode almaTusu = KeyCode.E;    // Varsayılan E
    public KeyCode birakmaTusu = KeyCode.F; // Varsayılan F

    // Şu an elimde tuttuğum BAHARAT şişesi
    private BaharatSisesi eldekiSise;

    void Update()
    {
        // --- EŞYAYI ALMA ---
        if (Input.GetKeyDown(almaTusu))
        {
            if (eldekiSise == null)
            {
                YerdenAl();
            }
        }

        // --- EŞYAYI BIRAKMA ---
        if (Input.GetKeyDown(birakmaTusu))
        {
            if (eldekiSise != null)
            {
                eldekiSise.YerineKoy();
                eldekiSise = null;
            }
        }
    }

    void YerdenAl()
    {
        RaycastHit hit;
        if (Physics.Raycast(kamera.position, kamera.forward, out hit, mesafe))
        {
            // Sadece etiketi "Baharat" olanlara bak
            if (hit.collider.CompareTag("Baharat"))
            {
                BaharatSisesi sise = hit.collider.GetComponent<BaharatSisesi>();
                
                if (sise != null)
                {
                    sise.ElineAl(elNoktasi);
                    eldekiSise = sise;
                }
            }
        }
    }
}