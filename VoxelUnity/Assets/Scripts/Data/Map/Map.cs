using System;
using Assets.Scripts.Algorithms.MapGeneration;
using Assets.Scripts.Control;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Data.Multiblock.Trees;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Data.Map
{
    public class Map : MonoBehaviour
    {
        public MapData MapData;
        public MaterialRegistry MaterialRegistry;
        public CameraController CameraController;

        public TreeManager TreeManager;

        public void Start()
        {
            var hmg = new HeightmapGenerator(129, 129, 1337);
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
                var pos = new Vector3(Random.Range(0, MapData.Chunks.GetLength(0)*Chunk.ChunkSize), 1000, Random.Range(0, MapData.Chunks.GetLength(0) * Chunk.ChunkSize));
                RaycastHit hit;
                Physics.Raycast(new Ray(pos, Vector3.down), out hit, float.PositiveInfinity);
                if (hit.collider.tag.Equals("Chunk"))
                {
                    TreeManager.GenerateTree(hit.point);
                }
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