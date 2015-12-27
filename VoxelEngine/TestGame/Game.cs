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
            CameraController.Camera.CameraPos = new Vector3(-10,-10,-10);

            //map
            int mapsize = 16 * Chunk.ChunkSize, mapheight = 16 * Chunk.ChunkSize;
            var hmg = new HeightmapGenerator(mapsize + 1, mapsize + 1, 10);
            Map = Map.LoadHeightmap(hmg.Values, hmg.BottomValues, hmg.CutPattern, (short)mapheight, mapheight*0.75f);

            new DirectionalLight(new Vector3(-10, -10, -10));

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
