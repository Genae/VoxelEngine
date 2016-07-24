using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Assets.Scripts.Algorithms;
using Assets.Scripts.Algorithms.MapGeneration;
using Assets.Scripts.Algorithms.Pathfinding;
using Assets.Scripts.Control;
using Assets.Scripts.Data.Material;
using UnityEngine;

namespace Assets.Scripts.Data.Map
{
    public class Map : MonoBehaviour
    {
        public MapData MapData;
        public static Map Instance;
        public MaterialRegistry MaterialRegistry;
        public CameraController CameraController;
        public AStarNetwork AStarNetwork;
        public bool IsDoneGenerating;
        

        public void Awake()
        {
            MainThread.Instantiate();
            Instance = this;
            StartCoroutine("CreateMap");
        }
        
        // ReSharper disable once UnusedMember.Local
        void Update()
        {
            if (!IsDoneGenerating)
                return;
            foreach (var chunkData in MapData.Chunks)
            {
                chunkData.CheckDirtyVoxels();
            }
        }

        // ReSharper disable once UnusedMember.Local
        IEnumerator CreateMap()
        {
            var hmg = new HeightmapGenerator();
            yield return hmg.CreateHeightMap(129, 129, 1337);
            MapData = new MapData(hmg.Values.GetLength(0) / Chunk.ChunkSize, 100 / Chunk.ChunkSize, 1f);
            SetCameraValues();
            yield return MapData.LoadHeightmap(hmg.Values, hmg.BottomValues, hmg.CutPattern, 100);
            AStarNetwork = new AStarNetwork(MapData.Chunks.GetLength(0) * Chunk.ChunkSize, MapData.Chunks.GetLength(1) * Chunk.ChunkSize, MapData.Chunks.GetLength(2) * Chunk.ChunkSize);
            yield return null;
            yield return InitializeMap();

            //Trees
            var treeManager = new TreeManager();
            yield return treeManager.GenerateTrees((int)(MapData.Chunks.GetLength(0)* MapData.Chunks.GetLength(0) * 0.3f), MapData);

            //Ressources
            var resourceManager = new ResourceManager();
            var weights = new Dictionary<VoxelMaterial, int>
            {
                {MaterialRegistry.Copper,7},
                {MaterialRegistry.Coal,7},
                {MaterialRegistry.Iron,5},
                {MaterialRegistry.Gold,3}
            };
            resourceManager.SpawnAllResources(MapData, weights);
            
            //RemoveTerrainNotOfType(new[] { MaterialRegistry.Iron, MaterialRegistry.Gold, MaterialRegistry.Copper, MaterialRegistry.Coal });
            //TestAStar();
            IsDoneGenerating = true;
        }

        private void SetCameraValues()
        {
            var mapSize = MapData.Chunks.GetLength(0) * Chunk.ChunkSize;
            var mapHeight = MapData.Chunks.GetLength(1) * Chunk.ChunkSize;
            CameraController.RightLimit = mapSize * 1.1f;
            CameraController.TopLimit = mapSize * 1.1f;
            CameraController.CameraMinHeight = mapHeight * 0.5f;
            CameraController.CameraMaxHeight = mapHeight * 1.5f;

            CameraController.gameObject.transform.position = new Vector3(0, mapHeight, 0);
            CameraController.RotateTo(55);
            CameraController.Eye.gameObject.transform.position = new Vector3(0, CameraController.CameraMaxHeight, 0);
        }

        private IEnumerator InitializeMap()
        {
            for (var x = 0; x < MapData.Chunks.GetLength(0); x++)
            {
                for (var z = 0; z < MapData.Chunks.GetLength(0); z++)
                {
                    for (var y = MapData.Chunks.GetLength(1)-1; y >= 0 ; y--)
                    {
                        Chunk.CreateChunk(x, y, z, this);
                        yield return null;
                    }
                }
            }
        }

        #region Tests
        private void TestAStar()
        {
            foreach (var chunk in gameObject.GetComponentsInChildren<Chunk>())
            {
                chunk.Update();
            }
            var allNodes = new List<Node>();
            for (var x = 0; x < MapData.Chunks.GetLength(0); x++)
            {
                for (var y = 0; y < MapData.Chunks.GetLength(1); y++)
                {
                    for (var z = 0; z < MapData.Chunks.GetLength(0); z++)
                    {
                        allNodes.AddRange(MapData.Chunks[x, y, z].LocalAStar.Nodes);
                        //MapData.Chunks[x, y, z].LocalAStar.Visualize();
                        if (MapData.Chunks[x, y, z].LocalAStar.Nodes.Count > 0)
                        {

                        }
                    }
                }
            }
            var amount = 0;
            while (amount < 5)
            {
                var start = allNodes[Random.Range(0, allNodes.Count)];
                var end = allNodes[Random.Range(0, allNodes.Count)];
                var path = Path.Calculate(MapData, start.Position, end.Position);
                var color = new[] { Color.green, Color.blue, Color.black, Color.magenta, Color.yellow }[amount];
                path.OnFinish += () => path.Visualize(color);
                amount++;
            }
        }
        private void RemoveTerrainNotOfType(VoxelMaterial[] types)
        {
            for (var x = 0; x < MapData.Chunks.GetLength(0) * Chunk.ChunkSize; x++)
            {
                for (var y = 0; y < MapData.Chunks.GetLength(1) * Chunk.ChunkSize; y++)
                {
                    for (var z = 0; z < MapData.Chunks.GetLength(0) * Chunk.ChunkSize; z++)
                    {
                        var chunk = MapData.Chunks[x / Chunk.ChunkSize, y / Chunk.ChunkSize, z / Chunk.ChunkSize];
                        var mat = chunk.GetVoxelType(x % Chunk.ChunkSize, y % Chunk.ChunkSize, z % Chunk.ChunkSize);
                        if (!types.Contains(mat))
                            chunk.SetVoxelType(x % Chunk.ChunkSize, y % Chunk.ChunkSize, z % Chunk.ChunkSize, MaterialRegistry.Air);
                    }
                }
            }
        }
        #endregion

        public bool IsInBounds(int x, int y, int z)
        {
            return x >= 0 && y >= 0 && z >= 0 && x < MapData.Size * Chunk.ChunkSize && y < MapData.Height * Chunk.ChunkSize && z < MapData.Size * Chunk.ChunkSize;
        }
    }
}