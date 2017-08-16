﻿using System.Collections;
using System.Linq;
using Assets.Scripts.Algorithms;
using Assets.Scripts.Data.Importer;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Logic;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class GameLoader : MonoBehaviour
    {
        public static bool GameLoaded = false;
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
            MaterialRegistry.Instance.Preload();
            yield return null;

            //Load Map
            SetStatus("Loading Map", 0.02f);
            var biomeConfig = ConfigImporter.GetConfig<BiomeConfiguration>("Biomes").First();
            yield return Map.Map.Instance.CreateMap(biomeConfig, this);

            //Trees
            SetStatus("Loading Trees", 0.8f);
            var treeManager = new TreeManager();
            yield return treeManager.GenerateTrees((int)(Map.Map.Instance.MapData.Chunks.GetLength(0) * Map.Map.Instance.MapData.Chunks.GetLength(0) * 0.3f), Map.Map.Instance.MapData, this);

            //Ressources
            SetStatus("Loading Ressources", 0.95f);
            var resourceManager = new ResourceManager();
            resourceManager.SpawnAllResources(Map.Map.Instance.MapData, biomeConfig.OreConfiguration);

            //Characters
            SetStatus("Loading Characters", 0.97f);
            var gameController = FindObjectOfType<GameController>();
            for (int i = 0; i < 5; i++)
            {
                gameController.SpawnCharacter();
            }

            //AmbientPlants
            SetStatus("Loading Ambient Plants", 0.99f);
            gameController.SpawnAmbientPlants(biomeConfig);

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
            var cameraController = Map.Map.Instance.CameraController;
            var chunks = Map.Map.Instance.MapData.Chunks;
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