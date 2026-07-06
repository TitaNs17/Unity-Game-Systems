using UnityEngine;

public class DrinkBox : MonoBehaviour
{
    [Header("Koli Ayarları")]
    public string boxID; // Örn: "Kola", "Ayran"
    public GameObject drinkPrefabInside; 
    public int currentCount = 12;
    public int maxCount = 12; // Geri alırken sınırı aşmamak için

    public bool HasDrink() => currentCount > 0;

    public void UseDrink() => currentCount--;

    // Raftan geri alınan içeceği koliye geri eklemek için:
    public void AddDrinkBack() 
    {
        if(currentCount < maxCount) currentCount++;
    }
}