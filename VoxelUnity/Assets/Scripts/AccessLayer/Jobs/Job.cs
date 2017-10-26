using System.Collections.Generic;
using Assets.Scripts.EngineLayer.Voxels.Containers;
using UnityEngine;

namespace Assets.Scripts.AccessLayer.Jobs
{
    public abstract class Job
    {
        public abstract string GetJobType();
        public bool Aborted;
        protected float RemainingTime;

        public virtual void Abort()
        {
            Aborted = true;
        }
        public virtual bool Solve(float deltaTime, GameObject actor, float actorSkill)
        {
            RemainingTime -= deltaTime * actorSkill;
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
}