using System.Collections.Generic;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Logic.Jobs;
using UnityEngine;

namespace Assets.Scripts.Logic.Tools
{
    public class DeleteTool : AreaTool
    {
        private JobController _jobController;

        void Awake()
        {
            _jobController = GameObject.Find("World").GetComponent<JobController>();
        }

        protected override void StartAction(IEnumerable<Vector3> voxels)
        {
            foreach (var vox in voxels)
            {
                ClearVoxelAtPosition(vox);
            }
        }

        protected override Color GetPreviewColor()
        {
            return new Color(1f, 0, 0, 0.5f);
        }

        private void ClearVoxelAtPosition(Vector3 pos)
        {
            if (_jobController.HasJob(pos, JobType.Mining))
                return;
            
            var type = Map.Instance.MapData.GetVoxelMaterial(pos);
            if (type.Equals(MaterialRegistry.Instance.GetMaterialFromName("Air")))
                return;
            _jobController.AddJob(new MiningJob(pos));
        }

        public override void SwapOverlays()
        {
            OverlayManager.SwapOverlays(true, true, false);
        }
    }
}
