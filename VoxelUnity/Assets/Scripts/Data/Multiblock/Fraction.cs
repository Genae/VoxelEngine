using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Data.Multiblock
{
    public class Fraction : MonoBehaviour {

        public void Init(Vector3 position, float scale, Color color, Vector3 middle, float force = 100f)
        {
            transform.position = position;
            transform.localScale = Vector3.one * scale;
            transform.GetComponent<MeshRenderer>().material.color = color;
            transform.GetComponent<Rigidbody>().AddExplosionForce(force, middle, 20f);
            Invoke("Destroy", Random.Range(3f, 4f));
        }

        public void Destroy()
        {
            name = "JobMarker";
            ObjectPool.Instance.PoolObject(gameObject);
        }
    }
}
