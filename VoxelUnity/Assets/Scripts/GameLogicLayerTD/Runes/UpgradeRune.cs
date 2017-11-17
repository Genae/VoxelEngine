using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayerTD.Runes
{
    public class UpgradeRune : Rune
    {
        private List<Transform> _towerList;
        public Transform Tower;
        private LineRenderer _lr;

        void Start()
        {
            _towerList = new List<Transform>();
            //linerenderer init
            if ((_lr = gameObject.GetComponent<LineRenderer>()) == null)
            {
                _lr = gameObject.AddComponent<LineRenderer>();
                var mat = new Material(Shader.Find("Standard"));
                mat.color = Color.black;
                _lr.material = mat;
            }
        }

        void Update()
        {

            GetTowers();
            GetClosestTower();
            ConfigLineRenderer();
        }

        private void GetTowers()
        {
            _towerList.Clear();
            foreach (var t in FindObjectsOfType<Algiz>())
            {
                _towerList.Add(t.transform);
            }
        }

        private void GetClosestTower()
        {
            Tower = null;
            var dist = 20f;
            foreach (var t in _towerList)
            {
                if (Vector3.Distance(transform.position, t.position) < dist)
                {
                    Tower = t;
                    dist = Vector3.Distance(transform.position, t.position); //awesome
                }
            }
        }

        private void ConfigLineRenderer()
        {
            if (Tower == null)
            {
                _lr.positionCount = 0;
                return;
            }
            _lr.positionCount = 2;
            _lr.SetPosition(0, transform.position);
            _lr.SetPosition(1, Tower.position);
            _lr.startWidth = 1;
            _lr.endWidth = 1;
            //_lr.startColor = Color.black;
            //_lr.endColor = Color.black;
        }
    }
}