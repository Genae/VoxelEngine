using Assets.Scripts.Algorithms.MapGeneration;
using Assets.Scripts.Control;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Data.Multiblock;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Data.Map
{
    public class Map : NetworkBehaviour
    {
        public MapData MapData;
        public MaterialRegistry MaterialRegistry;
        public CameraController CameraController;

        public TreeManager TreeManager;

        public override void OnStartServer()
        {
            var hmg = new HeightmapGenerator(129, 129, 1234);
            InitializeMap(MapData.LoadHeightmap(hmg.Values, hmg.BottomValues, hmg.CutPattern, 100, 100, 2));
            var mapSize = MapData.Chunks.GetLength(0)*Chunk.ChunkSize;
            var mapHeight = MapData.Chunks.GetLength(1)*Chunk.ChunkSize;
            CameraController.RightLimit = mapSize*1.1f;
            CameraController.TopLimit = mapSize*1.1f;
            CameraController.CameraMinHeight = mapHeight*0.5f;
            CameraController.CameraMaxHeight = mapHeight * 1.5f;

            CameraController.gameObject.transform.position = new Vector3(0, mapHeight, 0);

            TreeManager = new TreeManager();
            for(int i = 0; i < 100; i++)
            {
                var index = (int)Random.Range(0f, MapData.PossibleTreePositions.Count);
                TreeManager.GenerateTree(MapData.PossibleTreePositions[index], MapData);
            }
        }

        public void InitializeMap(MapData data)
        {
            MapData = data;

            for (var x = 0; x < MapData.Chunks.GetLength(0); x++)
            {
                for (var y = 0; y < MapData.Chunks.GetLength(1); y++)
                {
                    for (var z = 0; z < MapData.Chunks.GetLength(0); z++)
                    {
                        InitializeChunk(x, y, z);
                    }
                }
            }

            NetworkServer.Spawn(gameObject);
        }

        void Update()
        {
            foreach (var chunkData in MapData.Chunks)
            {
                chunkData.CheckDirtyVoxels();
            }
        }

        private void InitializeChunk(int x, int y, int z)
        {
            var chunk = new GameObject(string.Format("Chunk [{0}, {1}, {2}]", x, y, z));
            var chunkC = chunk.gameObject.AddComponent<Chunk>();
            chunkC.InitializeChunk(new Vector3(x * Chunk.ChunkSize, y * Chunk.ChunkSize, z * Chunk.ChunkSize), MapData.Chunks[x, y, z], MaterialRegistry.GetMaterials());
            chunkC.tag = "Chunk";
            chunk.transform.parent = transform;
        }
    }
}