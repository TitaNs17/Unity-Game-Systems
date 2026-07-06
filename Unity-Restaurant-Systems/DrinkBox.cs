using UnityEngine;

public class DrinkBox : MonoBehaviour
{
    [Header("Koli Ayarları")]
    public string boxID; 
    public GameObject drinkPrefabInside; 
    public int currentCount = 12;
    public int maxCount = 12; 

    public bool HasDrink() => currentCount > 0;

    public void UseDrink() => currentCount--;

  
    public void AddDrinkBack() 
    {
        if(currentCount < maxCount) currentCount++;
    }
}