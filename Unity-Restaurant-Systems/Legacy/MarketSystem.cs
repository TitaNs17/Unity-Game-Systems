using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MarketSystem : MonoBehaviour
{
    [Header("Veritabanı")]
    public ProductDatabase veritabani;
    [Header("UI Bağlantıları")]
    public GameObject tabletPanel;   // Açıp kapatacağımız panel
    public Transform contentArea;    // Butonların dizileceği yer (Grid)
    public GameObject buttonPrefab;  // Az önce yaptığın buton prefabı

    [Header("Teslimat Ayarları")]
    public Transform deliveryPoint;  // Koli nereye düşsün?

    private bool isTabletOpen = false;

    void Start()
    {
        // Başlangıçta tableti gizle
        tabletPanel.SetActive(false);

        // Dükkandaki ürünleri listele
        LoadShopItems();
    }

    void Update()
    {
        // TAB tuşu ile aç/kapa
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isTabletOpen = !isTabletOpen;
            tabletPanel.SetActive(isTabletOpen);

            // Mouse imlecini aç/kapa
            Cursor.visible = isTabletOpen;
            Cursor.lockState = isTabletOpen ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    void LoadShopItems()
    {
        {
            if (veritabani == null) return;

            foreach (ProductData product in veritabani.products)
            {
                // Butonu oluştur
                GameObject newBtn = Instantiate(buttonPrefab, contentArea);

                // --- DEĞİŞEN KISIM BURASI ---

                // Butonun üzerindeki bizim scripti bul
                MarketButton btnScript = newBtn.GetComponent<MarketButton>();

                if (btnScript != null)
                {
                    // Yazıyı yazdır
                    btnScript.UrunuYaz(product);
                }

                // Satın alma işlemini bağla
                Button btn = newBtn.GetComponent<Button>();
                btn.onClick.AddListener(() => BuyItem(product));

                // ----------------------------
            }
        }

        // SATIN ALMA FONKSİYONU
        void BuyItem(ProductData product)
        {
            // Para var mı?
            if (MoneyManager.Instance.SpendMoney(product.buyPrice))
            {
                Debug.Log(product.productName + " satın alındı!");

                // Ürünü (veya kolisini) dükkanın önüne ışınla
                if (product.prefab != null)
                {
                    Instantiate(product.prefab, deliveryPoint.position, Quaternion.identity);
                }
            }
            else
            {
                Debug.Log("Paran yetmiyor!");
            }
        }
    }
}