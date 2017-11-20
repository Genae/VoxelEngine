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
        private float _health = 100;
        private Healthbar _healthbar;
        private List<ElementType> _elementList;

        public void Init(List<Vector3> path, List<ElementType> elementList)
        {
            SetPath(path);
            transform.position = path[0];
            _elementList = new List<ElementType>(); //init list
            _elementList = elementList;
            SetColor(_elementList[0]); //well :D dont judge me
        }

        void Start()
        {
            _healthbar = gameObject.AddComponent<Healthbar>();
            _healthbar.Init(_health, _health);
        }

        private void SetPath(List<Vector3> path)
        {
            Path = path;
            wayIndex = 0;
            targetVector = path[0];
        }

        public void ApplyDmg(float dmg, List<ElementType> projElementList)
        {
            _health -= DamageCalculator.Calc(dmg, projElementList, _elementList);
            _healthbar.UpdateCurrentHealth(_health);
        }

        private void AliveCheck()
        {
            if (_health <= 0) Destroy(gameObject);
        }

        private void SetColor(ElementType type)
        {
            var m = GetComponent<MeshRenderer>().material;
            if (type == ElementType.Air) m.color = Color.grey;
            else if (type == ElementType.Water) m.color = Color.blue;
            else if (type == ElementType.Earth) m.color = Color.black;
            else if (type == ElementType.Fire) m.color = Color.red;
            else if (type == ElementType.Light) m.color = Color.yellow;
            else m.color = Color.white;
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