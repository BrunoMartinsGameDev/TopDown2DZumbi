using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Data")]
    public WeaponData weaponData;
    public Transform firePoint;

    private int currentMagazine;
    private int currentExtraMagazines;
    private float nextFireTime = 0f;
    private bool isReloading = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentMagazine = weaponData.magazineSize;
        currentExtraMagazines = weaponData.infiniteAmmo ? int.MaxValue : weaponData.startExtraMagazines;
    }

    // Input System callbacks
    public void OnFire(bool isPressed)
    {
        if (isPressed)
            TryShoot();
    }

    public void OnReload(bool isPressed)
    {
        if (isPressed)
            TryReload();
    }

    void TryShoot()
    {
        if (isReloading) return;
        if (Time.time < nextFireTime) return;
        if (currentMagazine <= 0)
        {
            TryReload();
            return;
        }
        Shoot();
        nextFireTime = Time.time + weaponData.fireRate;
    }

    void Shoot()
    {
        // Instancia o prefab da bala
        if (weaponData.bulletPrefab && firePoint)
        {
            GameObject bullet = Instantiate(weaponData.bulletPrefab, firePoint.position, firePoint.rotation);
            if(bullet.TryGetComponent<BulletBehavior>(out var bulletBehavior))
            {
                bulletBehavior.SetDamage(weaponData.damage);
            }
        }
        currentMagazine--;
        // Adicione efeitos, som, etc.
    }

    void TryReload()
    {
        if (weaponData.infiniteAmmo)
        {
            if (currentMagazine < weaponData.magazineSize)
                StartCoroutine(Reload(weaponData.magazineSize));
        }
        else
        {
            if (currentMagazine < weaponData.magazineSize && currentExtraMagazines > 0)
                StartCoroutine(Reload(weaponData.magazineSize));
        }
    }

    System.Collections.IEnumerator Reload(int magazineSize)
    {
        isReloading = true;
        yield return new WaitForSeconds(weaponData.reloadTime);
        int bulletsToReload = magazineSize - currentMagazine;
        if (weaponData.infiniteAmmo)
        {
            currentMagazine = magazineSize;
        }
        else
        {
            int bulletsAvailable = Mathf.Min(bulletsToReload, currentExtraMagazines * weaponData.magazineSize);
            currentMagazine += bulletsAvailable;
            currentExtraMagazines -= bulletsAvailable / weaponData.magazineSize;
            if (currentMagazine > weaponData.magazineSize)
                currentMagazine = weaponData.magazineSize;
        }
        isReloading = false;
    }

    // Métodos para adicionar munição extra futuramente
    public void AddExtraMagazine(int amount)
    {
        currentExtraMagazines += amount;
    }

    // Getters para UI
    public int GetCurrentMagazine() => currentMagazine;
    public int GetCurrentExtraMagazines() => currentExtraMagazines;
}
