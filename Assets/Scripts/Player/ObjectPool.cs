using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance;

        [SerializeField] private int poolSize;

        private readonly Dictionary<GameObject, Queue<GameObject>> _poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void InitializeNewPool(GameObject prefab)
        {
            _poolDictionary[prefab] = new Queue<GameObject>();
            
            for (int i = 0; i < poolSize; i++)
            {
                CreateNewObject(prefab);
            }
        }

        private void CreateNewObject(GameObject prefab)
        {
            GameObject newObject = Instantiate(prefab, transform);
            newObject.AddComponent<PooledObject>().OriginalPrefab = prefab;
            newObject.SetActive(false);
            _poolDictionary[prefab].Enqueue(newObject);
        }

        public GameObject GetObject(GameObject prefab)
        {
            if(!_poolDictionary.ContainsKey(prefab)) InitializeNewPool(prefab);
            
            if (_poolDictionary[prefab].Count == 0) CreateNewObject(prefab);
            
            GameObject objectToGet = _poolDictionary[prefab].Dequeue();
            objectToGet.SetActive(true);
            objectToGet.transform.parent = null;
            
            return objectToGet;
        }

        private void ReturnToPool(GameObject objectToReturn)
        {
            GameObject originalPrefab = objectToReturn.GetComponent<PooledObject>().OriginalPrefab;
            
            objectToReturn.SetActive(false);
            objectToReturn.transform.parent = transform;
            
            _poolDictionary[originalPrefab].Enqueue(objectToReturn);
        }

        public void ReturnObject(GameObject objectToReturn, float delay = .001f) =>
            StartCoroutine(DelayReturnToPool(objectToReturn, delay));

        private IEnumerator DelayReturnToPool(GameObject objectToReturn, float delay)
        {
            yield return new WaitForSeconds(delay);
            ReturnToPool(objectToReturn);
        }
    }
}
