using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "Shop/ShopItem", order = 1)]
public class ShopItemData : ScriptableObject
{
    public string itemName = "Item";
    public string description = "Description";
    public int price = 50;
    public Sprite icon;
    public ShopItemType itemType;
    public int amount = 1; // Quantidade de munição ou vida
}

public enum ShopItemType
{
    RifleAmmo,
    Health
}
