using Assets.Scripts.VoxelEngine.DataAccess;
using Assets.Scripts.VoxelEngine.Materials;
using UnityEngine;

namespace Assets.Scripts.WorldGeneration
{
    public class TestCube : MonoBehaviour
    {
        private MaterialCollection _collection;
        public BiomeConfiguration BiomeConfiguration;
        private World _world;
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
            WorldGenerator.GenerateWorld(out _world, _collection, BiomeConfiguration);
        }


        // Update is called once per frame
        void Update () {
            if (_init-- == 0)
            {
                Init();
            }
            if (_oldSlice != Slice && _init < 0)
            {
                _world.SetSlice(Slice);
                _oldSlice = Slice;
            }
        }
    }
}
