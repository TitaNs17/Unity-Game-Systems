using UnityEngine;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;

    public List<OrderData> activeOrders = new();
    public int basePrice = 30; // temel ücret

    void Awake()
    {
        Instance = this;
    }

    public OrderData CreateRandomOrder()
    {
        OrderData order = new OrderData();

        // 1. ANA YEMEK SEÇİMİ (ZORUNLU)
        List<ProductType> mainFoods = new List<ProductType>
        {
            ProductType.CeyrekKokorec, ProductType.YarimKokorec, ProductType.TamKokorec,
            ProductType.Sirdan, ProductType.Midye
        };

        ProductType selectedMain = mainFoods[Random.Range(0, mainFoods.Count)];

        // --- ADET HESAPLAMA SİSTEMİ ---
        int miktar = 1;

        if (selectedMain == ProductType.Midye)
        {
            miktar = Random.Range(3, 8); // 3 ile 7 arası rastgele Midye
        }
        else if (selectedMain == ProductType.Sirdan)
        {
            miktar = Random.Range(1, 4); // 1 ile 3 arası rastgele Şırdan
        }

        // Listeye ekle
        for (int i = 0; i < miktar; i++)
        {
            order.items.Add(selectedMain);
        }

        // 2. İÇECEK SEÇİMİ (İSTEĞE BAĞLI - %50 İhtimalle)
        if (Random.value > 0.5f)
        {
            List<ProductType> drinks = new List<ProductType>
            {
                ProductType.Ayran, ProductType.Salgam, ProductType.Kola, ProductType.Sprite
            };
            ProductType selectedDrink = drinks[Random.Range(0, drinks.Count)];
            order.items.Add(selectedDrink);
        }

        order.price = basePrice + (order.items.Count * 15);
        order.remainingTime = order.maxWaitTime;

        activeOrders.Add(order);

        return order;
    }

    public void CompleteOrder(OrderData order)
    {
        order.isCompleted = true;
        Debug.Log("Sipariş tamamlandı! +" + order.price + "₺");

        activeOrders.Remove(order);

        // Para ekleme sistemi (MoneyManager scriptin varsa çalışır)
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.AddMoney(order.price);
        }
    }
}