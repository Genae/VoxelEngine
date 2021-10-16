using System.Collections.Generic;
using AccessLayer;
using AccessLayer.Material;
using AccessLayer.Tools;
using AccessLayer.Worlds;
using GameLogicLayer.Actions;
using UnityEngine;

namespace GameLogicLayer.Tools
{
    public class PlaceWaterSourceTool : AreaTool {
        
        public int BlockMaterialId = 1;

        protected override Color GetPreviewColor()
        {
            var c = MaterialRegistry.Instance.MaterialFromId(BlockMaterialId).Color;
            return new Color(c.r, c.g, c.b, 0.5f);
        }

        protected override void StartAction(IEnumerable<Vector3> voxels)
        {
            foreach (var vox in voxels)
            {
                PlaceSourceAtPosition(vox);
            }
        }

        private void PlaceSourceAtPosition(Vector3 pos)
        {
            World.At(pos).SetVoxel(MaterialRegistry.Instance.GetMaterialFromName("WaterSource"));
        }

        public override void SwapOverlays()
        {
            OverlayManager.SwapOverlays(true, true, false);
        }
    }
}
