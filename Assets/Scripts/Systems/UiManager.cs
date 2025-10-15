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
}
