using System;
using System.Drawing;
using OpenTK;
using TestGame.MapGeneration;
using VoxelEngine;
using VoxelEngine.GameData;
using VoxelEngine.GUI;
using VoxelEngine.Light;

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

            //map
            Map = new Map(16, 16);
            var hmg = new HeightmapGenerator(Map.Chunks.GetLength(0) * Chunk.ChunkSize + 1, Map.Chunks.GetLength(0) * Chunk.ChunkSize + 1, 10);
            Map.LoadHeightmap(hmg.Values, hmg.BottomValues, hmg.CutPattern, (short)(Map.Chunks.GetLength(1) * Chunk.ChunkSize * 0.75));

            new DirectionalLight(new Vector3(0f, -1f, 1f));

            var ui = new AwsomUI("GUI/TestButton.html", new RelativePosition(RelativePosition.AnchorPoint.TopLeft, 100, 100));
            ui.BindCallback(new Callback("sayClick", false, (sender, args) =>
            {
                Console.WriteLine("Click :D");
            }));
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            CameraController.OnUpdateFrame(e);
        }
    }
}
