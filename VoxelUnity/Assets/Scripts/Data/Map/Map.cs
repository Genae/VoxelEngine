using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Assets.Scripts.Algorithms;
using Assets.Scripts.Algorithms.MapGeneration;
using Assets.Scripts.Algorithms.Pathfinding.Graphs;
using Assets.Scripts.Control;
using Assets.Scripts.Data.Importer;
using Assets.Scripts.Data.Material;
using UnityEngine;

namespace Assets.Scripts.Data.Map
{
    public class Map : MonoBehaviour
    {
        public MapData MapData;
        public static Map Instance;
        public CameraController CameraController;
        public VoxelGraph AStarNetwork;
        public bool IsDoneGenerating;
        public bool GenerateMap; 

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
                if (chunkData == null)
                    continue;
                chunkData.CheckDirtyVoxels();
            }
        }

        // ReSharper disable once UnusedMember.Local
        IEnumerator CreateMap()
        {
            if (GenerateMap)
            {
                var biomeConfig = ConfigImporter.GetConfig<BiomeConfiguration>("Biomes").First();
                var hmg = new HeightmapGenerator();
                yield return hmg.CreateHeightMap(129, 129, 42);
                MapData = new MapData(hmg.Values.GetLength(0) / Chunk.ChunkSize, 100 / Chunk.ChunkSize, 2f);
                SetCameraValues();
                yield return MapData.LoadHeightmap(hmg.Values, hmg.BottomValues, hmg.CutPattern, 100);
                AStarNetwork = new VoxelGraph();
                yield return null;
                yield return InitializeMap();

                //Trees
                var treeManager = new TreeManager();
                yield return treeManager.GenerateTrees((int)(MapData.Chunks.GetLength(0) * MapData.Chunks.GetLength(0) * 0.3f), MapData);

                //Ressources
                var resourceManager = new ResourceManager();
                resourceManager.SpawnAllResources(MapData, biomeConfig.OreConfiguration);

                //RemoveTerrainNotOfType(new[] { MaterialRegistry.Instance.GetMaterialFromName("Iron"), MaterialRegistry.Instance.GetMaterialFromName("Gold"), MaterialRegistry.Instance.GetMaterialFromName("Copper"), MaterialRegistry.Instance.GetMaterialFromName("Coal") });
                //TestAStar();
            } else
            {
                MapData = new MapData(129 / Chunk.ChunkSize, 100 / Chunk.ChunkSize, 2f);
            }
            IsDoneGenerating = true;
        }

        private void SetCameraValues()
        {
            if (CameraController == null) return;
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
                        if (MapData.Chunks[x, y, z] == null)
                            continue;
                        Chunk.CreateChunk(x, y, z, this);
                        yield return null;
                    }
                }
            }
        }

        #region Tests
        private void RemoveTerrainNotOfType(VoxelMaterial[] types)
        {
            for (var x = 0; x < MapData.Chunks.GetLength(0) * Chunk.ChunkSize; x++)
            {
                for (var y = 0; y < MapData.Chunks.GetLength(1) * Chunk.ChunkSize; y++)
                {
                    for (var z = 0; z < MapData.Chunks.GetLength(0) * Chunk.ChunkSize; z++)
                    {
                        var chunk = MapData.Chunks[x / Chunk.ChunkSize, y / Chunk.ChunkSize, z / Chunk.ChunkSize];
                        if(chunk == null)
                            continue;
                        var mat = chunk.GetVoxelType(x % Chunk.ChunkSize, y % Chunk.ChunkSize, z % Chunk.ChunkSize);
                        if (!types.Contains(mat))
                            chunk.SetVoxelType(x % Chunk.ChunkSize, y % Chunk.ChunkSize, z % Chunk.ChunkSize, MaterialRegistry.Instance.GetMaterialFromName("Air"));
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

    public class BiomeConfiguration
    {
        public string Name;
        public ResourceConfiguration[] OreConfiguration;
    }

    public class ResourceConfiguration
    {
        public string ResourceType;
        public int MinAmount = 5;
        public int MaxAmount = 10;
        public float MinVeinLength = 30;
        public float VeinRadius = 2;
        public float Frequency = 7;
        
        private VoxelMaterial _material;
        public VoxelMaterial Material
        { 
            get { return _material ?? (_material = MaterialRegistry.Instance.GetMaterialFromName(ResourceType)); }
            set { _material = value; }
        }
    }
}