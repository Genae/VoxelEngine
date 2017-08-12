using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Algorithms;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Util;
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
            chunkC.CanBeHighlighted = false;
            chunkC.InitializeContainer(new Vector3(x * ChunkSize, y * ChunkSize, z * ChunkSize), map.MapData.Chunks[x, y, z], MaterialRegistry.Instance.Materials);
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
        public bool CanBeHighlighted = true;
        private Color? _highlightColor;
        public Color? HighlightColor
        {
            get { return _highlightColor; }
            set
            {
                if (value != _highlightColor)
                {
                    SetHighlightMaterial(value);
                }
            }
        }

        private void SetHighlightMaterial(Color? value)
        {
            if (!CanBeHighlighted)
                return;
            var matReg = MaterialRegistry.Instance;
            if (value != null && !gameObject.GetComponent<MeshRenderer>().sharedMaterials.Any(m => m.shader.name.Equals(matReg.HighlightMaterial.shader.name)))
            {
                var mats = gameObject.GetComponent<MeshRenderer>().sharedMaterials.ToList();
                mats.Add(Instantiate(matReg.HighlightMaterial));
                gameObject.GetComponent<MeshRenderer>().sharedMaterials = mats.ToArray();
            }
            if (value == null)
            {
                var mats = gameObject.GetComponent<MeshRenderer>().sharedMaterials.ToList();
                gameObject.GetComponent<MeshRenderer>().sharedMaterials =
                    mats.Where(m => !m.shader.name.Equals(matReg.HighlightMaterial.shader.name)).ToArray();
            }
            else
            {
                var mats = gameObject.GetComponent<MeshRenderer>().sharedMaterials.ToList();
                mats.First(m => m.shader.name.Equals(matReg.HighlightMaterial.shader.name)).color = value.Value;
                gameObject.GetComponent<MeshRenderer>().sharedMaterials = mats.ToArray();
            }
            _highlightColor = value;
        }

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

        public void OnMouseOver()
        {
            HighlightColor = Color.white;
            if (this is Multiblock.Multiblock && transform.name.Equals("Tree"))
            {
                Exploder.Explode(this as Multiblock.Multiblock);
            }
        }
        
        public void OnMouseExit()
        {
            HighlightColor = null;
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

            Mesh = GetComponent<MeshFilter>() == null ? gameObject.AddComponent<MeshFilter>().mesh : GetComponent<MeshFilter>().sharedMesh;
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
            mCollider.sharedMesh = null;
            mCollider.sharedMesh = Mesh;
            SetHighlightMaterial(_highlightColor);
            gameObject.SetActive(vertices.Length != 0);
            //gameObject.GetComponent<MeshCollider>().enabled = vertices.Length != 0;
            //gameObject.GetComponent<MeshRenderer>().enabled = vertices.Length != 0;
            return upVoxels;
        }

        public Vector3 GetCenter()
        {
            return transform.GetComponent<MeshRenderer>().bounds.center;
        }
    }
}
