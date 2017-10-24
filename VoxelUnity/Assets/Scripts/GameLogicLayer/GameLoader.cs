using System.Collections;
using System.Linq;
using Assets.Scripts.AccessLayer;
using Assets.Scripts.AccessLayer.Material;
using Assets.Scripts.AccessLayer.Worlds;
using Assets.Scripts.Algorithms;
using Assets.Scripts.EngineLayer;
using Assets.Scripts.EngineLayer.Voxels.Containers;
using Assets.Scripts.EngineLayer.Voxels.Containers.Chunks;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayer
{
    public class GameLoader : MonoBehaviour
    {
        public static bool GameLoaded = false;
        public GameObject Bunny;
        private LoadingScreen _loadingScreen;

        void Awake()
        {
            MainThread.Instantiate();
            StartCoroutine(LoadGame());
        }
        
        IEnumerator LoadGame()
        {
            Time.timeScale = 0;
            yield return null;
            _loadingScreen = FindObjectOfType<LoadingScreen>();
            _loadingScreen.IsVisible.Value = true;

            //Load Materials
            SetStatus("Loading Materials", 0.01f);
            yield return null;
            MaterialRegistry.Instance.Preload();
            yield return null;

            //Load Map
            SetStatus("Loading Map", 0.02f);
            yield return null;
            var biomeConfig = ConfigImporter.GetAllConfigs<BiomeConfiguration>("World/Biomes").First();
            yield return Map.Instance.CreateMap(biomeConfig, this);

            //Trees
            SetStatus("Loading Tree of Life", 0.8f);
            yield return null;
            var treeManager = new TreeManager();
            //treeManager.BuildTreeOfLife();
            SetStatus("Loading Trees", 0.85f);
            yield return null;
            yield return treeManager.GenerateTrees((int)(Map.Instance.MapData.Chunks.GetLength(0) * Map.Instance.MapData.Chunks.GetLength(0) * 0.3f), Map.Instance.MapData, this);

            //Ressources
            SetStatus("Loading Ressources", 0.95f);
            yield return null;
            var resourceManager = new ResourceManager();
            resourceManager.SpawnAllResources(Map.Instance.MapData, biomeConfig.OreConfiguration);
            yield return null;

            //Characters
            SetStatus("Loading Characters", 0.96f);
            yield return null;
            for (int i = 0; i < 5; i++)
            {
                UnitManager.SpawnUnitAtRandomPosition(Bunny);
            }

            //AmbientPlants
            SetStatus("Loading Ambient Plants", 0.97f);
            yield return null;
            AmbientManager.SpawnAmbientPlants(biomeConfig);

            //AddT1Nodes
            SetStatus("Finish Pathfinder", 0.98f);
            yield return null;
            Map.Instance.AStarNetwork.AddTier1Nodes(20);

            SetStatus("Done", 1f);
            //Start Time Again
            Time.timeScale = 1;
            yield return null;
            GameLoaded = true;

            _loadingScreen.IsVisible.Value = false;
            SetCameraValues();
        }

        public void SetStatus(string text, float progress)
        {
            _loadingScreen.StatusText.Value = text;
            _loadingScreen.Progress.Value = progress;
        }


        private void SetCameraValues()
        {
            var cameraController = Map.Instance.CameraController;
            var chunks = Map.Instance.MapData.Chunks;
            if (cameraController == null) return;
            var mapSize = chunks.GetLength(0) * Chunk.ChunkSize;
            var mapHeight = chunks.GetLength(1) * Chunk.ChunkSize;
            cameraController.RightLimit = mapSize * 1.1f;
            cameraController.TopLimit = mapSize * 1.1f;
            cameraController.CameraMinHeight = mapHeight * 0.5f;
            cameraController.CameraMaxHeight = mapHeight * 1.5f;

            cameraController.gameObject.transform.position = new Vector3(0, mapHeight, 0);
            cameraController.RotateTo(55);
            cameraController.Eye.gameObject.transform.position = new Vector3(0, cameraController.CameraMaxHeight, 0);
        }
    }
}
