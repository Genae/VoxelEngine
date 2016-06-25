using Assets.Scripts.Algorithms.MapGeneration;
using Assets.Scripts.Control;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Data.Map
{
    public class Map : NetworkBehaviour
    {
        public MapData MapData;
        public Material[] Material;
        public CameraController CameraController;

        public override void OnStartServer()
        {
            var hmg = new HeightmapGenerator(129, 129, 1234);
            InitializeMap(MapData.LoadHeightmap(hmg.Values, hmg.BottomValues, hmg.CutPattern, 50, 50));
            CameraController.gameObject.transform.position = new Vector3(0,50,0);
        }

        public void InitializeMap(MapData data)
        {
            MapData = data;

            for (int x = 0; x < MapData.Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < MapData.Chunks.GetLength(1); y++)
                {
                    for (int z = 0; z < MapData.Chunks.GetLength(0); z++)
                    {
                        InitializeChunk(x, y, z);
                    }
                }
            }

            NetworkServer.Spawn(gameObject);
        }

        private void InitializeChunk(int x, int y, int z)
        {
            var chunk = new GameObject(string.Format("Chunk [{0}, {1}, {2}]", x, y, z));
            var chunkC = chunk.gameObject.AddComponent<Chunk>();
            var chunkMC = chunk.AddComponent<MeshCollider>();
            chunkC.InitializeChunk(new Vector3(x * Chunk.ChunkSize, y * Chunk.ChunkSize, z * Chunk.ChunkSize), MapData.Chunks[x, y, z], Material);
            chunkC.tag = "Chunk";
            chunkMC.sharedMesh = chunkC.ChunkMesh;
            chunk.transform.parent = transform;
        }
    }
}