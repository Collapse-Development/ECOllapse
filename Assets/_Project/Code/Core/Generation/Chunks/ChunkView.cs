using System.Collections.Generic;
using UnityEngine;

namespace Code.Core.Chunks
{
    public class ChunkViewPool : MonoBehaviour
    {
        [SerializeField] private GameObject chunkPrefab;
        [SerializeField] private int poolSize = 20;

        private Queue<GameObject> pool = new Queue<GameObject>();

        private void Awake()
        {
            for (int i = 0; i < poolSize; i++)
            {
                var obj = Instantiate(chunkPrefab, transform);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        }

        public GameObject Get()
        {
            if (pool.Count > 0)
                return pool.Dequeue();

            var obj = Instantiate(chunkPrefab, transform);
            obj.SetActive(false);
            return obj;
        }

        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
}
