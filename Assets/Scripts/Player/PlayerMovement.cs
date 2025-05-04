using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private static readonly int Running = Animator.StringToHash("IsRunning");
        private static readonly int ZVelocity = Animator.StringToHash("zVelocity");
        private static readonly int XVelocity = Animator.StringToHash("xVelocity");
        private Player _player;

        private PlayerController _playerController;
        private CharacterController _characterController;
        private Animator _animator;

        [Header("MovementInfo")]
        private float _speed;
        private float _verticalVelocity;
        private bool _isRunning;
        [SerializeField] private float runSpeed;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float turnSpeed;
    
        private Vector3 _movementDirection;
        public Vector2 MoveInput { get;  private set; }





        private void Start()
        {
            _player = GetComponent<Player>();

            _characterController = GetComponent<CharacterController>();
            _animator = GetComponentInChildren<Animator>();

            _speed = walkSpeed;

            AssignInputEvents();
        }

        private void Update()
        {
            ApplyMovement();
            ApplyRotation();
            AnimatorController();
        }


        private void AssignInputEvents()
        {
            _playerController = _player.PlayerController;

            _playerController.Character.Movement.performed += context => MoveInput = context.ReadValue<Vector2>();
            _playerController.Character.Movement.canceled += _ => MoveInput = Vector2.zero;


            _playerController.Character.Run.performed += _ =>
            {
                _speed = runSpeed;
                _isRunning = true;
            };
            _playerController.Character.Run.canceled += _ =>
            {
                _speed = walkSpeed;
                _isRunning = false;
            };

        }

        private void AnimatorController()
        {
            float xVelocity = Vector3.Dot(_movementDirection.normalized, transform.right);
            float zVelocity = Vector3.Dot(_movementDirection.normalized, transform.forward);

            _animator.SetFloat(XVelocity, xVelocity, 0.1f, Time.deltaTime);
            _animator.SetFloat(ZVelocity, zVelocity, 0.1f, Time.deltaTime);

            bool playRunAnimation = _isRunning && _movementDirection.magnitude > 0;
            _animator.SetBool(Running, playRunAnimation);

        }

        private void ApplyRotation()
        {
            Vector3 lookingDirection = _player.PlayerAim.GetMouseHitInfo().point - transform.position;
            lookingDirection.y = 0;
            lookingDirection.Normalize();

            //transform.forward = LookingDirection;

            Quaternion desiredRotation = Quaternion.LookRotation(lookingDirection);

            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);
        }

        private void ApplyMovement()
        {
            _movementDirection = new Vector3(MoveInput.x, 0, MoveInput.y);
            ApplyGravity();

            if (_movementDirection.magnitude > 0)
            {
                _characterController.Move(_movementDirection * (Time.deltaTime * _speed));
            }
        }

        private void ApplyGravity()
        {
            if (!_characterController.isGrounded)
            {
                _verticalVelocity -= 9.81f * Time.deltaTime;
                _movementDirection.y = _verticalVelocity;
            }
            else
            {
                _verticalVelocity = -0.5f;
            }
        }
    }
}
