
public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    ShineRifle
}

[System.Serializable]

public class Weapon
{
    public WeaponType weaponType;
    
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;
    
    [UnityEngine.Range(1,2)]
    public float reloadSpeed = 1;
    [UnityEngine.Range(1,2)]
    public float equipSpeed = 1;

    public bool CanShoot()
    {
        return HaveEnoughAmmo();
    }
    
    private bool HaveEnoughAmmo()
    {
        if (bulletsInMagazine > 0)
        {
            bulletsInMagazine--;
            return true;
        }
        return false;
    }

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
}
