using UnityEngine;

namespace Weapon_System
{
    [CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon System/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        public string weaponName;

        [Header("Magazine")]
        public int bulletsInMagazine;

        public int magazineCapacity;
        public int totalReserveAmmo;

        [Header("Regular shot")]
        public ShootType shootType;

        public int bulletsPerShot = 1;
        public float fireRate;

        [Header("Burst Fire")]
        public bool burstAvailable;

        public bool burstActive;
        public int burstModeBulletsPerShot;
        public float burstModeFireRate;
        public float burstFireDelay = 0.1f;

        [Header("Spread")]
        public float baseSpreadAmount;

        public float maxSpreadAmount;
        public float spreadIncreaseRate = 0.15f;
        public float spreadCooldown = 0.5f;

        [Header("Generics")]
        public WeaponType weaponType;

        public float reloadSpeed = 1;
        public float equipSpeed = 1;
        public float gunDistance = 4;
        public float cameraDistance = 6;
    }
}
