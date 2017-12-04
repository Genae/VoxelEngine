using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogicLayerTD.Runes;
using Assets.Scripts.UI;
using UnityEngine;


namespace Assets.Scripts.GameLogicLayerTD
{
    public class TDMinion : MonoBehaviour
    {
        public List<Vector3> Path;
        private int wayIndex = 0;
        private Vector3 targetVector;
        private float _speed = 10;
        private float _health = 100;
        private float _scale = 1;
        public float DistanceMoved = 0f;
        private float _slowIntensity = 1f;
        private float _slowTime = 0;
        private Healthbar _healthbar;
        private List<ElementType> _elementList;

        public void Init(List<Vector3> path, List<ElementType> elementList, float speed, float health, float scale)
        {
            _speed = speed;
            _health = health;
            _scale = scale;
            SetPath(path);
            transform.position = path[0];
            transform.localScale = Vector3.one * scale;
            _elementList = new List<ElementType>(); //init list
            _elementList = elementList;
            SetColor(_elementList[0]); //well :D dont judge me
        }

        void Start()
        {
            _healthbar = gameObject.AddComponent<Healthbar>();
            _healthbar.Init(_health, _health);
        }

        void OnDestroy()
        {
            WaveManager.AliveMinions.Remove(this);
        }

        private void SetPath(List<Vector3> path)
        {
            Path = path;
            wayIndex = 0;
            targetVector = path[0];
        }

        public bool ApplyDmg(float dmg, List<ElementType> projElementList)
        {
            _health -= DamageCalculator.Calc(dmg, projElementList, _elementList);
            _healthbar.UpdateCurrentHealth(_health);
            AliveCheck();
            return _health <= 0;
        }

        private void AliveCheck()
        {
            if (_health <= 0)
            {
                Destroy(gameObject);
            }
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
            SlowCheck();

            var oldPos = transform.position;
            if (wayIndex <= Path.Count - 1)
            {
                if (Vector3.Distance(transform.position, targetVector) < 1f)
                {
                    targetVector = Path[wayIndex];
                    wayIndex++;
                }
            }
            else
            {
                if ((transform.position - Path[wayIndex - 1]).magnitude < 1f)
                {
                    var gebo = TDMap.Instance.Villages.First().Marker.GetUpgradeRunes().OfType<Gebo>().ToList();
                    if (gebo.Any() && ResourceOverview.Instance.Gold.Value > 0)
                    {
                        ResourceOverview.Instance.Gold.Value -= (int)(20 * Mathf.Pow(0.9f, gebo.Count));
                    }
                    else
                    {
                        ResourceOverview.Instance.Lives.Value -= 1;
                    }
                    Destroy(gameObject);
                }
            }
            var q1 = Quaternion.LookRotation(targetVector - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, q1, Time.deltaTime);

            var moveVector = (targetVector - oldPos).normalized * Time.deltaTime * _speed * _slowIntensity;
            transform.position += moveVector;
            DistanceMoved += moveVector.magnitude;
        }

        private void SlowCheck()
        {
            if (_slowTime <= 0)
                _slowIntensity = 1f;
            else
                _slowTime -= Time.deltaTime;
        }

        public void ApplySlow(float slowIntensity)
        {
            if (_slowIntensity > slowIntensity)
                _slowIntensity = slowIntensity;
            _slowTime += 2;
        }
    }
}