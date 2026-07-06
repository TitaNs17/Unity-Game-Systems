using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SiparisKontrol : MonoBehaviour
{
    [Header("Gereken Atamalar")]
    public Kokorec ekmek;

    [Header("Sipariş Bilgileri")]
    [Tooltip("Müşterinin istediği baharatların listesi")]
    public List<BaharatTuru> istenenler = new List<BaharatTuru>();
    
    [Tooltip("Sistemdeki tüm baharatların listesi")]
    public List<BaharatTuru> tumBaharatlar;

    void Start()
    {
       
        tumBaharatlar = System.Enum.GetValues(typeof(BaharatTuru)).Cast<BaharatTuru>().ToList();
        
        
        YeniSiparisOlustur();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("--- SİPARİŞ KONTROLÜ BAŞLADI ---");
            bool siparisTamamMi = true;

            if (ekmek == null)
            {
                Debug.LogError("HATA: Ekmek objesi Inspector'da tanıtılmamış!");
                return;
            }

            if (istenenler.Count == 0)
            {
                Debug.LogWarning("Mevcut sipariş listesi boş.");
                return;
            }

            foreach (var istek in istenenler)
            {
                if (ekmek.BuBaharatVarMi(istek))
                {
                    Debug.Log("✅ " + istek + " TAMAM");
                }
                else
                {
                    Debug.Log("❌ " + istek + " EKSİK");
                    siparisTamamMi = false;
                }
            }

           
            if (siparisTamamMi)
            {
                Debug.Log("🎉 TEBRİKLER! Sipariş tamamlandı.");
                
                
                YeniSiparisOlustur();
            }
            else
            {
                Debug.Log("😔 Siparişte eksikler var.");
            }
        }
    }

   
    void YeniSiparisOlustur()
    {
        
        istenenler.Clear();

        
        int rastgeleCesitSayisi = Random.Range(1, tumBaharatlar.Count + 1); 

      
        List<BaharatTuru> karistirilmisListe = tumBaharatlar.OrderBy(a => Random.Range(0, 100)).ToList();

      
        for (int i = 0; i < rastgeleCesitSayisi; i++)
        {
            istenenler.Add(karistirilmisListe[i]);
        }
        
        string siparis = "YENİ SİPARİŞ: ";
        foreach(var baharat in istenenler)
        {
            siparis += baharat.ToString() + ", ";
        }
        Debug.Log(siparis.TrimEnd(' ', ','));
    }
}