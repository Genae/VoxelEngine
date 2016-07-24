using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public class ObjectPool : MonoBehaviour
    {

        public static ObjectPool Instance;
        [SerializeField] private GameObject[] _objectPrefabs;
        [SerializeField] private List<GameObject>[] _pooledObjects;
        [SerializeField] private int[] _minAmountToBuffer;
        [SerializeField] private int[] _maxAmountToBuffer;
        [SerializeField] private int _defaultBufferAmount = 300;
        [SerializeField] private int FillObjectsPerFrame = 100;
        protected GameObject ContainerObject;

        public ObjectPool(GameObject[] objectPrefabs)
        {
            _objectPrefabs = objectPrefabs;
        }

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            ContainerObject = gameObject;
            _pooledObjects = new List<GameObject>[_objectPrefabs.Length];
            var minLength = _minAmountToBuffer.Length;
            if (_minAmountToBuffer.Length < _objectPrefabs.Length)
            {
                Array.Resize(ref _minAmountToBuffer, _objectPrefabs.Length);
            }
            var maxLength = _maxAmountToBuffer.Length;
            if (_maxAmountToBuffer.Length < _objectPrefabs.Length)
            {
                Array.Resize(ref _maxAmountToBuffer, _objectPrefabs.Length);
            }
            for (var i = 0; i < _objectPrefabs.Length; i++)
            {
                _pooledObjects[i] = new List<GameObject>();
                if (i >= minLength)
                {
                    _minAmountToBuffer[i] = _defaultBufferAmount;
                }
                if (i > maxLength)
                {
                    _maxAmountToBuffer[i] = _maxAmountToBuffer[i] > _minAmountToBuffer[i] ? _maxAmountToBuffer[i] : _minAmountToBuffer[i] * 10;
                }
            }
        }

        void Update()
        {
            var count = 0;
            var i = 0;
            foreach (var objectPrefab in _objectPrefabs)
            {
                var bufferAmount = i < _minAmountToBuffer.Length ? _minAmountToBuffer[i] : _defaultBufferAmount;

                for (var n = _pooledObjects[i].Count; n < bufferAmount; n++)
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
            for (var i = 0; i < _objectPrefabs.Length; i++)
            {
                var prefab = _objectPrefabs[i];
                if (prefab.name != objectType)
                    continue;
                if (_pooledObjects[i].Count > 0)
                {
                    var pooledObject = _pooledObjects[i][0];
                    _pooledObjects[i].RemoveAt(0);
                    pooledObject.transform.parent = parent;
                    pooledObject.SetActive(true);

                    return pooledObject;

                }
                if (!onlyPooled)
                {
                    var newObj = Instantiate(_objectPrefabs[i]);
                    newObj.name = _objectPrefabs[i].name;
                    newObj.transform.parent = parent;
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
            for (var i = 0; i < _objectPrefabs.Length; i++)
            {
                if (_objectPrefabs[i].name != obj.name)
                    continue;
                obj.SetActive(false);
                obj.transform.parent = ContainerObject.transform;
                if (_pooledObjects[i].Count < _maxAmountToBuffer[i])
                {
                    _pooledObjects[i].Add(obj);
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