using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public ShopItemData itemData;
    public TMPro.TMP_Text priceText;
    public TMPro.TMP_Text nameText;
    public Image iconImage;

    void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (itemData == null) return;

        if (priceText != null)
            priceText.text = $"${itemData.price}";

        if (iconImage != null && itemData.icon != null)
            iconImage.sprite = itemData.icon;

        if( nameText != null)
            nameText.text = LocalizationManager.Instance.GetLocalizedValue(itemData.itemType.ToString()+"Item");
    }

    public void OnBuyButtonClick()
    {
        if (itemData != null && ShopManager.Instance != null)
        {
            ShopManager.Instance.BuyItem(itemData);
        }
    }
}
