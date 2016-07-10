using Assets.Scripts.Algorithms.Pathfinding;
using Assets.Scripts.Data.Map;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class GameController : MonoBehaviour
    {
        public GameObject CharacterPreset;
        public GameObject BunnyPreset;
        public Map Map;

        void Start()
        {
            SpawnCharacter();
        }

        private void SpawnCharacter()
        {
            Vector3 start;
            Vector3 target;
            while (true)
            {
                var pos = new Vector3(Random.Range(0, Map.MapData.Chunks.GetLength(0) * Chunk.ChunkSize), 1000, Random.Range(0, Map.MapData.Chunks.GetLength(0) * Chunk.ChunkSize));
                RaycastHit hit;
                Physics.Raycast(new Ray(pos, Vector3.down), out hit, float.PositiveInfinity);
                if (hit.collider.tag.Equals("Chunk"))
                {
                    start = hit.point;
                    break;
                }
            }
            while (true)
            {
                var pos = new Vector3(Random.Range(0, Map.MapData.Chunks.GetLength(0) * Chunk.ChunkSize), 1000, Random.Range(0, Map.MapData.Chunks.GetLength(0) * Chunk.ChunkSize));
                RaycastHit hit;
                Physics.Raycast(new Ray(pos, Vector3.down), out hit, float.PositiveInfinity);
                if (hit.collider.tag.Equals("Chunk"))
                {
                    target = hit.point;
                    break;
                }
            }
            var path = Path.Calculate(Map.MapData, start, target);
            if (path == null)
            {
                SpawnCharacter();
            }
            else
            {
                var bunny = (GameObject)Instantiate(BunnyPreset, start, Quaternion.identity);
                var wc = bunny.GetComponent<WalkingController>();
                wc.PathToTarget = path;
            }
        }
    }
}
