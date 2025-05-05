using UnityEngine;

namespace Player
{
    public class PlayerAnimationEvents : MonoBehaviour
    {
        private WeaponVisualController _visualController;
        private PlayerWeaponController _weaponController;

        void Start()
        {
            _visualController = GetComponentInParent<WeaponVisualController>();
            _weaponController = GetComponentInParent<PlayerWeaponController>();
        }

        public void ReloadIsOver()
        {
            _visualController.ReturnRigWeightToOne();
            _weaponController.CurrentWeapon().ReloadBullets();
            
            _weaponController.SetWeaponReady(true);
        }

        public void ReturnRig()
        {
            _visualController.ReturnRigWeightToOne();
            _visualController.ReturnWeightToLeftHandIK();
        }

        public void WeaponEquipIsOver()
        {
            _weaponController.SetWeaponReady(true);
        }

        public void SwitchOnWeaponModel() => _visualController.SwitchOnCurrentWeaponModel();
    }
}
