using UnityEngine;

public class OyuncuEtkilesim : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform kamera;
    public Transform elNoktasi;
    public float mesafe = 3f;

    [Header("Tuş Ayarları")]
    public KeyCode almaTusu = KeyCode.E;   
    public KeyCode birakmaTusu = KeyCode.F; 

   
    private BaharatSisesi eldekiSise;

    void Update()
    {
      
        if (Input.GetKeyDown(almaTusu))
        {
            if (eldekiSise == null)
            {
                YerdenAl();
            }
        }

      
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