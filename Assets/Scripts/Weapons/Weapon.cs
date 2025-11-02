using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    [Header("Weapon Data")]
    public WeaponData[] weaponData;
    public Transform[] firePoints;

    
    public GameObject muzzleFlashPrefab;
    public AudioClip switchWeaponSound;

    private int currentMagazine;
    private int currentExtraMagazines;
    private float nextFireTime = 0f;
    private bool isReloading = false;
    private PlayerMovement playerMovement;

    private WeaponData currentWeaponData;

    private int[] magazines;
    private int[] extraMagazines;
    private int currentWeaponIndex = 0;

    [Header("UI Elements")]
    public GameObject loadingIcon;

    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        // Inicializa arrays de munição para cada arma
        magazines = new int[weaponData.Length];
        extraMagazines = new int[weaponData.Length];
        for (int i = 0; i < weaponData.Length; i++)
        {
            magazines[i] = weaponData[i].magazineSize;
            extraMagazines[i] = weaponData[i].infiniteAmmo ? int.MaxValue : weaponData[i].startExtraMagazines;
        }
        currentWeaponIndex = 0;
        currentWeaponData = weaponData[currentWeaponIndex];
        currentMagazine = magazines[currentWeaponIndex];
        currentExtraMagazines = extraMagazines[currentWeaponIndex];
        UiManager.instance.UpdateWeaponUI(currentWeaponData);
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
    public void OnChangeWeapon(bool isPressed)
    {
        if (!isPressed) return;
        // Salva o estado da arma atual
        magazines[currentWeaponIndex] = currentMagazine;
        extraMagazines[currentWeaponIndex] = currentExtraMagazines;
        // Troca para a próxima arma
        currentWeaponIndex = (currentWeaponIndex + 1) % weaponData.Length;
        currentWeaponData = weaponData[currentWeaponIndex];
        currentMagazine = magazines[currentWeaponIndex];
        currentExtraMagazines = extraMagazines[currentWeaponIndex];
        UiManager.instance.UpdateWeaponUI(currentWeaponData);
        playerMovement.UpdateWeaponSprite(currentWeaponData);
        SoundsManager.Instance.PlaySFX(switchWeaponSound,transform.position);
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
        nextFireTime = Time.time + currentWeaponData.fireRate;
    }

    void Shoot()
    {
        // Instancia o prefab da bala
        if (currentWeaponData.bulletPrefab && firePoints.Length > 0)
        {
            GameObject bullet = Instantiate(currentWeaponData.bulletPrefab, firePoints[currentWeaponIndex].position, firePoints[currentWeaponIndex].rotation);
            if(bullet.TryGetComponent<BulletBehavior>(out var bulletBehavior))
            {
                bulletBehavior.SetDamage(currentWeaponData.damage);
            }
        }
        currentMagazine--;

        Quaternion flashRotation = firePoints[currentWeaponIndex].rotation * Quaternion.Euler(0, 0, -45);
        
        GameObject flash = Instantiate(muzzleFlashPrefab, firePoints[currentWeaponIndex].position, flashRotation, firePoints[currentWeaponIndex]);

        Destroy(flash, 0.1f); // Destroi o efeito de flash
        SoundsManager.Instance.PlaySFX(currentWeaponData.shootSound, transform.position);
        CameraFollow.Instance.Shake(currentWeaponData.recoilIntensity, 0.1f);
        UiManager.instance.UpdateWeaponUI(currentWeaponData);
    }
    void TryReload()
    {
        if (currentWeaponData.infiniteAmmo)
        {
            if (currentMagazine < currentWeaponData.magazineSize)
                StartCoroutine(Reload(currentWeaponData.magazineSize));
        }
        else
        {
            if (currentMagazine < currentWeaponData.magazineSize && currentExtraMagazines > 0)
                StartCoroutine(Reload(currentWeaponData.magazineSize));
        }
    }

    System.Collections.IEnumerator Reload(int magazineSize)
    {
        isReloading = true;
        loadingIcon.SetActive(true);
        SoundsManager.Instance.PlaySFX(currentWeaponData.reloadSound, transform.position);
        yield return new WaitForSeconds(currentWeaponData.reloadTime);
        int bulletsToReload = magazineSize - currentMagazine;
        if (currentWeaponData.infiniteAmmo)
        {
            currentMagazine = magazineSize;
        }
        else
        {
            int bulletsAvailable = Mathf.Min(bulletsToReload, currentExtraMagazines);
            currentMagazine += bulletsAvailable;
            currentExtraMagazines -= bulletsAvailable;
            if (currentMagazine > currentWeaponData.magazineSize)
                currentMagazine = currentWeaponData.magazineSize;
        }
        // Sincroniza o array após recarregar
        magazines[currentWeaponIndex] = currentMagazine;
        extraMagazines[currentWeaponIndex] = currentExtraMagazines;
        isReloading = false;
        loadingIcon.SetActive(false);
        UiManager.instance.UpdateWeaponUI(currentWeaponData);
    }


    // Adiciona munição extra para a arma atualmente equipada
    public void AddExtraMagazine(int amount)
    {
        extraMagazines[currentWeaponIndex] += amount;
        currentExtraMagazines = extraMagazines[currentWeaponIndex];
        UiManager.instance.UpdateWeaponUI(currentWeaponData);
    }

    // Adiciona munição extra para uma arma específica pelo índice
    public void AddExtraMagazineToWeapon(int weaponIndex, int amount)
    {
        if (weaponIndex < 0 || weaponIndex >= extraMagazines.Length)
            return;
        extraMagazines[weaponIndex] += amount;
        // Se a arma adicionada for a equipada, atualiza o valor atual
        if (weaponIndex == currentWeaponIndex)
            currentExtraMagazines = extraMagazines[weaponIndex];
        // UiManager.instance.UpdateWeaponUI(weaponData[weaponIndex]);
    }

    // Getters para UI
    public int GetCurrentMagazine() => currentMagazine;
    public int GetCurrentExtraMagazines() => currentExtraMagazines;
}
