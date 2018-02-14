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
        private Dictionary<string, GameObject> _models = new Dictionary<string, GameObject>();
        public bool spawn = true;

        public static List<TDMinion> AliveMinions;

        void Start()
        {
            Reset();
        }

        public void Reset()
        {
            AliveMinions = new List<TDMinion>();
            _cm = CampaignManager.Instance;
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
            spawn = SpawnUnit(TDMap.Instance.Path);
        }


        private bool SpawnUnit(List<Vector3> instancePath)
        {
            var mobStats = currentWave.GetMobStats();
            if (mobStats == null)
                return false;
            if (mobStats.SpawnTime <= 0)
                mobStats.SpawnTime = 1f;
             currentCooldown = cooldown*mobStats.SpawnTime;
            var go = Instantiate(GetModel(mobStats.Model));
            var minion = go.AddComponent<TDMinion>();
            AliveMinions.Add(minion);
            minion.Init(instancePath, mobStats.ElementList.ToList(), mobStats.Speed, mobStats.Health, mobStats.Scale);
            go.transform.parent = transform;
            return true;
        }

        private GameObject GetModel(string model)
        {
            if (!_models.ContainsKey(model))
            {
                _models[model] = GameObject.Find(model);
            }
            return _models[model];
        }
    }
}