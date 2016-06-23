using Assets.Scripts.Algorithms;
using UnityEngine;

namespace Assets.Scripts.Data.Map
{
    public class Chunk : MonoBehaviour
    {
        public const int ChunkSize = 16;
        public Mesh ChunkMesh;
        public ChunkData ChunkData;
        public float Scale = 1f;

        public void InitializeChunk(Vector3 pos, ChunkData data, Material mat)
        {
            ChunkData = data;
            gameObject.transform.position = pos;
            data.ChunkUpdated += chunkData =>
            {
                OnChunkUpdated();
            };
            ChunkMesh = GetComponent<MeshFilter>() == null ? gameObject.AddComponent<MeshFilter>().mesh : GetComponent<MeshFilter>().mesh;
            if (gameObject.GetComponent<MeshRenderer>() == null)
            {
                gameObject.AddComponent<MeshRenderer>();
            }
            GetComponent<MeshRenderer>().sharedMaterial = mat;
            OnChunkUpdated();
        }


        protected void Load()
        {
            OnChunkUpdated();
        }

        public void OnChunkUpdated()
        {
            int[] triangles;
            Vector3[] vertices;
            Color[] colors;
            Vector3[] normals;
            GreedyMeshing.CreateMesh(out vertices, out triangles, out colors, out normals, ChunkData.Voxels, ChunkData.NeighbourBorders, gameObject.transform.position, Scale);
            
            ChunkMesh.Clear();
            ChunkMesh.vertices = vertices;
            ChunkMesh.colors = colors;
            ChunkMesh.triangles = triangles;
            ChunkMesh.normals = normals;
        }
    }
}
