using Object_Pool;
using UnityEngine;

namespace Enemy
{
    public class EnemyAxe : MonoBehaviour
    {
        [SerializeField] private GameObject impactFX;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform axeVisual;
    
    
        private Transform _player;
        private float _flySpeed;
        private float _rotationSpeed;
        private Vector3 _axeDirection;
        private float _timer;

        public void AxeSetup(Transform player, float flySpeed, float rotationSpeed, float timer)
        {
            _player = player;
            _flySpeed = flySpeed;
            _rotationSpeed = rotationSpeed;
            _timer = timer;
        }

        private void Update()
        {
            axeVisual.Rotate(Vector3.right * (_rotationSpeed * Time.deltaTime));

            _timer -= Time.deltaTime;

            if (_timer > 0)
            {
                _axeDirection = _player.position + Vector3.up - axeVisual.position;
            }
        
            rb.linearVelocity = _axeDirection.normalized * _flySpeed;
            transform.forward = rb.linearVelocity;
        }

        private void OnTriggerEnter(Collider other)
        {
            Bullet bullet = other.GetComponent<Bullet>();
            Player.Player player = other.GetComponent<Player.Player>();

            if (bullet || player)
            {
                GameObject newFx = ObjectPool.Instance.GetObject(impactFX);
                newFx.transform.position = transform.position;
            
                ObjectPool.Instance.ReturnObject(gameObject);
                ObjectPool.Instance.ReturnObject(newFx, 0.5f);
            }
        }
    }
}
