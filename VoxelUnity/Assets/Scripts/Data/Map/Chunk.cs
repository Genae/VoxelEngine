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
        
        protected override List<Vector3> UpdateMesh()
        {
            var up = base.UpdateMesh();
            var cd = (ChunkData) ContainerData;
            cd.LocalAStar.RefreshNetwork(cd, up);
            return up;
        }
    }

    public class VoxelContainer : MonoBehaviour
    {
        public Mesh Mesh;
        private UnityEngine.Material[] _materials;
        protected bool MeshNeedsUpdate;
        public ContainerData ContainerData;

        public static VoxelContainer CreateContainer<T>(Vector3 pos, ContainerData data, UnityEngine.Material[] materials, string name = null) where T : VoxelContainer
        {
            var container = new GameObject(string.Format(name!=null?name:"Container" + "[{0}, {1}, {2}]", pos.x, pos.y, pos.z));
            var containerC = container.gameObject.AddComponent<T>();
            containerC.InitializeContainer(pos, data, materials);
            containerC.tag = "Container";
            return containerC;
        }

        public void Update()
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

        protected virtual List<Vector3> UpdateMesh()
        {
            Dictionary<int, int[]> triangles;
            Vector3[] vertices;
            Vector3[] normals;
            Vector2[] uvs;
            List<Vector3> upVoxels;
            GreedyMeshing.CreateMesh(out vertices, out triangles, out normals, out uvs, out upVoxels, ContainerData, ContainerData.Size);

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
            GetComponent<MeshFilter>().sharedMesh = Mesh;
            var mCollider = GetComponent<MeshCollider>() != null ? GetComponent<MeshCollider>() : gameObject.AddComponent<MeshCollider>();
            mCollider.sharedMesh = Mesh;
            return upVoxels;
        }
    }
}
