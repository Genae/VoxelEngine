using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayerTD
{
    public class WaveManager : MonoBehaviour
    {
        private float currentCooldown;
        public float cooldown = 2f;
        public Wave currentWave;
        private CampaignManager _cm;
        private GameObject _bunny;
        public bool spawn = true;

        public static List<TDMinion> AliveMinions;

        void Start()
        {
            AliveMinions = new List<TDMinion>();
            _cm = FindObjectOfType<CampaignManager>();
            _bunny = GameObject.Find("bunny_root");
            currentWave = _cm.GetNextWave();
        }

        void Update()
        {
            if (!spawn)
            {
                currentWave = _cm.GetNextWave();
                spawn = currentWave != null;
                return;
            }
            if (TDMap.Instance.Path.Count == 0)
                return; //no path
            currentCooldown -= Time.deltaTime;
            if (currentCooldown > 0)
                return;
            currentCooldown = cooldown;
            spawn = SpawnUnit(TDMap.Instance.Path);
        }


        private bool SpawnUnit(List<Vector3> instancePath)
        {
            var mobStats = currentWave.GetMobStats();
            if (mobStats == null)
                return false;
            var go = Instantiate(_bunny) as GameObject;
            var minion = go.AddComponent<TDMinion>();
            AliveMinions.Add(minion);
            minion.Init(instancePath, mobStats.ElementList.ToList(), mobStats.Speed, mobStats.Health, mobStats.Scale);
            go.transform.parent = transform;
            return true;
        }
    }
}