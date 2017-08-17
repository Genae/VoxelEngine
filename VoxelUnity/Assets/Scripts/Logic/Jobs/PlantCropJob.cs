using Assets.Scripts.Data.Material;
using Assets.Scripts.Logic.Farming;
using Assets.Scripts.Logic.Tools;
using UnityEngine;

namespace Assets.Scripts.Logic.Jobs
{
    public class PlantCropJob : Job
    {
        private readonly FarmBlock _block;

        public override JobType GetJobType()
        {
            return JobType.PlantCrop;
        }

        public PlantCropJob(FarmBlock block) : base(block.Position + Vector3.up, 1.1f, new Color(0f, 0.7f, 0.1f, 0.5f), Overlay.Farming)
        {
            _block = block;
            RemainingTime = 1f;
        }

        protected override void SolveInternal(GameObject actor)
        {
            _block.Stage = 1;
        }
    }
}
