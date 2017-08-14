using Assets.Scripts.Data.Material;
using Assets.Scripts.Logic.Tools;
using UnityEngine;

namespace Assets.Scripts.Logic.Jobs
{
    public class CreateSoilJob : Job
    {
        public override JobType GetJobType()
        {
            return JobType.CreateSoil;
        }

        public CreateSoilJob(Vector3 position) : base(position, 1.1f, new Color(0f, 0.3f, 0.3f, 0.5f), Overlay.Farming)
        {
            RemainingTime = 1f;
        }

        protected override void SolveInternal()
        {
            Map.MapData.Chunks[(int)Position.x/16, (int)Position.y/16, (int)Position.z/16]
                .SetVoxelType((int)Position.x%16, (int)Position.y%16, (int)Position.z%16, MaterialRegistry.Instance.GetMaterialFromName("Soil"));
        }
    }
}
