using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using UnityEngine;

namespace Assets.Scripts.Logic.Jobs
{
    public class MiningJob : Job
    {
        private float _remainingTime = 0.1f;
        public override JobType GetJobType()
        {
            return JobType.Mining;
        }

        public override bool Solve(float deltaTime)
        {
            _remainingTime -= deltaTime;
            if (_remainingTime > 0)
                return false;
            GameObject.Find("Map").GetComponent<Map>().MapData.Chunks[(int)transform.position.x/16, (int)transform.position.y/16, (int)transform.position.z/16]
                .SetVoxelType((int)transform.position.x%16, (int)transform.position.y%16, (int)transform.position.z%16, MaterialRegistry.Air);
            DestroyImmediate(gameObject);
            return true;
        }
    }
}
