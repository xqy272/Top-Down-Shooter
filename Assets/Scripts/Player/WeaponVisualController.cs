using UnityEngine;
using UnityEngine.Animations.Rigging;
using Weapon_System;

namespace Player
{
    public class WeaponVisualController : MonoBehaviour
    {
        private static readonly int Reload = Animator.StringToHash("Reload");
        private static readonly int WeaponEquipType = Animator.StringToHash("WeaponEquipType");
        private static readonly int EquipWeapon = Animator.StringToHash("EquipWeapon");
        private static readonly int EquipSpeed = Animator.StringToHash("EquipSpeed");
        private static readonly int ReloadSpeed = Animator.StringToHash("ReloadSpeed");
        private static readonly int Fire = Animator.StringToHash("Fire");

        [SerializeField] private WeaponModel[] weaponModels;
        [SerializeField] private BackupWeaponModel[] backupWeaponModels;


        [Header("Rig")]
        [SerializeField] private float rigIncreaseStep;


        [Header("Left hand IK")]
        [SerializeField] private Transform leftHandIKTarget;

        [SerializeField] private TwoBoneIKConstraint leftHandIK;
        [SerializeField] private float leftHandIKIncreaseStep;
        private Animator _anim;
        private bool _leftHandWeightShouldBeIncreased;
        private Player _player;
        private Rig _rig;
        private bool _rigShouldBeIncreased;


        private void Start()
        {

            _player = GetComponent<Player>();
            _anim = GetComponentInChildren<Animator>();
            _rig = GetComponentInChildren<Rig>();
            weaponModels = GetComponentsInChildren<WeaponModel>(true);
            backupWeaponModels = GetComponentsInChildren<BackupWeaponModel>(true);
        }

        private void Update()
        {
            UpdateRigWeight();
            UpdateLeftHandIKWeight();
        }

        public void PlayFireAnimation() => _anim.SetTrigger(Fire);

        public void PlayReloadAnimation()
        {
            float reloadSpeed = _player.WeaponController.CurrentWeapon().ReloadSpeed;
            
            _anim.SetFloat(ReloadSpeed, reloadSpeed);
            _anim.SetTrigger(Reload);
            ReduceRigWeight();
        }


        public void PlayWeaponEquipAnimation()
        {
            EquipType equipType = GetCurrentWeaponModel().equipType;
            
            float equipSpeed = _player.WeaponController.CurrentWeapon().EquipSpeed;
            
            leftHandIK.weight = 0;
            ReduceRigWeight();
            _anim.SetFloat(WeaponEquipType, ((float)equipType));
            _anim.SetTrigger(EquipWeapon);
            _anim.SetFloat(EquipSpeed, equipSpeed);
        }


        public void SwitchOnCurrentWeaponModel()
        {
            SwitchAnimationLayer((int)(GetCurrentWeaponModel().holdType));
            
            SwitchOffWeaponModels();
            SwitchOffBackupWeaponModels();
            if(!_player.WeaponController.HasOnlyOneWeapon())
                SwitchOnBackupWeaponModel();

            
            GetCurrentWeaponModel().gameObject.SetActive(true);
            AttachLeftHand();
        }

        public WeaponModel GetCurrentWeaponModel()
        {
            WeaponType currentWeaponType = _player.WeaponController.CurrentWeapon().weaponType;
            
            foreach (WeaponModel model in weaponModels)
            {
                if (model.weaponType == currentWeaponType)
                {
                    return model;
                }
            }
            return null;
        }

        public void SwitchOffWeaponModels()
        {
            foreach (WeaponModel weaponModel in weaponModels)
            {
                weaponModel.gameObject.SetActive(false);
            }
        }

        private void SwitchOffBackupWeaponModels()
        {
            foreach (BackupWeaponModel backupWeaponModel in backupWeaponModels)
            {
                backupWeaponModel.Activate(false);
            }
        }

        public void SwitchOnBackupWeaponModel()
        {
            SwitchOffBackupWeaponModels();

            BackupWeaponModel lowHangWeapon = null;
            BackupWeaponModel backHangWeapon = null;
            BackupWeaponModel sideHangWeapon = null;

            foreach (BackupWeaponModel backupWeaponModel in backupWeaponModels)
            {
                if(backupWeaponModel.weaponType == _player.WeaponController.CurrentWeapon().weaponType) continue;
                
                if (_player.WeaponController.GetWeaponInSlots(backupWeaponModel.weaponType) != null)
                {
                    if (backupWeaponModel.IsHangTypeMatch(HangType.LowBackHang))
                        lowHangWeapon = backupWeaponModel;
                    
                    if (backupWeaponModel.IsHangTypeMatch(HangType.BackHang))
                        backHangWeapon = backupWeaponModel;
                    
                    if (backupWeaponModel.IsHangTypeMatch(HangType.SideHang))
                        sideHangWeapon = backupWeaponModel;
                }
            }
            
            lowHangWeapon?.Activate(true);
            backHangWeapon?.Activate(true);
            sideHangWeapon?.Activate(true);
        }


        private void SwitchAnimationLayer(int layerIndex)
        {
            for (int i = 1; i < _anim.layerCount; i++)
            {
                _anim.SetLayerWeight(i, 0);
            }

            _anim.SetLayerWeight(layerIndex, 1);
        }


        #region Animation Rigging Methods

        private void UpdateLeftHandIKWeight()
        {
            if (_leftHandWeightShouldBeIncreased)
            {
                leftHandIK.weight += leftHandIKIncreaseStep * Time.deltaTime;

                if (leftHandIK.weight >= 1)
                {
                    _leftHandWeightShouldBeIncreased = false;
                }
            }
        }

        private void UpdateRigWeight()
        {
            if (_rigShouldBeIncreased)
            {
                _rig.weight += rigIncreaseStep * Time.deltaTime;

                if (_rig.weight >= 1)
                {
                    _rigShouldBeIncreased = false;
                }

            }
        }

        private void AttachLeftHand()
        {
            Transform targetTransform = GetCurrentWeaponModel().holdPoint;

            leftHandIKTarget.localPosition = targetTransform.localPosition;
            leftHandIKTarget.localRotation = targetTransform.localRotation;
        }

        private void ReduceRigWeight()
        {
            _rig.weight = 0.15f;
        }

        public void ReturnRigWeightToOne() => _rigShouldBeIncreased = true;
        public void ReturnWeightToLeftHandIK() => _leftHandWeightShouldBeIncreased = true;

        #endregion
    }
}