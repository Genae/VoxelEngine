using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Algorithms;
using UnityEngine;

namespace Assets.Scripts.Data.Map
{
    public class Chunk : VoxelContainer
    {
        public const int ChunkSize = 16;
        
        public static GameObject CreateChunk(int x, int y, int z, Map map)
        {
            var chunk = new GameObject(string.Format("Chunk [{0}, {1}, {2}]", x, y, z));
            var chunkC = chunk.gameObject.AddComponent<Chunk>();
            chunkC.InitializeContainer(new Vector3(x * ChunkSize, y * ChunkSize, z * ChunkSize), map.MapData.Chunks[x, y, z], map.MaterialRegistry.Materials);
            chunkC.tag = "Chunk";
            chunk.transform.parent = map.transform;
            return chunk;
        }
    }

    public class VoxelContainer : MonoBehaviour
    {
        public Mesh Mesh;
        private UnityEngine.Material[] _materials;
        protected bool MeshNeedsUpdate;
        public ContainerData ContainerData;

        public static GameObject CreateContainer(Vector3 pos, ContainerData data, Map map, string name = null)
        {
            var container = new GameObject(string.Format(name!=null?name:"Container" + "[{0}, {1}, {2}]", pos.x, pos.y, pos.z));
            var containerC = container.gameObject.AddComponent<VoxelContainer>();
            containerC.InitializeContainer(pos, data, map.MaterialRegistry.Materials);
            containerC.tag = "Container";
            container.transform.parent = map.transform;
            return container;
        }

        void Update()
        {
            if (MeshNeedsUpdate)
            {
                UpdateMesh();
                MeshNeedsUpdate = false;
            }
        }

        public void OnContainerUpdated()
        {
            MeshNeedsUpdate = true;
        }

        protected void InitializeContainer(Vector3 pos, ContainerData data, UnityEngine.Material[] mats)
        {
            ContainerData = data;
            gameObject.transform.position = pos;
            _materials = mats;

            data.ContainerUpdated += OnContainerUpdated;

            Mesh = GetComponent<MeshFilter>() == null ? gameObject.AddComponent<MeshFilter>().mesh : GetComponent<MeshFilter>().mesh;
            if (gameObject.GetComponent<MeshRenderer>() == null)
            {
                gameObject.AddComponent<MeshRenderer>();
            }
            OnContainerUpdated();
            Update();
        }

        private void UpdateMesh()
        {
            Dictionary<int, int[]> triangles;
            Vector3[] vertices;
            Vector3[] normals;
            Vector2[] uvs;
            GreedyMeshing.CreateMesh(out vertices, out triangles, out normals, out uvs, ContainerData, ContainerData.Size);

            Mesh.Clear();
            Mesh.vertices = vertices;
            Mesh.normals = normals;
            Mesh.subMeshCount = triangles.Keys.Count;
            Mesh.uv = uvs;

            var keyArray = triangles.Keys.ToArray();
            var myMats = new UnityEngine.Material[triangles.Keys.Count];
            for (var i = 0; i < triangles.Keys.Count; i++)
            {
                Mesh.SetTriangles(triangles[keyArray[i]], i);
                myMats[i] = _materials[keyArray[i]];
            }
            gameObject.GetComponent<MeshRenderer>().sharedMaterials = myMats;
            GetComponent<MeshFilter>().mesh = Mesh;
            var mCollider = GetComponent<MeshCollider>() != null ? GetComponent<MeshCollider>() : gameObject.AddComponent<MeshCollider>();
            mCollider.sharedMesh = Mesh;
        }
    }
}
