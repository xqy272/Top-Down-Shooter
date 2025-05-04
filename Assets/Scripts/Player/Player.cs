using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour 
    {
        public PlayerController PlayerController {  get; private set; }
        public PlayerAim PlayerAim {  get; private set; }
        public PlayerMovement Movement {  get; private set; }
        public PlayerWeaponController WeaponController { get; private set; }
        
        public WeaponVisualController WeaponVisualController { get; private set; }

        private void Awake()
        {
            PlayerController = new PlayerController();
            PlayerAim = GetComponent<PlayerAim>();
            Movement = GetComponent<PlayerMovement>();
            WeaponController = GetComponent<PlayerWeaponController>();
            WeaponVisualController = GetComponent<WeaponVisualController>();
        }

        private void OnEnable()
        {
            PlayerController.Enable();
        }

        private void OnDisable()
        {
            PlayerController.Disable();
        }
    }
}
