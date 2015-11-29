using System;
using TerrainGeneration.Algorithms;
using VoxelEngine;
using VoxelEngine.GameData;

namespace TestGame
{
    class Game : Engine
    {
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
            var ds = new DiamondSquare(0.5f, Map.Chunks.GetLength(0)*Chunk.ChunkSize + 1, Map.Chunks.GetLength(2) * Chunk.ChunkSize + 1);
            Map.LoadHeightmap(ds.Generate(new Random(0)), (short)(Map.Chunks.GetLength(1) * Chunk.ChunkSize *0.75));
        }
    }
}
