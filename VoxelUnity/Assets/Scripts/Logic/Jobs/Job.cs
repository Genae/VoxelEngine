using System.Collections.Generic;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Logic.Tools;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Logic.Jobs
{
    public abstract class Job
    {
        protected Map Map;

        public abstract JobType GetJobType();

        protected abstract void SolveInternal();

        public JobMarker Marker;

        public Vector3 Position;

        protected float RemainingTime;

        protected Job(Vector3 position, float scale, Color color, Overlay jobOverlay)
        {
            Position = position;
            Map = Map.Instance;
            Marker = ObjectPool.Instance.GetObjectForType<JobMarker>(parent: OverlayManager.GetOverlay(jobOverlay));
            Marker.Init(position, scale, color);
        }

        public List<Vector3> GetPossibleWorkLocations()
        {
            var locs = new List<Vector3>();
            for (var dx = -1; dx <= 1; dx++)
            {
                for (var dy = -3; dy <= 0; dy++)
                {
                    for (var dz = -1; dz <= 1; dz++)
                    {
                        if(dx == 0 && dy == 1 && dz == 0)
                            continue; //never dig straight down
                        if (!Map.Instance.IsInBounds((int) Position.x + dx, (int) Position.y + dy,(int) Position.z + dz))
                            continue;
                        if (Map.AStarNetwork.GetNode(new Vector3(Position.x + dx, (int) Position.y + dy,(int) Position.z + dz)) != null)
                        {
                            locs.Add(new Vector3((int)Position.x + dx, (int)Position.y + dy, (int)Position.z + dz));
                        }
                    }
                }
            }
            return locs;
        }

        public bool Solve(float deltaTime)
        {
            RemainingTime -= deltaTime;
            if (RemainingTime > 0)
                return false;
            SolveInternal();
            GameObject.Find("World").GetComponent<JobController>().SolveJob(this);
            Marker.Destroy();
            return true;
        }
    }
    
    public enum JobType
    {
        Mining,
        CreateSoil
    }
}
