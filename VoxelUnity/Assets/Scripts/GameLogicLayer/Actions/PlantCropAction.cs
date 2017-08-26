using Assets.Scripts.AccessLayer.Farming;
using Assets.Scripts.AccessLayer.Jobs;
using Assets.Scripts.AccessLayer.Tools;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayer.Actions
{
    public class PlantCropAction : SolveJobAction
    {
        public PlantCropAction() : base("hasPlanted", "PlantCrop", 1.5f)
        { }
    }

    public class PlantCropJob : PositionedJob
    {
        private readonly FarmBlock _block;

        public PlantCropJob(FarmBlock block) : base(block.Position + Vector3.up, 1.1f, new Color(0f, 0.7f, 0.1f, 0.5f), Overlay.Farming, "PlantCrop")
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
