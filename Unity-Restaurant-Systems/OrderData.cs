using System.Collections.Generic;

public enum ProductType
{
    CeyrekKokorec,
    YarimKokorec,
    TamKokorec,
    Sirdan,
    Midye,
    Ayran,
    Salgam,
    Kola,
    Sprite
}

[System.Serializable]
public class OrderData
{
    public List<ProductType> items = new();
    public float maxWaitTime = 20f;     
    public float remainingTime = 20f;
    public int price = 0;
    public bool isCompleted = false;
}