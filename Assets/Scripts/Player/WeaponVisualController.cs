using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Player
{
    public class WeaponVisualController : MonoBehaviour
    {
        private static readonly int Reload = Animator.StringToHash("Reload");
        private static readonly int WeaponEquipType = Animator.StringToHash("WeaponEquipType");
        private static readonly int EquipWeapon = Animator.StringToHash("EquipWeapon");
        private static readonly int BusyEquippingWeapon = Animator.StringToHash("BusyEquippingWeapon");
        private static readonly int EquipSpeed = Animator.StringToHash("EquipSpeed");
        private static readonly int ReloadSpeed = Animator.StringToHash("ReloadSpeed");
        private Animator _anim;
        private bool _busyEquippingWeapon;
        private Player _player;

        [SerializeField] private WeaponModel[] weaponModels;
        [SerializeField] private BackupWeaponModel[] backupWeaponModels;
        
    
        [Header("Rig")]
        [SerializeField] private float rigIncreaseStep;
        private bool _rigShouldBeIncreased;
        private Rig _rig;

    
        [Header("Left hand IK")]
        [SerializeField] private Transform leftHandIKTarget;
        [SerializeField] private TwoBoneIKConstraint leftHandIK;
        [SerializeField] private float leftHandIKIncreaseStep;
        private bool _leftHandWeightShouldBeIncreased;



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

        public void PlayReloadAnimation()
        {
            if(_busyEquippingWeapon)
                return;
        
            float reloadSpeed = _player.WeaponController.CurrentWeapon().reloadSpeed;
            
            _anim.SetFloat(ReloadSpeed, reloadSpeed);
            _anim.SetTrigger(Reload);
            ReduceRigWeight();
        }
        

        public void PlayWeaponEquipAnimation()
        {
            EquipType equipType = GetCurrentWeaponModel().equipType;
            
            float equipSpeed = _player.WeaponController.CurrentWeapon().equipSpeed;
            
            leftHandIK.weight = 0;
            ReduceRigWeight();
            _anim.SetFloat(WeaponEquipType, ((float)equipType));
            _anim.SetTrigger(EquipWeapon);
            _anim.SetFloat(EquipSpeed, equipSpeed);

            SetBusyEquippingWeaponTo(true);
        }
        
        public void SetBusyEquippingWeaponTo(bool busy)
        {
            _busyEquippingWeapon = busy;
            _anim.SetBool(BusyEquippingWeapon, _busyEquippingWeapon);
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

        private WeaponModel GetCurrentWeaponModel()
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

        private void SwitchOffWeaponModels()
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
                backupWeaponModel.gameObject.SetActive(false);
            }
        }

        public void SwitchOnBackupWeaponModel()
        {
            WeaponType weaponType = _player.WeaponController.BackupWeapon().weaponType;

            foreach (BackupWeaponModel backupWeaponModel in backupWeaponModels)
            {
                if(backupWeaponModel.weaponType == weaponType)
                    backupWeaponModel.gameObject.SetActive(true);
            }
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