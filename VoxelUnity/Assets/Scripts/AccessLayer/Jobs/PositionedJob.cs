using System.Collections.Generic;
using Assets.Scripts.AccessLayer.Tools;
using Assets.Scripts.EngineLayer.Util;
using UnityEngine;

namespace Assets.Scripts.AccessLayer.Jobs
{
    public abstract class PositionedJob : Job
    {
        public JobMarker Marker;
        public Vector3 Position;
        private readonly string _jobType;

        public PositionedJob(Vector3 position, float scale, Color color, Overlay jobOverlay, string jobType)
        {
            Position = position;
            _jobType = jobType;
            Marker = ObjectPool.Instance.GetObjectForType<JobMarker>(parent: OverlayManager.GetOverlay(jobOverlay));
            Marker.Init(position, scale, color);
            RemainingTime = 1f;
        }
        
        public override List<Vector3> GetPossibleWorkLocations()
        {
            return GetStandingPositions(Position);
        }
        
        public override bool Solve(float deltaTime, GameObject actor, float actorSkill)
        {
            if (!base.Solve(deltaTime, actor, actorSkill))
                return false;
            JobController.Instance.SolveJob(this);
            Marker.Destroy();
            return true;
        }

        public override string GetJobType()
        {
            return _jobType;
        }

        public override void Abort()
        {
            base.Abort();
            JobController.Instance.SolveJob(this);
            Marker.Destroy();
        }
    }
}
