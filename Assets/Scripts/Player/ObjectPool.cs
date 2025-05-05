using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance;

        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private int bulletPoolSize;

        private Queue<GameObject> _bulletPool;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            _bulletPool = new Queue<GameObject>();
            CreateBulletPool();
        }

        private void CreateBulletPool()
        {
            for (int i = 0; i < bulletPoolSize; i++)
            {
                CreateNewBullet();
            }
        }

        private void CreateNewBullet()
        {
            GameObject newBullet = Instantiate(bulletPrefab, transform);
            newBullet.SetActive(false);
            _bulletPool.Enqueue(newBullet);
        }

        public GameObject GetBullet()
        {
            if (_bulletPool.Count == 0)
                CreateNewBullet();
            
            GameObject bullet = _bulletPool.Dequeue();
            bullet.SetActive(true);
            bullet.transform.parent = null;
            
            return bullet;
        }

        public void ReturnBullet(GameObject bullet)
        {
            bullet.SetActive(false);
            _bulletPool.Enqueue(bullet);
            bullet.transform.parent = transform;
        }
    }
}
