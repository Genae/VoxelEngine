using System;
using OpenTK;
using VoxelEngine.Client;
using VoxelEngine.Client.GameData;
using VoxelEngine.Client.GUI;
using VoxelEngine.Client.Light;

namespace TestGame
{
    class Game : EngineClient
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
            /*Map = Map.CreateEmpty(mapsize, mapheight);

            Map.Chunks[0, 0, 0].Voxels[0, 0, 0].IsActive = true;
            Map.Chunks[0, 0, 0].Voxels[15, 15, 15].IsActive = true;

            Map.Chunks[0,0,0].Voxels[0, 1, 0].IsActive = true;
            Map.Chunks[0, 0, 0].Voxels[0, 1, 1].IsActive = true;

            Map.Chunks[0, 0, 0].Voxels[0, 2, 2].IsActive = true;
            Map.Chunks[0, 0, 0].Voxels[0, 2, 3].IsActive = true;
            Map.Chunks[0, 0, 0].Voxels[0, 3, 2].IsActive = true;
            Map.Chunks[0, 0, 0].Voxels[0, 3, 3].IsActive = true;
            Map.Chunks[0, 0, 0].Voxels[1, 2, 2].IsActive = true;
            Map.Chunks[0, 0, 0].Voxels[1, 2, 3].IsActive = true;
            Map.Chunks[0, 0, 0].Voxels[1, 3, 2].IsActive = true;
            Map.Chunks[0, 0, 0].Voxels[1, 3, 3].IsActive = true;

            Map.Chunks[0, 0, 0].Voxels[3, 2, 2].IsActive = true;
            Map.Chunks[0, 0, 0].Voxels[3, 2, 3].IsActive = true;
            Map.Chunks[0, 0, 0].Voxels[3, 3, 2].IsActive = true;
            Map.Chunks[0, 0, 0].Voxels[3, 3, 3].IsActive = true;
            Map.Chunks[0,0,0].OnChunkUpdated();*/

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
