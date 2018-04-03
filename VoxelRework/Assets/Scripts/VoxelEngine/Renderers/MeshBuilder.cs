using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Scripts.VoxelEngine.Containers.Chunks;
using Assets.Scripts.VoxelEngine.Materials;
using UnityEngine;

namespace Assets.Scripts.VoxelEngine.Renderers
{
    public class MeshBuilder : MonoBehaviour
    {
        private Mesh _mesh;
        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;
        private MeshFilter _meshFilter;

        //sliced top
        private GameObject _sliced;
        private Mesh _slicedMesh;
        private MeshRenderer _slicedRenderer;
        private MeshCollider _slicedCollider;
        private MeshFilter _slicedFilter;

        //cache Mesh
        private MeshData _meshData;
        private MeshData _meshDataSlice;

        public void Init()
        {
            _meshRenderer = gameObject.GetComponent<MeshRenderer>() != null ? gameObject.GetComponent<MeshRenderer>() : gameObject.AddComponent<MeshRenderer>();
            _meshCollider = gameObject.GetComponent<MeshCollider>() != null ? gameObject.GetComponent<MeshCollider>() : gameObject.AddComponent<MeshCollider>();
            _meshFilter = gameObject.GetComponent<MeshFilter>() != null ? gameObject.GetComponent<MeshFilter>() : gameObject.AddComponent<MeshFilter>();

            if (_sliced == null)
            {
                _sliced = new GameObject(gameObject.name + " slice");
                _sliced.SetActive(false);
                _sliced.transform.parent = transform.parent;
                _sliced.transform.localPosition = transform.position - (Vector3.up * 0.1f);
                _slicedRenderer = _sliced.GetComponent<MeshRenderer>() != null ? _sliced.GetComponent<MeshRenderer>() : _sliced.AddComponent<MeshRenderer>();
                _slicedCollider = _sliced.GetComponent<MeshCollider>() != null ? _sliced.GetComponent<MeshCollider>() : _sliced.AddComponent<MeshCollider>();
                _slicedFilter = _sliced.GetComponent<MeshFilter>() != null ? _sliced.GetComponent<MeshFilter>() : _sliced.AddComponent<MeshFilter>();
                //_slicedRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
        }
        
        public void BuildMeshAndApply(MaterialCollection materials, Dictionary<ChunkSide, Chunk> neighbours, Chunk chunk, int slice, bool rebuild)
        {
            BuildMesh(materials, neighbours, chunk, slice, rebuild);
            ApplyMeshData(materials, slice, rebuild);
        }

        public Task BuildMesh(MaterialCollection materials, Dictionary<ChunkSide, Chunk> neighbours, Chunk chunk, int slice, bool rebuild)
        {
            return Task.Run(() =>
            {
                if (rebuild)
                {
                    _meshData = GreedyMeshing.CreateMesh(chunk, neighbours, materials, null);
                }
                if (slice >= 0 && slice < ChunkDataSettings.YSize)
                {
                    _meshDataSlice = GreedyMeshing.CreateMesh(chunk, neighbours, materials, slice);
                }
            });            
        }

        public void ApplyMeshData(MaterialCollection materials, int slice, bool rebuild)
        {
            if (rebuild)
            {
                ApplyMeshData(materials, _meshData, ref _mesh, _meshRenderer, _meshCollider, _meshFilter);
                gameObject.SetActive(_meshData.Vertices.Length != 0);
            }
            if (slice >= 0 && slice < ChunkDataSettings.YSize)
            {
                ApplyMeshData(materials, _meshDataSlice, ref _slicedMesh, _slicedRenderer, _slicedCollider, _slicedFilter);
                _sliced.SetActive(_meshDataSlice.Vertices.Length != 0);
            }
            else
            {
                _sliced.SetActive(false);
            }
        }

        private static void ApplyMeshData(MaterialCollection materials, MeshData meshdata, ref Mesh mesh, MeshRenderer meshRenderer, MeshCollider meshCollider, MeshFilter meshFilter)
        {
            if (mesh == null)
                mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = meshdata.Vertices;
            mesh.normals = meshdata.Normals;
            mesh.subMeshCount = meshdata.Triangles.Keys.Count;
            mesh.uv = meshdata.Uvs[0];
            mesh.uv2 = meshdata.Uvs[1];
            mesh.uv3 = meshdata.Uvs[2];
            mesh.colors = new Color[meshdata.Vertices.Length];

            var keyArray = meshdata.Triangles.Keys.ToArray();
            var myMats = new Material[meshdata.Triangles.Keys.Count];
            for (var i = 0; i < meshdata.Triangles.Keys.Count; i++)
            {
                mesh.SetTriangles(meshdata.Triangles[keyArray[i]], i);
                myMats[i] = materials.GetById((ushort) keyArray[i]).Material;
            }
            meshRenderer.sharedMaterials = myMats;
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
            //SetHighlightMaterial(_highlightColor);
        }
    }
}