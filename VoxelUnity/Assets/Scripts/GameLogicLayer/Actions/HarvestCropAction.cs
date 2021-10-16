﻿using AccessLayer.Farming;
using AccessLayer.Jobs;
using AccessLayer.Tools;
using UnityEngine;

namespace GameLogicLayer.Actions
{
    public class HarvestCropAction : SolveJobAction
    {
        public HarvestCropAction() : base("hasHarvested", "HarvestCrop", 1.5f)
        { }
    }

    public class HarvestCropJob : PositionedJob
    {
        private readonly FarmBlock _block;

        public HarvestCropJob(FarmBlock block) : base(block.Position + Vector3.up, 1.1f, new Color(0.7f, 0.7f, 0f, 0.5f), Overlay.Farming, "HarvestCrop")
        {
            _block = block;
            RemainingTime = 1f;
        }

        protected override void SolveInternal(GameObject actor)
        {
            _block.Stage = 0;
        }
    }
}
