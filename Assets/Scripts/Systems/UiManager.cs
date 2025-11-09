using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{

    public static UiManager instance;
    public Weapon weapon;

    [Header("UI Elements")]
    public GameObject pistolUIImage;
    public GameObject rifleUIImage;
    public TMPro.TMP_Text ammoText;
    public Slider healthSlider;
    public GameObject gameOverPanel;
    public GameObject deathSpriteUI;
    public GameObject playerSpriteUI;
    public TMPro.TMP_Text moneyText;
    public GameObject waveCompletedPanel;
    public TMPro.TMP_Text waveCompletedText;
    public GameObject shopPanel;
    public GameObject purchaseMessagePanel;
    public TMPro.TMP_Text purchaseMessageText;
    public GameObject nextWaveTimerPanel;
    public TMPro.TMP_Text nextWaveTimerText;
    public GameObject victoryPanel;
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (waveCompletedPanel != null)
            waveCompletedPanel.SetActive(false);
        if (purchaseMessagePanel != null)
            purchaseMessagePanel.SetActive(false);
        if (nextWaveTimerPanel != null)
            nextWaveTimerPanel.SetActive(false);
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
        if (shopPanel != null)
            ShopManager.Instance.SetShopPanel(shopPanel);
    }

    public void UpdateWeaponUI(WeaponData newWeaponData)
    {
        ShowWeaponUI(newWeaponData);
        ammoText.text = $"{weapon.GetCurrentMagazine()} / {(weapon.GetCurrentExtraMagazines() == int.MaxValue ? "∞" : weapon.GetCurrentExtraMagazines().ToString())}";
    }

    void ShowWeaponUI(WeaponData weaponData)
    {
        pistolUIImage.SetActive(false);
        rifleUIImage.SetActive(false);
        switch (weaponData.weaponName)
        {
            case "Pistol":
                pistolUIImage.SetActive(true);
                break;
            case "Rifle":
                rifleUIImage.SetActive(true);
                break;
            default:
                pistolUIImage.SetActive(false);
                rifleUIImage.SetActive(false);
                break;
        }
    }

    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }
   
    public void ShowDeathSprite(){
        if(deathSpriteUI != null && playerSpriteUI != null){
            deathSpriteUI.SetActive(true);
            playerSpriteUI.SetActive(false);
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void UpdateMoneyUI(int money)
    {
        if (moneyText != null)
            moneyText.text = $"${money}";
    }

    public void ShowWaveCompleted(int wave, int reward)
    {
        if (waveCompletedPanel != null && waveCompletedText != null)
        {
            string template = LocalizationManager.Instance.GetLocalizedValue("waveCompleted");
            waveCompletedText.text = string.Format(template, wave, reward);
            waveCompletedPanel.SetActive(true);
            Invoke(nameof(HideWaveCompleted), 3f);
        }
    }

    void HideWaveCompleted()
    {
        if (waveCompletedPanel != null)
            waveCompletedPanel.SetActive(false);
    }

    public void ShowPurchaseSuccess(string itemName)
    {
        string template = LocalizationManager.Instance.GetLocalizedValue("purchaseSuccess");
        ShowPurchaseMessage(string.Format(template, itemName), Color.green);
    }

    public void ShowPurchaseFailed(string reason)
    {
        string template = LocalizationManager.Instance.GetLocalizedValue("purchaseFailed");
        ShowPurchaseMessage(string.Format(template, reason), Color.red);
    }

    void ShowPurchaseMessage(string message, Color color)
    {
        if (purchaseMessagePanel != null && purchaseMessageText != null)
        {
            purchaseMessageText.text = message;
            purchaseMessageText.color = color;
            purchaseMessagePanel.SetActive(true);
            Invoke(nameof(HidePurchaseMessage), 2f);
        }
    }

    void HidePurchaseMessage()
    {
        if (purchaseMessagePanel != null)
            purchaseMessagePanel.SetActive(false);
    }

    public void ShowNextWaveTimer(float time)
    {
        if (nextWaveTimerPanel != null)
        {
            nextWaveTimerPanel.SetActive(true);
            StartCoroutine(UpdateNextWaveTimer(time));
        }
    }

    System.Collections.IEnumerator UpdateNextWaveTimer(float time)
    {
        float remainingTime = time;
        while (remainingTime > 0)
        {
            if (nextWaveTimerText != null)
            {
                string template = LocalizationManager.Instance.GetLocalizedValue("nextWaveTimer");
                nextWaveTimerText.text = string.Format(template, Mathf.CeilToInt(remainingTime));
            }
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }
        if (nextWaveTimerPanel != null)
            nextWaveTimerPanel.SetActive(false);
    }

    public void ShowVictoryPanel()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(true);
    }
}
