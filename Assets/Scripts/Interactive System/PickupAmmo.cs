using System.Collections.Generic;
using Object_Pool;
using Player;
using UnityEngine;
using Weapon_System;

namespace Interactive_System
{
    [System.Serializable]
    public struct AmmoData
    {
        public WeaponType weaponType;
        public int amount;
        public int minAmount;
        public int maxAmount;
        public bool randomAmount;
    }

    public enum AmmoBoxType { SmallBoxAmmo, BigBoxAmmo }

    public class PickupAmmo : Interactable
    {
        [SerializeField] private AmmoBoxType ammoBoxType;

        [SerializeField] private List<AmmoData> smallBoxAmmo;
        [SerializeField] private List<AmmoData> bigBoxAmmo;

        [SerializeField] private GameObject[] boxModel;

        private void Start() => SetupBoxModel();

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);

            if (!PlayerWeaponController)
                PlayerWeaponController = other.GetComponent<PlayerWeaponController>();
        }

        public override void Interaction()
        {
            List<AmmoData> currentAmmoList = smallBoxAmmo;
        
            if(ammoBoxType == AmmoBoxType.BigBoxAmmo)
                currentAmmoList = bigBoxAmmo;

            foreach (AmmoData ammo in currentAmmoList)
            {
                Weapon weapon = PlayerWeaponController.GetWeaponInSlots(ammo.weaponType);
                AddBulletsToWeapon(weapon, ammo.amount);
                if (ammo.randomAmount) AddBulletsToWeapon(weapon, RandomGetBulletAmount(ammo));
            }
        
            ObjectPool.Instance.ReturnObject(gameObject);
        }

        private static void AddBulletsToWeapon(Weapon weapon, int amount)
        {
            if(weapon == null) return;
            weapon.totalReserveAmmo += amount;
        }

        private void SetupBoxModel()
        {
            for (var i = 0; i < boxModel.Length; i++)
            {
                boxModel[i].SetActive(false);

                if (i != (int)ammoBoxType) continue;
                boxModel[i].SetActive(true);
                UpdateMeshAndMaterial(boxModel[i].GetComponent<MeshRenderer>());
            }
        }

        private static int RandomGetBulletAmount(AmmoData ammoData)
        {
            int min = Mathf.Min(ammoData.minAmount, ammoData.maxAmount);
            int max = Mathf.Max(ammoData.minAmount, ammoData.maxAmount);

            int randomAmount = Random.Range(min, max);
            return randomAmount;
        }
    }
}