using UnityEngine;

[System.Serializable]
public class ProductData
{
    public string productName;
    public float basePrice;
    [HideInInspector] public float dailyPrice;

    public ProductData(string name, float price)
    {
        productName = name;
        basePrice   = price;
        dailyPrice  = price;
    }
}
