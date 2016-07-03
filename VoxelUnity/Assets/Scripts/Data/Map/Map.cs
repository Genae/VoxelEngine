using System.Collections.Generic;
using System.Linq;
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
        

        public void Start()
        {
            var hmg = new HeightmapGenerator(129, 129, 1337);
            var mapData = MapData.LoadHeightmap(hmg.Values, hmg.BottomValues, hmg.CutPattern, 100, 100, 1);
            InitializeMap(mapData);


            var mapSize = MapData.Chunks.GetLength(0)*Chunk.ChunkSize;
            var mapHeight = MapData.Chunks.GetLength(1)*Chunk.ChunkSize;
            CameraController.RightLimit = mapSize*1.1f;
            CameraController.TopLimit = mapSize*1.1f;
            CameraController.CameraMinHeight = mapHeight*0.5f;
            CameraController.CameraMaxHeight = mapHeight * 1.5f;

            CameraController.gameObject.transform.position = new Vector3(0, mapHeight, 0);

            //Trees
            var treeManager = new TreeManager();
            treeManager.GenerateTrees(0, MapData);

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
            var allNodes = new List<Node>();
            for (var x = 0; x < MapData.Chunks.GetLength(0); x++)
            {
                for (var y = 0; y < MapData.Chunks.GetLength(1); y++)
                {
                    for (var z = 0; z < MapData.Chunks.GetLength(0); z++)
                    {
                        MapData.Chunks[x, y, z].AStar.ConnectNetworkToNeighbours(MapData.Chunks[x, y, z]);
                        if (MapData.Chunks[x, y, z].AStar.Nodes.Count > 0)
                        {
                            allNodes.AddRange(MapData.Chunks[x,y,z].AStar.Nodes);
                        }
                    }
                }
            }
            //test ASTar
            var amount = 0;
            while (amount < 5)
            {
                var start = allNodes[Random.Range(0, allNodes.Count)];
                var end = allNodes[Random.Range(0, allNodes.Count)];
                var path = AStar.GetPath(MapData, start.Position, end.Position);
                if (path == null)
                    continue;
                for (int i = 0; i < path.Nodes.Count - 1; i++)
                {
                    Debug.DrawLine(path.Nodes[i].Position, path.Nodes[i + 1].Position, new[] {Color.green, Color.blue, Color.black, Color.magenta, Color.yellow}[amount], 60000, true);
                }
                amount++;
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