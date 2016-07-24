using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public class ObjectPool : MonoBehaviour
    {

        public static ObjectPool Instance;
        [SerializeField]
        protected GameObject[] ObjectPrefabs;
        [SerializeField]
        protected List<GameObject>[] PooledObjects;
        [SerializeField]
        protected int[] MinAmountToBuffer;
        [SerializeField]
        protected int[] MaxAmountToBuffer;
        [SerializeField]
        protected int DefaultBufferAmount;
        [SerializeField]
        protected int FillObjectsPerFrame;
        protected GameObject ContainerObject;

        public ObjectPool(GameObject[] objectPrefabs)
        {
            ObjectPrefabs = objectPrefabs;
        }

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            ContainerObject = gameObject;
            PooledObjects = new List<GameObject>[ObjectPrefabs.Length];
            var minLength = MinAmountToBuffer.Length;
            if (MinAmountToBuffer.Length < ObjectPrefabs.Length)
            {
                Array.Resize(ref MinAmountToBuffer, ObjectPrefabs.Length);
            }
            var maxLength = MaxAmountToBuffer.Length;
            if (MaxAmountToBuffer.Length < ObjectPrefabs.Length)
            {
                Array.Resize(ref MaxAmountToBuffer, ObjectPrefabs.Length);
            }
            for (var i = 0; i < ObjectPrefabs.Length; i++)
            {
                PooledObjects[i] = new List<GameObject>();
                if (i >= minLength)
                {
                    MinAmountToBuffer[i] = DefaultBufferAmount;
                }
                if (i > maxLength)
                {
                    MaxAmountToBuffer[i] = MaxAmountToBuffer[i] > MinAmountToBuffer[i] ? MaxAmountToBuffer[i] : MinAmountToBuffer[i] * 10;
                }
            }
        }

        void Update()
        {
            var count = 0;
            var i = 0;
            foreach (var objectPrefab in ObjectPrefabs)
            {
                var bufferAmount = i < MinAmountToBuffer.Length ? MinAmountToBuffer[i] : DefaultBufferAmount;

                for (var n = PooledObjects[i].Count; n < bufferAmount; n++)
                {
                    var newObj = Instantiate(objectPrefab);
                    newObj.name = objectPrefab.name;
                    PoolObject(newObj);
                    if (count++ >= FillObjectsPerFrame)
                        return;
                }

                i++;
            }
        }

        public GameObject GetObjectForType(string objectType, Transform parent = null, bool onlyPooled = false)
        {
            for (var i = 0; i < ObjectPrefabs.Length; i++)
            {
                var prefab = ObjectPrefabs[i];
                if (prefab.name != objectType)
                    continue;
                if (PooledObjects[i].Count > 0)
                {
                    var pooledObject = PooledObjects[i][0];
                    PooledObjects[i].RemoveAt(0);
                    pooledObject.transform.parent = parent;
                    pooledObject.SetActive(true);

                    return pooledObject;

                }
                if (!onlyPooled)
                {
                    var newObj = Instantiate(ObjectPrefabs[i]);
                    newObj.name = ObjectPrefabs[i].name;
                    newObj.transform.parent = parent;
                    Debug.Log("missing");
                    return newObj;
                }

                break;
            }
            return null;
        }

        public T GetObjectForType<T>(string prefabName = null, Transform parent = null, bool onlyPooled = false)
        {
            if (prefabName == null)
                prefabName = typeof(T).Name;
            return GetObjectForType(prefabName, parent, onlyPooled).GetComponent<T>();
        }

        public void PoolObject(GameObject obj)
        {
            for (var i = 0; i < ObjectPrefabs.Length; i++)
            {
                if (ObjectPrefabs[i].name != obj.name)
                    continue;
                obj.SetActive(false);
                obj.transform.parent = ContainerObject.transform;
                if (PooledObjects[i].Count < MaxAmountToBuffer[i])
                {
                    PooledObjects[i].Add(obj);
                }
                else
                {
                    Destroy(obj);
                }
                return;
            }
        }

    }
}