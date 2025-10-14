using UnityEngine;

public class UiManager : MonoBehaviour
{

    public static UiManager instance;
    public Weapon weapon;

    [Header("UI Elements")]
    public GameObject pistolUIImage;
    public GameObject rifleUIImage;
    public TMPro.TMP_Text ammoText;
    

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
}
