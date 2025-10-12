using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    public string weaponName = "Pistol";
    public float damage = 8f;
    public float fireRate = 0.2f;
    public int magazineSize = 6;
    public float reloadTime = 0.5f;
    public bool infiniteAmmo = true;
    public int startExtraMagazines = 0;
    public GameObject bulletPrefab;
}
