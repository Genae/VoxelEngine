using System.Collections.Generic;
using Assets.Scripts.Data.Map;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Logic.Jobs
{
    public abstract class Job : MonoBehaviour
    {
        protected Map Map;

        public abstract JobType GetJobType();

        public abstract bool Solve(float deltaTime);

        void Start()
        {
            Map = GameObject.Find("Map").GetComponent<Map>();
        }

        public List<Vector3> GetPossibleWorkLocations()
        {
            var locs = new List<Vector3>();
            for (var dx = -1; dx < 1; dx++)
            {
                for (var dy = -2; dy < 1; dy++)
                {
                    for (var dz = -1; dz < 1; dz++)
                    {
                        if(dx == 0 && dy == 1 && dz == 0)
                            continue; //never dig straight down
                        if (!IsInBounds((int) transform.position.x + dx, (int) transform.position.y + dy,(int) transform.position.z + dz))
                            continue;
                        if (Map.AStarNetwork.NodeGrid[(int) transform.position.x + dx, (int) transform.position.y + dy,(int) transform.position.z + dz] != null)
                        {
                            locs.Add(new Vector3((int)transform.position.x + dx, (int)transform.position.y + dy, (int)transform.position.z + dz));
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
