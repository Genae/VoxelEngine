using System.Collections.Generic;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Logic.Jobs
{
    public abstract class Job
    {
        protected Map Map;

        public abstract JobType GetJobType();

        public abstract bool Solve(float deltaTime);

        public JobMarker Marker;

        public Vector3 Position;

        protected Job(Vector3 position, float scale, Color color)
        {
            Position = position;
            Map = Map.Instance;
            var jobs = GameObject.Find("Jobs").transform;
            Marker = ObjectPool.Instance.GetObjectForType<JobMarker>(parent:jobs);
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
                        if (!IsInBounds((int) Position.x + dx, (int) Position.y + dy,(int) Position.z + dz))
                            continue;
                        if (Map.AStarNetwork.NodeGrid[(int) Position.x + dx, (int) Position.y + dy,(int) Position.z + dz] != null)
                        {
                            locs.Add(new Vector3((int)Position.x + dx, (int)Position.y + dy, (int)Position.z + dz));
                        }
                    }
                }
            }
            return locs;
        }

        private bool IsInBounds(int x, int y, int z)
        {
            return x >= 0 && y >= 0 && z >= 0 && x < Map.MapData.Size * Chunk.ChunkSize && y < Map.MapData.Height * Chunk.ChunkSize && z < Map.MapData.Size * Chunk.ChunkSize;
        }
    }

    public enum JobType
    {
        Mining
    }
}
