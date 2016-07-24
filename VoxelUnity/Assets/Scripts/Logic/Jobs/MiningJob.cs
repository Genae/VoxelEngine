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

        public MiningJob(Vector3 position) : base(position, 1.1f, new Color(1f, 0f, 0f, 0.5f))
        {}

        public override bool Solve(float deltaTime)
        {
            _remainingTime -= deltaTime;
            if (_remainingTime > 0)
                return false;
            Map.MapData.Chunks[(int)Position.x/16, (int)Position.y/16, (int)Position.z/16]
                .SetVoxelType((int)Position.x%16, (int)Position.y%16, (int)Position.z%16, MaterialRegistry.Air);
            Marker.Destroy();
            return true;
        }
    }
}
