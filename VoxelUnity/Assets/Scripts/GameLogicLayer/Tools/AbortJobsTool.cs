using System.Collections.Generic;
using Assets.Scripts.AccessLayer.Tools;
using Assets.Scripts.Logic;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayer.Tools
{
    public class AbortJobsTool : AreaTool
    {
        protected override void StartAction(IEnumerable<Vector3> voxels)
        {
            foreach (var vox in voxels)
            {
                ClearJobAtPosition(vox);
            }
        }

        protected override Color GetPreviewColor()
        {
            return new Color(1f, 0.3f, 0.3f, 0.5f);
        }

        private void ClearJobAtPosition(Vector3 pos)
        {
            var jobs = JobController.Instance.GetJobAt(pos);
            if(jobs != null && jobs.Count > 0)
            foreach (var job in jobs.ToArray())
            {
                job.Abort();
            }
        }

        public override void SwapOverlays()
        {
            OverlayManager.SwapOverlays(true, true, true);
        }
    }
}
