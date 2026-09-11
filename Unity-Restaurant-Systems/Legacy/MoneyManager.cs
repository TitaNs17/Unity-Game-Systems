using UnityEngine;
using TMPro; 

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance; 

    [Header("Ayarlar")]
    public int suankiPara = 0;
    public TextMeshProUGUI paraText; 

    private void Awake()
    {
        
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
        UpdateUI(); 
    }

    public void AddMoney(int miktar)
    {
        suankiPara += miktar;
        UpdateUI();
        Debug.Log("Kasa: " + miktar + "₺ eklendi. Toplam: " + suankiPara);
    }

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