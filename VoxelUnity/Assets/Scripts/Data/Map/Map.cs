using System.Collections.Generic;
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
        public MaterialRegistry MaterialRegistry;
        public CameraController CameraController;
        public AStarNetwork AStarNetwork;
        

        public void Awake()
        {
            var hmg = new HeightmapGenerator(129, 129, 1337);
            MapData = MapData.LoadHeightmap(hmg.Values, hmg.BottomValues, hmg.CutPattern, 100, 100, 1);
            AStarNetwork = new AStarNetwork(MapData.Chunks.GetLength(0) * Chunk.ChunkSize, MapData.Chunks.GetLength(1) * Chunk.ChunkSize, MapData.Chunks.GetLength(2) * Chunk.ChunkSize);
            InitializeMap(MapData);

            var mapSize = MapData.Chunks.GetLength(0)*Chunk.ChunkSize;
            var mapHeight = MapData.Chunks.GetLength(1)*Chunk.ChunkSize;
            CameraController.RightLimit = mapSize*1.1f;
            CameraController.TopLimit = mapSize*1.1f;
            CameraController.CameraMinHeight = mapHeight*0.5f;
            CameraController.CameraMaxHeight = mapHeight * 1.5f;

            CameraController.gameObject.transform.position = new Vector3(0, mapHeight, 0);

            //Trees
            var treeManager = new TreeManager();
            treeManager.GenerateTrees(10, MapData);

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
            //Remove all Terrain not of type t
            /*for (var x = 0; x < MapData.Chunks.GetLength(0) * Chunk.ChunkSize; x++)
            {
                for (var y = 0; y < MapData.Chunks.GetLength(1) * Chunk.ChunkSize; y++)
                {
                    for (var z = 0; z < MapData.Chunks.GetLength(0) * Chunk.ChunkSize; z++)
                    {
                        var chunk = MapData.Chunks[x/Chunk.ChunkSize, y/Chunk.ChunkSize, z/Chunk.ChunkSize];
                        var mat = chunk.GetVoxelType(x%Chunk.ChunkSize, y%Chunk.ChunkSize, z%Chunk.ChunkSize);
                        if (!new[] {MaterialRegistry.Iron, MaterialRegistry.Gold, MaterialRegistry.Copper, MaterialRegistry.Coal}.Contains(mat))
                            chunk.SetVoxelType(x%Chunk.ChunkSize, y%Chunk.ChunkSize, z%Chunk.ChunkSize,MaterialRegistry.Air);
                    }
                }
            }*/

            //test ASTar
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
                var path = AStar.GetPath(MapData, start.Position, end.Position);
                if (path == null)
                    continue;
                path.Visualize(new[] { Color.green, Color.blue, Color.black, Color.magenta, Color.yellow }[amount]);
                amount++;
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
                        Chunk.CreateChunk(x, y, z, this);
                    }
                }
            }
        }

        void Update()
        {
            foreach (var chunkData in MapData.Chunks)
            {
                chunkData.CheckDirtyVoxels();
            }
        }
    }
}