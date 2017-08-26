using Assets.Scripts.EngineLayer.Voxels.Containers;
using Assets.Scripts.EngineLayer.Voxels.Containers.Chunks;
using Assets.Scripts.MultiblockHandling;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class GameController : MonoBehaviour
    {
        public GameObject CharacterPreset;
        public GameObject BunnyPreset;
        public Map Map;
        

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
                        var c = Map.Instance.MapData.Chunks[(int)start.x / Chunk.ChunkSize, (int)start.y / Chunk.ChunkSize, (int)start.z / Chunk.ChunkSize];
                        if (c == null)
                            continue;
                        var mb = MultiblockLoader.LoadMultiblock("Plants/Ambient/" + ambientPlantConfiguration.Name, new Vector3(start.x - 0.5f, start.y, start.z - 0.5f), parent, 1);
                        c.RegisterSmallMultiblock(mb, new Vector3((int)start.x, (int)start.y, (int)start.z) - c.Position);
                    }
                }
            }
        }
    }
}
