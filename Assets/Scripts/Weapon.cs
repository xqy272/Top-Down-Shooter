using UnityEngine;

public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    ShineRifle
}

public enum ShootType
{
    Single,
    Auto,
}

[System.Serializable]

public class Weapon
{
    public WeaponType weaponType;

    [Header("Shoot")]
    public ShootType shootType;

    public int bulletsPerShot;
    public float defaultFireRate;
    public float fireRate = 1;

    [Header("Burst Fire")]
    public bool burstAvailable;

    public bool burstActive;
    public int burstModeBulletsPerShot;
    public float burstModeFireRate;
    public float burstFireDelay = 0.1f;

    [Header("Magazine")]
    public int bulletsInMagazine;

    public int magazineCapacity;
    public int totalReserveAmmo;

    [Range(0, 5)] public float reloadSpeed = 1;
    [Range(0, 5)] public float equipSpeed = 1;
    [Range(0, 20)] public float gunDistance = 4;
    [Range(3, 8)] public float cameraDistance = 6;


    [Header("Spread")]
    public float baseSpreadAmount;

    public float maxSpreadAmount;
    public float spreadIncreaseRate;
    public float spreadCooldown = 1;
    private float _currentSpreadAmount;
    private float _lastShootTime;
    private float _lastSpreadUpdateTime;


    public bool CanShoot() => HaveEnoughAmmo() && ReadyToFire();

    private bool ReadyToFire()
    {
        if (!(Time.time > _lastShootTime + 1 / fireRate)) return false;
        _lastShootTime = Time.time;
        return true;
    }

    #region Burst methods

    public bool BurstActivated()
    {
        if (weaponType == WeaponType.Shotgun)
        {
            burstFireDelay = 0;
            return true;
        }
        return burstActive;
    }

    public void ToggleBurst()
    {
        if(!burstAvailable)
            return;
        burstActive = !burstActive;

        if (burstActive)
        {
            bulletsPerShot = burstModeBulletsPerShot;
            fireRate = burstModeFireRate;
        }
        else
        {
            bulletsPerShot = 1;
            fireRate = defaultFireRate;
        }
    }

    #endregion


    #region Spread Methods

    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();
        
        float spread = Random.Range(-_currentSpreadAmount, _currentSpreadAmount);
        
        Quaternion spreadRotation = Quaternion.Euler(spread, spread, spread);
        
        return spreadRotation * originalDirection;
    }

    private void UpdateSpread()
    {
        if (Time.time > _lastSpreadUpdateTime + spreadCooldown)
            _currentSpreadAmount = baseSpreadAmount;
        else
            IncreaseSpread();
        
        _lastShootTime = Time.time;
    }

    private void IncreaseSpread()
    {
        _currentSpreadAmount = Mathf.Clamp(_currentSpreadAmount + spreadIncreaseRate, baseSpreadAmount, maxSpreadAmount);
    }

    #endregion


    #region Reload Methods

    private bool HaveEnoughAmmo() => bulletsInMagazine > 0;

    public bool CanReload()
    {
        if(bulletsInMagazine == magazineCapacity) return false;
        if(totalReserveAmmo > 0)
            return true;
        return false;
    }

    public void ReloadBullets()
    {
        totalReserveAmmo += bulletsInMagazine;
        
        int bulletsToReload = magazineCapacity;

        if (bulletsToReload > totalReserveAmmo)
        {
            bulletsToReload = totalReserveAmmo;
        }
        
        totalReserveAmmo -= bulletsToReload;
        bulletsInMagazine = bulletsToReload;

        if (totalReserveAmmo < 0)
        {
            totalReserveAmmo = 0;
        }
    }

    #endregion
}
