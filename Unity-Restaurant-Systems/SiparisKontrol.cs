using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Randomlama için lazım

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
        // Enum'daki tüm baharatları otomatik al. Artık Inspector'a elle girmenize gerek yok.
        tumBaharatlar = System.Enum.GetValues(typeof(BaharatTuru)).Cast<BaharatTuru>().ToList();
        
        // Oyun başlar başlamaz ilk siparişi oluştur
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

            // Sipariş tamamlanmışsa
            if (siparisTamamMi)
            {
                Debug.Log("🎉 TEBRİKLER! Sipariş tamamlandı.");
                
                // Yeni sipariş oluştur (Rastgelelik burada başlıyor)
                YeniSiparisOlustur();
            }
            else
            {
                Debug.Log("😔 Siparişte eksikler var.");
            }
        }
    }

    /// <summary>
    /// Rastgele bir baharat listesi oluşturur.
    /// </summary>
    void YeniSiparisOlustur()
    {
        // Önceki siparişi temizle
        istenenler.Clear();

        // Kaç farklı baharat istensin? (Örn: En az 1, en fazla 4 çeşit)
        int rastgeleCesitSayisi = Random.Range(1, tumBaharatlar.Count + 1); 

        // Tüm baharatları karıştır
        List<BaharatTuru> karistirilmisListe = tumBaharatlar.OrderBy(a => Random.Range(0, 100)).ToList();

        // Karıştırılmış listeden rastgele sayıda eleman seç
        for (int i = 0; i < rastgeleCesitSayisi; i++)
        {
            istenenler.Add(karistirilmisListe[i]);
        }
        
        // Müşterinin ne istediğini konsola yazdır
        string siparis = "YENİ SİPARİŞ: ";
        foreach(var baharat in istenenler)
        {
            siparis += baharat.ToString() + ", ";
        }
        Debug.Log(siparis.TrimEnd(' ', ','));
    }
}