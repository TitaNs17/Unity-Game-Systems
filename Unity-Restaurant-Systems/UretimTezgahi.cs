using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UretimTezgahi : MonoBehaviour
{
    [Header("1. Ortaya Çıkacak Sonuç")]
    public GameObject hazirKokorecPrefab; // Birleşme bitince doğacak tam ekmek
    public Transform urunDogmaNoktasi;
    public float birlesmeSuresi = 3.0f;
    public TMP_Text sureYazisi;

    [Header("2. İstenen Malzeme Prefabları (Buraya Sürükle)")]
    public Malzeme gerekliEkmek;
    public Malzeme gerekliKokorec;
    public Malzeme gerekliDomates;
    public Malzeme gerekliBiber;

    // Tezgahın üstündeki fiziksel objeleri tuttuğumuz yer
    private List<GameObject> tezgahtakiObjeler = new List<GameObject>();
    private bool uretimBasladi = false;

    private void Start()
    {
        if (sureYazisi != null) sureYazisi.text = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        Malzeme malzeme = other.GetComponent<Malzeme>();

        if (malzeme != null && !uretimBasladi)
        {
            if (!tezgahtakiObjeler.Contains(other.gameObject))
            {
                tezgahtakiObjeler.Add(other.gameObject);
                UretimiKontrolEt();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Malzeme malzeme = other.GetComponent<Malzeme>();

        if (malzeme != null && !uretimBasladi)
        {
            if (tezgahtakiObjeler.Contains(other.gameObject))
            {
                tezgahtakiObjeler.Remove(other.gameObject);
            }
        }
    }

    private void UretimiKontrolEt()
    {
        // Başlangıçta hiçbir şey yok varsayıyoruz
        bool ekmekVar = false;
        bool kokorecVar = false;
        bool domatesVar = false;
        bool biberVar = false;

        // Tezgaha konan objelere tek tek bakıyoruz
        foreach (GameObject obje in tezgahtakiObjeler)
        {
            if (obje == null) continue;
            
            Malzeme m = obje.GetComponent<Malzeme>();

            // Girdiğin prefablarla eşleşiyor mu kontrolü
            if (gerekliEkmek != null && m.tur == gerekliEkmek.tur) ekmekVar = true;
            if (gerekliKokorec != null && m.tur == gerekliKokorec.tur) kokorecVar = true;
            if (gerekliDomates != null && m.tur == gerekliDomates.tur) domatesVar = true;
            if (gerekliBiber != null && m.tur == gerekliBiber.tur) biberVar = true;
        }

        // Eğer senin belirlediğin 4 prefabın da türü tezgahta mevcutsa, birleşmeyi başlat
        if (ekmekVar && kokorecVar && domatesVar && biberVar)
        {
            StartCoroutine(UretimSureci());
        }
    }

    private IEnumerator UretimSureci()
    {
        uretimBasladi = true;
        float kalanSure = birlesmeSuresi;

        while (kalanSure > 0)
        {
            if (sureYazisi != null) sureYazisi.text = kalanSure.ToString("F1") + " Sn";
            yield return new WaitForSeconds(0.1f);
            kalanSure -= 0.1f;
        }

        if (sureYazisi != null) sureYazisi.text = "HAZIR!";

        // 1. Tezgahtaki eski malzemeleri yok et
        foreach (GameObject obje in tezgahtakiObjeler)
        {
            if (obje != null) Destroy(obje);
        }
        tezgahtakiObjeler.Clear();

        // 2. Tam kokoreç ekmeğini yarat
        Vector3 spawnPos = urunDogmaNoktasi != null ? urunDogmaNoktasi.position : transform.position;
        Instantiate(hazirKokorecPrefab, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(1.0f);
        if (sureYazisi != null) sureYazisi.text = "";

        uretimBasladi = false;
    }
}