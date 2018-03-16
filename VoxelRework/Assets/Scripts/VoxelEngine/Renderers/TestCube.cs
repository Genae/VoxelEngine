using Assets.Scripts.VoxelEngine.Containers.Chunks;
using Assets.Scripts.VoxelEngine.Materials;
using UnityEngine;

namespace Assets.Scripts.VoxelEngine.Renderers
{
    public class TestCube : MonoBehaviour
    {
        private MaterialCollection _collection;
        public VoxelMaterial OpaqueMaterial;
        public VoxelMaterial TransparentMaterial;
        private ChunkCloud _cloud;
        private int _oldSlice;
        public int Slice = 10;

        private int _init = 10;

        // Use this for initialization
        void Start ()
        {
        }
        
        private void Init()
        {
            _oldSlice = Slice;
            _collection = new MaterialCollection();
            var go = new GameObject("map");
            _cloud = new ChunkCloud(_collection, go.transform);
            _cloud.StartBatch();
            for (var x = -30; x < 30; x++)
            {
                for (var y = -5; y < 5; y++)
                {
                    for (var z = -30; z < 30; z++)
                    {
                        //_cloud.SetVoxel(TransparentMaterial, new Vector3Int(x, y, z));
                    }
                }
            }
            for (var x = -20; x < 2; x++)
            {
                for (var y = -20; y < 2; y++)
                {
                    for (var z = -20; z < 2; z++)
                    {
                        _cloud.SetVoxel(OpaqueMaterial, new Vector3Int(x, y, z));
                    }
                }
            }
            _cloud.FinishBatch();
        }

        // Update is called once per frame
        void Update () {
            if (_init == 0)
            {
                Init();
            }
            _init--;
            if (_oldSlice != Slice && _init < 0)
            {
                _cloud.SetSlice(Slice);
                _oldSlice = Slice;
            }
        }
    }
}
