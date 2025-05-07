using Player;
using UnityEngine;
using Weapon_System;

namespace Interactive_System
{
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;

        private void OnTriggerEnter(Collider other)
        {
            other.GetComponent<PlayerWeaponController>() ? .PickupWeapon(weapon);
        }
    }
}
