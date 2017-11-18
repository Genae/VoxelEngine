using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayerTD.Runes
{
    public class UpgradeRune : Rune
    {
        //can upgrade
        private readonly bool _towerUpgrade;
        private readonly bool _farmUpgrade;
        private readonly bool _mobUpgrade;
        private readonly bool _villageUpgrade;
        private readonly bool _pathUpgrade;
        
        public Transform UpgradeTarget;
        private LineRenderer _lr;

        public UpgradeRune(bool towerUpgrade, bool farmUpgrade, bool mobUpgrade, bool villageUpgrade, bool pathUpgrade)
        {
            _towerUpgrade = towerUpgrade;
            _farmUpgrade = farmUpgrade;
            _mobUpgrade = mobUpgrade;
            _villageUpgrade = villageUpgrade;
            _pathUpgrade = pathUpgrade;
        }


        void Start()
        {
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
            var list = GetUpgradeableRunes();
            GetClosestUpgradeTarget(list);
            ConfigLineRenderer();
        }

        private List<Transform> GetUpgradeableRunes()
        {
            var upgradeableList = new List<Transform>();
            if (_towerUpgrade)
            {
                foreach (var t in FindObjectsOfType<Algiz>())
                {
                    upgradeableList.Add(t.transform);
                }
            }
            if (_farmUpgrade)
            {
                foreach (var t in FindObjectsOfType<Jera>())
                {
                    upgradeableList.Add(t.transform);
                }
            }
            if (_mobUpgrade)
            {
                foreach (var t in FindObjectsOfType<Isa>())
                {
                    upgradeableList.Add(t.transform);
                }
            }
            if (_villageUpgrade)
            {
                foreach (var t in FindObjectsOfType<Mannaz>())
                {
                    upgradeableList.Add(t.transform);
                }
            }
            if (_pathUpgrade)
            {
                foreach (var t in FindObjectsOfType<Raido>())
                {
                    upgradeableList.Add(t.transform);
                }
            }
            return upgradeableList;
        }

        private void GetClosestUpgradeTarget(List<Transform> allTargets)
        {
            UpgradeTarget = null;
            var dist = 20f;
            foreach (var t in allTargets)
            {
                if (Vector3.Distance(transform.position, t.position) < dist)
                {
                    UpgradeTarget = t;
                    dist = Vector3.Distance(transform.position, t.position); //awesome
                }
            }
        }

        private void ConfigLineRenderer()
        {
            if (UpgradeTarget == null)
            {
                _lr.positionCount = 0;
                return;
            }
            _lr.positionCount = 2;
            _lr.SetPosition(0, transform.position);
            _lr.SetPosition(1, UpgradeTarget.position);
            _lr.startWidth = 1;
            _lr.endWidth = 1;
            //_lr.startColor = Color.black;
            //_lr.endColor = Color.black;
        }
    }
}