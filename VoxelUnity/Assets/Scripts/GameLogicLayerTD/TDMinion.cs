using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayerTD
{
    public class TDMinion : MonoBehaviour
    {
        public List<Vector3> Path;
        private int wayIndex = 0;
        private Vector3 targetVector;
        private float speed = 10;

        public void SetPath(List<Vector3> path)
        {
            Path = path;
            wayIndex = 0;
            targetVector = path[0];

        }
        void Update()
        {
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
                Destroy(gameObject);
            }
            var q1 = Quaternion.LookRotation(targetVector - this.transform.position);

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, q1, Time.deltaTime);

            this.transform.position += ((targetVector - oldPos).normalized * Time.deltaTime * speed);
        }
    }
}