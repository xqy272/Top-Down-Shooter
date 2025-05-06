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
    public ShootType shootType;
    public bool burstActive;

    [Header("Magazine")]
    public int bulletsInMagazine;

    public int magazineCapacity;
    public int totalReserveAmmo;


    [Header("Spread")]
    private float _baseSpreadAmount;

    [Header("Burst Fire")]
    private bool _burstAvailable;

    private int _burstModeBulletsPerShot;
    private float _burstModeFireRate;

    private float _currentSpreadAmount;
    private float _defaultFireRate;
    private float _fireRate;
    private float _lastShootTime;
    private float _lastSpreadUpdateTime;
    private float _maxSpreadAmount;
    private float _spreadCooldown;
    private float _spreadIncreaseRate;

    public Weapon(WeaponData weaponData)
    {
        _fireRate = weaponData.fireRate;
        weaponType = weaponData.weaponType;

        shootType = weaponData.shootType;
        BulletsPerShot = weaponData.bulletsPerShot;
        
        bulletsInMagazine = weaponData.bulletsInMagazine;
        magazineCapacity = weaponData.magazineCapacity;
        totalReserveAmmo = weaponData.totalReserveAmmo;
        
        _burstAvailable = weaponData.burstAvailable;
        burstActive = weaponData.burstActive;
        _burstModeBulletsPerShot = weaponData.burstModeBulletsPerShot;
        _burstModeFireRate = weaponData.burstModeFireRate;
        BurstFireDelay = weaponData.burstFireDelay;

        _baseSpreadAmount = weaponData.baseSpreadAmount;
        _maxSpreadAmount = weaponData.maxSpreadAmount;
        _spreadIncreaseRate = weaponData.spreadIncreaseRate;
        _spreadCooldown = weaponData.spreadCooldown;

        ReloadSpeed = weaponData.reloadSpeed;
        EquipSpeed = weaponData.equipSpeed;
        GunDistance = weaponData.gunDistance;
        CameraDistance = weaponData.cameraDistance;
        
        _defaultFireRate = _fireRate;
    }

    public int BulletsPerShot { get; private set; }
    public float BurstFireDelay { get; private set; }

    public float ReloadSpeed {get; private set;}
    public float EquipSpeed {get; private set;}
    public float GunDistance {get; private set;}
    public float CameraDistance {get; private set;}

    public bool CanShoot() => HaveEnoughAmmo() && ReadyToFire();

    private bool ReadyToFire()
    {
        if (!(Time.time > _lastShootTime + 1 / _fireRate)) return false;
        _lastShootTime = Time.time;
        return true;
    }

    #region Burst methods

    public bool BurstActivated()
    {
        if (weaponType == WeaponType.Shotgun)
        {
            BurstFireDelay = 0;
            return true;
        }
        return burstActive;
    }

    public void ToggleBurst()
    {
        if(!_burstAvailable)
            return;
        burstActive = !burstActive;

        if (burstActive)
        {
            BulletsPerShot = _burstModeBulletsPerShot;
            _fireRate = _burstModeFireRate;
        }
        else
        {
            BulletsPerShot = 1;
            _fireRate = _defaultFireRate;
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
        if (Time.time > _lastSpreadUpdateTime + _spreadCooldown)
            _currentSpreadAmount = _baseSpreadAmount;
        else
            IncreaseSpread();
        
        _lastShootTime = Time.time;
    }

    private void IncreaseSpread()
    {
        _currentSpreadAmount = Mathf.Clamp(_currentSpreadAmount + _spreadIncreaseRate, _baseSpreadAmount, _maxSpreadAmount);
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
