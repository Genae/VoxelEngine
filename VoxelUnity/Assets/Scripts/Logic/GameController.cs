using System.Collections.Generic;
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

        public GameObject flower1;
        public GameObject flower2;
        public GameObject shroom1;
        public GameObject shroom2;
        private List<GameObject> _ambientPlants;

        void Start()
        {
            Map.GenerateMap = true;
            _ambientPlants = new List<GameObject>();
            _ambientPlants.Add(flower1);
            _ambientPlants.Add(flower2);
            _ambientPlants.Add(shroom1);
            _ambientPlants.Add(shroom2);
        }

        void Update()
        {
            if (!_runOnce && Map.IsDoneGenerating)
            {
                _runOnce = true;
                for (int i = 0; i < 5; i++)
                {
                    SpawnCharacter();
                }
                //SpawnAmbientPlants(1000);
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

        private void SpawnAmbientPlants(int count)
        {
            var parent = transform.FindChild("Map").FindChild("AmbientFlowers");
            for(int i = 0; i < count; i++)
            {
                var pos = new Vector3(Random.Range(0, Map.MapData.Chunks.GetLength(0) * Chunk.ChunkSize), 1000, Random.Range(0, Map.MapData.Chunks.GetLength(0) * Chunk.ChunkSize));
                RaycastHit hit;
                Physics.Raycast(new Ray(pos, Vector3.down), out hit, float.PositiveInfinity);
                if (hit.collider.tag.Equals("Chunk"))
                {
                    var index = Random.Range(0, 4);
                    var start = hit.point;
                    var go = (GameObject)Instantiate(_ambientPlants[index], new Vector3(start.x + 0.5f, start.y, start.z + 0.5f), Quaternion.identity);
                    go.transform.parent = parent;
                   
                }
            }
        }
    }
}
