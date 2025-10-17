using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }
    
    public ShopItemData[] shopItems;
    private GameObject shopPanel;
    private PlayerStats playerStats;
    private Weapon playerWeapon;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
            playerWeapon = player.GetComponentInChildren<Weapon>();
        }
    }

    public void BuyItem(ShopItemData item)
    {
        if (item == null || GameManager.Instance == null) return;

        if (GameManager.Instance.SpendMoney(item.price))
        {
            ApplyItemEffect(item);
            UiManager.instance?.ShowPurchaseSuccess(item.itemName);
        }
        else
        {
            UiManager.instance?.ShowPurchaseFailed("Dinheiro insuficiente!");
        }
    }

    void ApplyItemEffect(ShopItemData item)
    {
        switch (item.itemType)
        {
            case ShopItemType.RifleAmmo:
                if (playerWeapon != null)
                {
                    playerWeapon.AddExtraMagazine(item.amount);
                }
                break;
            
            case ShopItemType.Health:
                if (playerStats != null)
                {
                    playerStats.Heal(item.amount);
                }
                break;
        }
    }

    public void OpenShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(true);
        Time.timeScale = 0f; // Pausa o jogo
    }

    public void CloseShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);
        Time.timeScale = 1f; // Despausa o jogo
    }

    public void SetShopPanel(GameObject panel)
    {
        shopPanel = panel;
        if (shopPanel != null)
            shopPanel.SetActive(false);
    }
}
