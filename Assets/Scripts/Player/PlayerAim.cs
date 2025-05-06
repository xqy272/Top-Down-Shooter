using UnityEngine;

namespace Player
{
    public class PlayerAim : MonoBehaviour
    {
        [Header("Aim Visual - Laser")] 
        [SerializeField] private LineRenderer aimLaser;

        // [SerializeField] private float laserLength;
        // [SerializeField] private float laserTipLength;
        [SerializeField] private float laserStartWidth;
        [SerializeField] private float laserEndWidth;

        [Header("Aim Info")]
        [SerializeField] private Transform aim;

        // [SerializeField] private bool isAimingPrecisely;
        [SerializeField] private bool isLockingToTarget;

        [Header("Camera Info")]
        [SerializeField] private Transform cameraTarget;

        [Range(0.5f, 1.5f)]
        [SerializeField] private float minCameraDistance = 1.5f;

        [Range(1.0f, 3.0f)]
        [SerializeField] private float maxCameraDistance = 4.0f;

        [Range(3.0f, 5.0f)]
        [SerializeField] private float cameraSensitivity = 5.0f;

        [Space]
        [SerializeField] private LayerMask aimLayerMask;

        private RaycastHit _lastKnownMouseHit;

        private Vector2 _mouseInput;
        private Player _player;
        private PlayerController _playerController;

        void Start()
        {
            _player = GetComponent<Player>();
            AssignInputEvents();
        }

        void Update()
        {
            UpdateAimVisuals();
            UpdateAimPosition();
            UpdateCameraTargetPosition();
        }

        private void UpdateCameraTargetPosition()
        {
            cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensitivity * Time.deltaTime);
        }

        private void UpdateAimPosition()
        {
            Transform target = Target();
            if (target != null && isLockingToTarget)
            {
                aim.position = target.GetComponent<Renderer>() != null ? target.GetComponent<Renderer>().bounds.center : target.position;
                return;
            }
        
            aim.position = GetMouseHitInfo().point;
        }

        public RaycastHit GetMouseHitInfo()
        {
            if (Camera.main != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(_mouseInput);

                if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
                {
                    _lastKnownMouseHit = hitInfo;
                    return hitInfo;
                }
            }

            return _lastKnownMouseHit;
        }

        private Vector3 DesiredCameraPosition()
        { 
            float actualMaxCameraDistance = _player.Movement.MoveInput.y < 0.5f ? minCameraDistance : maxCameraDistance;
        
            Vector3 desiredCameraPosition = GetMouseHitInfo().point;
            Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

            float distanceToDesiredPosition = Vector3.Distance(transform.position, desiredCameraPosition);
            float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, minCameraDistance, actualMaxCameraDistance);
        
            desiredCameraPosition = transform.position + aimDirection * clampedDistance;
            desiredCameraPosition.y = transform.position.y + 1;


            return desiredCameraPosition;
        }

        private void UpdateAimVisuals()
        {
            aimLaser.enabled = _player.WeaponController.WeaponReady();
            if (!aimLaser.enabled) return;

            WeaponModel weaponModel = _player.WeaponVisualController.GetCurrentWeaponModel();
            
            weaponModel.transform.LookAt(aim);
            weaponModel.bulletSpawnPoint.LookAt(aim);
            
            float startWidth = laserStartWidth;
            float endWidth;

            float gunDistance = _player.WeaponController.CurrentWeapon().GunDistance;
        
            Transform firstPoint = _player.WeaponController.BulletSpawnPoint();
            Vector3 laserDirection = _player.WeaponController.BulletDirection();
            Vector3 secondPoint = firstPoint.position + laserDirection * gunDistance;
        
            if (Physics.Raycast(firstPoint.position, laserDirection, out RaycastHit hit, gunDistance))
            {
                secondPoint = hit.point;
                // laserEndWidth = (gunDistance - (firstPoint.position - secondPoint).magnitude) % gunDistance * laserStartWidth;
                endWidth = laserStartWidth;
            }
            else
            {
                endWidth = laserEndWidth;
            }
            SetAimLaser(firstPoint, secondPoint, startWidth, endWidth);
        }

        private void SetAimLaser(Transform firstPoint, Vector3 secondPoint, float startWidth, float endWidth)
        {
            aimLaser.startWidth = startWidth;
            aimLaser.endWidth = endWidth;
            aimLaser.SetPosition(0, firstPoint.position);
            aimLaser.SetPosition(1, secondPoint);
        }

        private Transform Target()
        {
            Transform target = null;

            if (GetMouseHitInfo().transform.GetComponent<Target>() != null)
            {
                target = GetMouseHitInfo().transform;
            }
            return target;
        }

        private void AssignInputEvents()
        {
            _playerController = _player.PlayerController;

            _playerController.Character.Aim.performed += c => _mouseInput = c.ReadValue<Vector2>();
            _playerController.Character.Aim.canceled += _ => _mouseInput = Vector2.zero;
        }

        public Transform Aim() => aim;
    }
}
