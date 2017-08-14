using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Map;
using Assets.Scripts.MultiblockHandling;
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
            _ambientPlants = new List<GameObject>();
            _ambientPlants.Add(flower1);
            _ambientPlants.Add(flower2);
            _ambientPlants.Add(shroom1);
            _ambientPlants.Add(shroom2);
        }

        void Update()
        {
        }

        public void SpawnCharacter()
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

        public void SpawnAmbientPlants(BiomeConfiguration biomeConfig)
        {
            var parent = transform.Find("Map").Find("AmbientFlowers");
            foreach (var ambientPlantConfiguration in biomeConfig.AmbientPlants)
            {
                for (int i = 0; i < ambientPlantConfiguration.Amount; i++)
                {
                    var pos = new Vector3(Random.Range(0, Map.MapData.Chunks.GetLength(0) * Chunk.ChunkSize), 1000, Random.Range(0, Map.MapData.Chunks.GetLength(0) * Chunk.ChunkSize));
                    RaycastHit hit;
                    Physics.Raycast(new Ray(pos, Vector3.down), out hit, float.PositiveInfinity);
                    if (hit.collider.tag.Equals("Chunk"))
                    {
                        var start = hit.point;
                        MultiblockLoader.LoadMultiblock("Plants/Ambient/" + ambientPlantConfiguration.Name, new Vector3(start.x + 0.5f, start.y, start.z + 0.5f), parent);
                    }
                }
            }
        }
    }
}
