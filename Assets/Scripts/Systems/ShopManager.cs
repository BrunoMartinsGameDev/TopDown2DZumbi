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
        Debug.Log($"Buying item: {item.itemName} for ${item.price}");
        if (GameManager.Instance.SpendMoney(item.price))
        {
            ApplyItemEffect(item);
            UiManager.instance?.ShowPurchaseSuccess(item.itemName);
            Debug.Log($"Item purchased: {item.itemName}");
        }
        else
        {
            UiManager.instance?.ShowPurchaseFailed("Dinheiro insuficiente!");
            Debug.Log($"Failed to purchase item: {item.itemName} - insufficient funds.");
        }
    }

    void ApplyItemEffect(ShopItemData item)
    {
        switch (item.itemType)
        {
            case ShopItemType.RifleAmmo:
                if (playerWeapon != null)
                {
                    AddExtraMagazineToWeapon("rifle", item.amount);
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

    void AddExtraMagazineToWeapon(string weaponName, int amount)
    {
        // Procura o índice do rifle no array de armas
        int rifleIndex = -1;
        for (int i = 0; i < playerWeapon.weaponData.Length; i++)
        {
            if (playerWeapon.weaponData[i].weaponName.ToLower().Contains(weaponName))
            {
                rifleIndex = i;
                break;
            }
        }
        if (rifleIndex != -1)
        {
            playerWeapon.AddExtraMagazineToWeapon(rifleIndex, amount);
        }
        else
        {
            Debug.LogWarning("Rifle não encontrado no array de armas!");
        }
    }

    public void OpenShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(true);
        FindFirstObjectByType<PlayerInputGeneral>()?.ChangeControlScheme(ControlMap.UI);
    }

    public void CloseShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);
        FindFirstObjectByType<PlayerInputGeneral>()?.ChangeControlScheme(ControlMap.Player);
    }

    public void SetShopPanel(GameObject panel)
    {
        shopPanel = panel;
        if (shopPanel != null)
            shopPanel.SetActive(false);
    }
}
