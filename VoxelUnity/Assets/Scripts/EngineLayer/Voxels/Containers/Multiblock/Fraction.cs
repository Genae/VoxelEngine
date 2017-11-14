using Assets.Scripts.EngineLayer.Util;
using UnityEngine;

namespace Assets.Scripts.EngineLayer.Voxels.Containers.Multiblock
{
    public class Fraction : MonoBehaviour {
        private float _force;
        private Vector3 _middle;

        public void Init(Vector3 position, float scale, Color color, Vector3 middle, float force = 10f)
        {
            _force = force;
            _middle = middle;
            transform.position = position;
            transform.localScale = Vector3.one * scale;
            transform.GetComponent<MeshRenderer>().material.color = color;
            Invoke("Explode", 0.1f);
            Invoke("Destroy", Random.Range(3f, 4f));
        }

        public void Explode()
        {
            transform.GetComponent<Rigidbody>().AddExplosionForce(_force, _middle, 20f, 1f, ForceMode.VelocityChange);
        }


        public void Destroy()
        {
            name = "JobMarker";
            ObjectPool.Instance.PoolObject(gameObject);
        }
    }
}
