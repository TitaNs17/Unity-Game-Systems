using UnityEngine;
using UnityEngine.UI;

public class BaharatSisesi : MonoBehaviour
{
    [Header("Ayarlar")]
    public BaharatTuru siseTuru;
    public float dolumSuresi = 2.0f;

    [Header("Menzil Ayarı (YENİ)")]
    [Tooltip("Kameradan ne kadar uzağa sıkabilsin?")]
    public float dokmeMesafesi = 10.0f; 

    [Header("Tuş Ayarı")]
    public KeyCode dokmeTusu = KeyCode.Mouse0;

    [Header("Bileşenler")]
    public ParticleSystem partikulSistemi;
    public Slider dolumBari;
    public GameObject barObjesi;

    private Vector3 baslangicPos;
    private Quaternion baslangicRot;
    private bool elimizdeMi = false; 
    private float gecenSure = 0f;

    void Start()
    {
        baslangicPos = transform.position;
        baslangicRot = transform.rotation;
        if (barObjesi != null) barObjesi.SetActive(false);
        RengiAyarla();
    }

    void Update()
    {
        if (!elimizdeMi) return;

        if (Input.GetKey(dokmeTusu))
        {
            DokmeIslemi();
        }
        else
        {
            DurmaIslemi();
        }
    }

    public void ElineAl(Transform elNoktasi)
    {
        elimizdeMi = true;
        if(GetComponent<Rigidbody>()) GetComponent<Rigidbody>().isKinematic = true;
        transform.SetParent(elNoktasi);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void YerineKoy()
    {
        elimizdeMi = false;
        DurmaIslemi();
        transform.SetParent(null);
        transform.position = baslangicPos;
        transform.rotation = baslangicRot;
        if(GetComponent<Rigidbody>()) GetComponent<Rigidbody>().isKinematic = false;
    }

    void DokmeIslemi()
    {
        var emission = partikulSistemi.emission;
        emission.enabled = true;

        
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        
        Debug.DrawRay(ray.origin, ray.direction * dokmeMesafesi, Color.blue);

        RaycastHit hit;

      
        int layerMask = 1 << LayerMask.NameToLayer("Ekmek");

        
        if (Physics.Raycast(ray, out hit, dokmeMesafesi, layerMask))
        {
            Kokorec kokorec = hit.collider.GetComponent<Kokorec>();
            
            if (kokorec != null)
            {
                if (barObjesi != null) barObjesi.SetActive(true);
                
                gecenSure += Time.deltaTime;
                if (dolumBari != null) dolumBari.value = gecenSure / dolumSuresi;

                if (gecenSure >= dolumSuresi) kokorec.BaharatTamamlandi(siseTuru);
            }
        }
        else
        {
            if (barObjesi != null) barObjesi.SetActive(false);
        }
    }

    void DurmaIslemi()
    {
        var emission = partikulSistemi.emission;
        emission.enabled = false;
        gecenSure = 0;
        if (dolumBari != null) dolumBari.value = 0;
        if (barObjesi != null) barObjesi.SetActive(false);
    }

    void RengiAyarla()
    {
        if (partikulSistemi == null) return;
        var main = partikulSistemi.main;
        switch (siseTuru)
        {
            case BaharatTuru.Tuz: main.startColor = Color.white; break; // Beyaz
            case BaharatTuru.PulBiber: main.startColor = Color.red; break; // Kırmızı
            case BaharatTuru.Kimyon: main.startColor = new Color(0.0f, 0.4f, 0.0f); break; // Koyu Yeşil
            case BaharatTuru.Karabiber: main.startColor = new Color(0.3f, 0.2f, 0.1f); break; // Koyu Kahve
        }
    }
}