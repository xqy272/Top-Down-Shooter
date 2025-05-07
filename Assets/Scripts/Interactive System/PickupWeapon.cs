using Object_Pool;
using UnityEngine;
using Weapon_System;

namespace Interactive_System
{
    public class PickupWeapon : Interactable
    {
        [SerializeField] private WeaponData weaponData;
        [SerializeField] private Weapon weapon;

        [SerializeField] private BackupWeaponModel[] models;

        private bool _oldWeapon;

        private void Start()
        {
            if (!_oldWeapon) weapon = new Weapon(weaponData);
        
            UpdateGameObject();
        }

        public void SetupPickupWeapon(Weapon setWeapon, Transform setTransform)
        {
            _oldWeapon = true;
        
            weapon = setWeapon;
            weaponData = setWeapon.WeaponData;
        
            transform.position = setTransform.position + new Vector3(0, 0.5f, 0);
        }

        [ContextMenu("Update Item Models")]
        private void UpdateGameObject()
        {
            gameObject.name = "Pickup_Weapon - " + weaponData.weaponType.ToString();
            UpdateItemModel();
        }

        private void UpdateItemModel()
        {
            foreach (var model in models)
            {
                model.gameObject.SetActive(false);

                if (model.weaponType == weaponData.weaponType)
                {
                    model.gameObject.SetActive(true);
                    UpdateMeshAndMaterial(model.GetComponent<MeshRenderer>());
                }
            }
        }

        public override void Interaction()
        {
            PlayerWeaponController.PickupWeapon(weapon);
            ObjectPool.Instance.ReturnObject(gameObject);
        }
    }
}
