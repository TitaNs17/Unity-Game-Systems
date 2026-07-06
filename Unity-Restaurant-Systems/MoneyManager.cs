using UnityEngine;
using TMPro; // TextMeshPro kullandığımız için bunu ekliyoruz

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance; // Her yerden ulaşmak için anahtar

    [Header("Ayarlar")]
    public int suankiPara = 0;
    public TextMeshProUGUI paraText; // Ekrana yazılacak yazı

    private void Awake()
    {
        // Singleton yapısı: Oyunun her yerinden 'MoneyManager.Instance' diye ulaşabilirsin
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateUI(); // Başlar başlamaz ekrana yaz
    }

    // Para Ekleme Fonksiyonu
    public void AddMoney(int miktar)
    {
        suankiPara += miktar;
        UpdateUI();
        Debug.Log("Kasa: " + miktar + "₺ eklendi. Toplam: " + suankiPara);
    }

    // Para Harcama Fonksiyonu (İleride market için lazım)
    public bool SpendMoney(int miktar)
    {
        if (suankiPara >= miktar)
        {
            suankiPara -= miktar;
            UpdateUI();
            return true;
        }
        return false;
    }

    void UpdateUI()
    {
        if (paraText != null)
        {
            paraText.text = suankiPara.ToString() + " $";
        }
    }
}