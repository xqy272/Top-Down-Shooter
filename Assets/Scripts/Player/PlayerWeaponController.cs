using System.Collections;
using System.Collections.Generic;
using Interactive_System;
using Object_Pool;
using UnityEngine;
using Weapon_System;

namespace Player
{
    public class PlayerWeaponController : MonoBehaviour
    {
        [ Header( "Bullet" )]
        private const float ReferenceBulletSpeed = 20f;
        // private static readonly int Fire = Animator.StringToHash("Fire");

        [SerializeField] private WeaponData defaultWeaponData;
        [SerializeField] private Weapon currentWeapon;

        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float bulletSpeed;


        [SerializeField] private Transform weaponHolder;

        [Header("Inventory")]
        [SerializeField] private int maxSlots = 2;

        [SerializeField] private List<Weapon> weaponSlots;

        [SerializeField] private GameObject weaponPickupPrefabs;
        private bool _isShooting;
        private Player _player;
        private bool _weaponReady;


        private void Start()
        {
            _player = GetComponent<Player>();
            AssignInputEvents();
        
            Invoke(nameof(EquipStartingWeapon), 0.1f);
        }

        private void Update()
        {
            if(_isShooting)
                Shoot();
        }


        private void Shoot()
        {
            if(!WeaponReady()) return;
            
            if(!currentWeapon.CanShoot()) return;
            
            _player.WeaponVisualController.PlayFireAnimation();
            
            if(currentWeapon.shootType == ShootType.Single) _isShooting = false;

            if (currentWeapon.BurstActivated())
            {
                StartCoroutine(BurstFire());
                return;
            }
            
            FireSingleBullet();
        }

        private IEnumerator BurstFire()
        {
            SetWeaponReady(false);
            
            for (int i = 1; i <= currentWeapon.BulletsPerShot; i++)
            {
                FireSingleBullet();
                
                yield return new WaitForSeconds(currentWeapon.BurstFireDelay);
                
                if(i >= currentWeapon.BulletsPerShot)
                    SetWeaponReady(true);
            }
            
            SetWeaponReady(true);
        }


        #region AssignInputEvents

        private void AssignInputEvents()
        {
            var controller = _player.PlayerController;
            AssignWeaponActionEvents(controller);
            AssignEquipmentEvents(controller);
        }

        private void AssignWeaponActionEvents(PlayerController controller)
        {
            controller.Character.Fire.performed += _ => _isShooting = true;
            controller.Character.Fire.canceled += _ => _isShooting = false;
            controller.Character.Reload.performed += _ =>
            {
                if (currentWeapon.CanReload() && WeaponReady())
                {
                    Reload();
                }
            };
        }

        private void AssignEquipmentEvents(PlayerController controller)
        {
            controller.Character.EquipSlot1.performed += _ => EquipWeapon(0);
            controller.Character.EquipSlot2.performed += _ => EquipWeapon(1);
            controller.Character.EquipSlot3.performed += _ => EquipWeapon(2);
            controller.Character.EquipSlot4.performed += _ => EquipWeapon(3);
            controller.Character.EquipSlot5.performed += _ => EquipWeapon(4);
            
            controller.Character.DropCurrentWeapon.performed += _ => DropWeapon();

            controller.Character.ToogleWeaponMode.performed += _ => currentWeapon.ToggleBurst();
        }

        #endregion


        #region Slots Management - Pickup\Drop\Equip\Ready Weapon

        private void EquipWeapon(int i)
        {
            if(i >= weaponSlots.Count) return;
            
            SetWeaponReady(false);
            
            currentWeapon = weaponSlots[i];
            _player.WeaponVisualController.PlayWeaponEquipAnimation();
            
            CameraManager.Instance.ChangeCameraDistance(currentWeapon.CameraDistance);
        }

        public void PickupWeapon(Weapon newWeapon)
        {
            if (GetWeaponInSlots(newWeapon.weaponType) != null)
            {
                GetWeaponInSlots(newWeapon.weaponType).totalReserveAmmo += newWeapon.bulletsInMagazine;
                return;
            }

            if (weaponSlots.Count >= maxSlots && newWeapon.weaponType != currentWeapon.weaponType)
            {
                int weaponIndex = weaponSlots.IndexOf(currentWeapon);

                _player.WeaponVisualController.SwitchOffWeaponModels();
                weaponSlots[weaponIndex] = newWeapon;
                
                CreateWeaponOnTheGround();
                EquipWeapon(weaponIndex);
                return;
            }
            
            weaponSlots.Add(newWeapon);
            _player.WeaponVisualController.SwitchOnBackupWeaponModel();
        }

        private void DropWeapon()
        {
            if (HasOnlyOneWeapon())
                return;

            CreateWeaponOnTheGround();

            weaponSlots.Remove(currentWeapon);

            EquipWeapon(0);
        }

        private void CreateWeaponOnTheGround()
        {
            GameObject droppedWeapon = ObjectPool.Instance.GetObject(weaponPickupPrefabs);
            droppedWeapon.GetComponent<PickupWeapon>()?.SetupPickupWeapon(currentWeapon, transform);
        }

        private void Reload()
        {
            SetWeaponReady(false);
            _player.WeaponVisualController.PlayReloadAnimation();
        }

        private void EquipStartingWeapon()
        {
            weaponSlots[0] = new Weapon(defaultWeaponData);
            
            EquipWeapon(0);
        }

        public Weapon CurrentWeapon() => currentWeapon;

        public bool HasOnlyOneWeapon() => weaponSlots.Count < 2;

        public Weapon GetWeaponInSlots(WeaponType weaponType)
        {
            foreach (Weapon weapon in weaponSlots)
            {
                if(weapon.weaponType == weaponType)
                    return weapon;
            }
            return null;
        }

        public void SetWeaponReady(bool ready) => _weaponReady = ready;
        public bool WeaponReady() => _weaponReady;

        #endregion


        #region Bullet

        private void FireSingleBullet()
        {
            currentWeapon.bulletsInMagazine--;
            
            GameObject newBullet = ObjectPool.Instance.GetObject(bulletPrefab);

            newBullet.transform.position = BulletSpawnPoint().position;
            newBullet.transform.rotation = Quaternion.LookRotation(BulletSpawnPoint().forward);
        
            Rigidbody rb = newBullet.GetComponent<Rigidbody>();
            
            Bullet bulletScript = newBullet.GetComponent<Bullet>();
            bulletScript.BulletSetup(currentWeapon.GunDistance);
            
            Vector3 bulletDirection = currentWeapon.ApplySpread(BulletDirection());
            
            rb.mass = ReferenceBulletSpeed / bulletSpeed;
            rb.linearVelocity = bulletDirection * bulletSpeed;
        }

        public Vector3 BulletDirection()
        {
            Transform aim = _player.PlayerAim.Aim();
        
            Vector3 direction = (aim.position - BulletSpawnPoint().position).normalized;
        
            return direction;
        }

        public Transform BulletSpawnPoint() => _player.WeaponVisualController.GetCurrentWeaponModel().bulletSpawnPoint;

        #endregion
    }
}
