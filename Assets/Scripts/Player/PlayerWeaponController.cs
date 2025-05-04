using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerWeaponController : MonoBehaviour
    {
        private Player _player;
        private static readonly int Fire = Animator.StringToHash("Fire");


        [SerializeField] private Weapon currentWeapon;
    
    
        [ Header( "Bullet" )]
        private const float ReferenceBulletSpeed = 20f;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float bulletSpeed;
        [SerializeField] private Transform bulletSpawnPoint;
    
    
        [SerializeField] private Transform weaponHolder;

        [Header("Inventory")] [SerializeField] private List<Weapon> weaponSlots;

        private void Start()
        {
            _player = GetComponent<Player>();
            AssignInputEvents();
        
            currentWeapon.bulletsInMagazine = currentWeapon.totalReserveAmmo;
        }

        
private void AssignInputEvents()
{
    var controller = _player.PlayerController;
    AssignWeaponActionEvents(controller);
    AssignEquipmentEvents(controller);
}

private void AssignWeaponActionEvents(PlayerController controller)
{
    controller.Character.Fire.performed += _ => Shoot();
    controller.Character.Reload.performed += _ =>
    {
        if (currentWeapon.CanReload())
        {
            _player.WeaponVisualController.PlayReloadAnimation();
        }
    };
}

private void AssignEquipmentEvents(PlayerController controller)
{
    controller.Character.EquipSlot1.performed += _ => EquipWeapon(0);
    controller.Character.EquipSlot2.performed += _ => EquipWeapon(1);
    controller.Character.DropCurrentWeapon.performed += _ => DropWeapon();
}

        private void Shoot()
        {
            if(currentWeapon.CanShoot() == false)
                return;
        
        
            GameObject newBullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(bulletSpawnPoint.forward));
        
            Rigidbody rb = newBullet.GetComponent<Rigidbody>();
            rb.mass = ReferenceBulletSpeed / bulletSpeed;
            rb.linearVelocity = BulletDirection() * bulletSpeed;
        
            Destroy(newBullet, 3f);
        
            GetComponentInChildren<Animator>().SetTrigger(Fire);
        }

        private void EquipWeapon(int i)
        {
            currentWeapon = weaponSlots[i];
            _player.WeaponVisualController.PlayWeaponEquipAnimation();
        }

        public void PickupWeapon(Weapon newWeapon)
        {
            if (weaponSlots.Count >= 2)
                return;
            
            weaponSlots.Add(newWeapon);
            _player.WeaponVisualController.SwitchOnBackupWeaponModel();
        }

        private void DropWeapon()
        {
            if (HasOnlyOneWeapon())
                return;

            weaponSlots.Remove(currentWeapon);

            EquipWeapon(0);
        }

        public Vector3 BulletDirection()
        {
            // weaponHolder.LookAt(aim);
            // bulletSpawnPoint.LookAt(aim);todo:Lock at Aim.

            Transform aim = _player.PlayerAim.Aim();
        
            Vector3 direction = (aim.position - bulletSpawnPoint.position).normalized;
        
            return direction;
        }

        public Weapon BackupWeapon()
        {
            foreach (Weapon weapon in weaponSlots)
            {
                if (weapon != currentWeapon)
                    return weapon;
            }
            return null;
        }
    
        public Transform BulletSpawnPoint() => bulletSpawnPoint;
        public Weapon CurrentWeapon() => currentWeapon;

        public bool HasOnlyOneWeapon() => weaponSlots.Count < 2;
    }
}
