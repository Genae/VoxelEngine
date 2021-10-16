using EngineLayer.Util;
using UnityEngine;

namespace AccessLayer.Jobs
{
    public class JobMarker : MonoBehaviour
    {
        public void Init(Vector3 position, float scale, Color color)
        {
            transform.position = position;
            transform.localScale = Vector3.one * scale;
            transform.GetComponent<MeshRenderer>().material.color = color;
        }

        public void Destroy()
        {
            name = "JobMarker";
            ObjectPool.Instance.PoolObject(gameObject);
        }
    }
}
