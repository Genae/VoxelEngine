using System;
using OpenTK;
using TerrainGeneration.Algorithms;
using VoxelEngine;
using VoxelEngine.GameData;

namespace TestGame
{
    class Game : Engine
    {
        public GameCameraController CameraController;
        public Map Map;

        [STAThread]
        public static void Main()
        {
            using (var game = new Game())
            {
                Instance = game;
                game.Run(60);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Load stuff
            CameraController = new GameCameraController();
            Map = new Map(16, 4);
            var ds = new DiamondSquare(0.5f, Map.Chunks.GetLength(0)*Chunk.ChunkSize + 1, Map.Chunks.GetLength(2) * Chunk.ChunkSize + 1);
            Map.LoadHeightmap(ds.Generate(new Random()), (short)(Map.Chunks.GetLength(1) * Chunk.ChunkSize *0.75));
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            CameraController.OnUpdateFrame(e);
        }
    }
}
