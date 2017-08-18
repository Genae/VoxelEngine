using System.Collections.Generic;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Logic.Tools;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Logic.Jobs
{
    public abstract class PositionedJob : Job
    {
        public abstract JobType GetJobType();

        public JobMarker Marker;
        public Vector3 Position;

        protected PositionedJob(Vector3 position, float scale, Color color, Overlay jobOverlay)
        {
            Position = position;
            Marker = ObjectPool.Instance.GetObjectForType<JobMarker>(parent: OverlayManager.GetOverlay(jobOverlay));
            Marker.Init(position, scale, color);
        }

        public override List<Vector3> GetPossibleWorkLocations()
        {
            return GetStandingPositions(Position);
        }
        
        public override bool Solve(float deltaTime, GameObject actor)
        {
            if (!base.Solve(deltaTime, actor))
                return false;
            GameObject.Find("World").GetComponent<JobController>().SolveJob(this);
            Marker.Destroy();
            return true;
        }

        public override void Abort()
        {
            base.Abort();
            GameObject.Find("World").GetComponent<JobController>().SolveJob(this);
            Marker.Destroy();
        }
    }

    public abstract class Job
    {
        public bool Aborted;
        protected float RemainingTime;

        public virtual void Abort()
        {
            Aborted = true;
        }
        public virtual bool Solve(float deltaTime, GameObject actor)
        {
            RemainingTime -= deltaTime;
            if (RemainingTime > 0)
                return false;
            SolveInternal(actor);
            return true;
        }
        
        protected List<Vector3> GetStandingPositions(Vector3 pos)
        {
            var locs = new List<Vector3>();
            for (var dx = -1; dx <= 1; dx++)
            {
                for (var dy = -3; dy <= 0; dy++)
                {
                    for (var dz = -1; dz <= 1; dz++)
                    {
                        if (dx == 0 && dy == 1 && dz == 0)
                            continue; //never dig straight down
                        if (!Map.Instance.IsInBounds((int)pos.x + dx, (int)pos.y + dy, (int)pos.z + dz))
                            continue;
                        if (Map.Instance.AStarNetwork.GetNode(new Vector3(pos.x + dx, (int)pos.y + dy, (int)pos.z + dz)) != null)
                        {
                            locs.Add(new Vector3((int)pos.x + dx, (int)pos.y + dy, (int)pos.z + dz));
                        }
                    }
                }
            }
            return locs;
        }

        protected abstract void SolveInternal(GameObject actor);
        public abstract List<Vector3> GetPossibleWorkLocations();
    }
    
    public enum JobType
    {
        Mining,
        CreateSoil,
        PlantCrop,
        HarvestCrop,
        Building
    }
}
