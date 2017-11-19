using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.*;

namespace Assets.Scripts.GameLogicLayerTD
{
    public class TDMinion : MonoBehaviour
    {
        public List<Vector3> Path;
        private int wayIndex = 0;
        private Vector3 targetVector;
        private float speed = 10;
        private float _health = 100;
        private Healthbar _healthbar;
        //private ElementType _type;

        void Start()
        {
            _healthbar = gameObject.AddComponent<Healthbar>();
            _healthbar.Init(_health, _health);
        }

        public void SetPath(List<Vector3> path)
        {
            Path = path;
            wayIndex = 0;
            targetVector = path[0];
        }

        public void ApplyDmg(float dmg)
        {
            _health -= dmg;
            _healthbar.UpdateCurrentHealth(_health);
        }

        private void AliveCheck()
        {
            if (_health <= 0) Destroy(gameObject);
        }

        void Update()
        {
            AliveCheck();

            var oldPos = this.transform.position;
            if (wayIndex <= Path.Count - 1)
            {
                if (Vector3.Distance(this.transform.position, targetVector) < 1f)
                {
                    targetVector = Path[wayIndex];
                    wayIndex++;
                }
            }
            else
            {
                if((this.transform.position - Path[wayIndex-1]).magnitude < 1f)
                Destroy(gameObject);
            }
            var q1 = Quaternion.LookRotation(targetVector - this.transform.position);

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, q1, Time.deltaTime);

            this.transform.position += ((targetVector - oldPos).normalized * Time.deltaTime * speed);
        }
    }
}