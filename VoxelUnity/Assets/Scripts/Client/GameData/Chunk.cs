using Assets.Scripts.Base.Algorithms;
using Assets.Scripts.Base.Data.Map;
using UnityEngine;

namespace Assets.Scripts.Client.GameData
{
    public class Chunk : Mesh
    {
        public const int ChunkSize = 16;
        public Mesh ChunkBorders;
        public ChunkData ChunkData;

        public Chunk(Vector3 pos, ChunkData data):base(ChunkSize, pos)
        {
            ChunkData = data;
            data.ChunkUpdated += chunkData =>
            {
                OnChunkUpdated();
            };

            Shader = DirectionalDiffuse.Instance;
            ChunkBorders = new ChunkBorder(ChunkSize, pos) {Shader = Shader};
        }


        protected override void Load()
        {
            OnChunkUpdated();
        }

        public void OnChunkUpdated()
        {
            ushort[] triangles;
            float[] vertecies;
            float[] colors;
            float[] normals;
            GreedyMeshing.CreateMesh(out vertecies, out triangles, out colors, out normals, ChunkData.Voxels, ChunkData.NeighbourBorders, Pos, Scale);
            ChunkBorders.SetActive(triangles.Length != 0);

            CreateMesh(vertecies, triangles, colors, normals);
        }
    }
}
