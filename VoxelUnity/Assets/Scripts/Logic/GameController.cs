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
        private bool _runOnce;

        void Update()
        {
            if (!_runOnce && Map.IsDoneGenerating)
            {
                _runOnce = true;
                for (int i = 0; i < 5; i++)
                {
                    SpawnCharacter();
                }
            }
        }

        private void SpawnCharacter()
        {
            while (true)
            {
                var pos = new Vector3(Random.Range(0, Map.MapData.Chunks.GetLength(0) * Chunk.ChunkSize), 1000, Random.Range(0, Map.MapData.Chunks.GetLength(0) * Chunk.ChunkSize));
                RaycastHit hit;
                Physics.Raycast(new Ray(pos, Vector3.down), out hit, float.PositiveInfinity);
                if (hit.collider.tag.Equals("Chunk"))
                {
                    var start = hit.point;
                    Instantiate(BunnyPreset, start, Quaternion.identity);
                    break;
                }
            }
        }
    }
}
