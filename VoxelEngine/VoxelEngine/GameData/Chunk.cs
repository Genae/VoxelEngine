using OpenTK;
using VoxelEngine.Algorithmen.GreedyMeshing;
using VoxelEngine.Shaders.DirectionalDiffuse;

namespace VoxelEngine.GameData
{
    public class Chunk : Mesh
    {
        public const int ChunkSize = 16;
        public Voxel[,,] Voxels;
        private bool[][,] _borders;
        public Mesh ChunkBorders;

        public Chunk(Vector3 pos):base(ChunkSize, pos)
        {
            Shader = DirectionalDiffuse.Instance;
            Voxels = new Voxel[ChunkSize, ChunkSize, ChunkSize];
            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        Voxels[x,y,z]  = new Voxel();
                    }
                }
            }
            ChunkBorders = new ChunkBorder(ChunkSize, pos);
            ChunkBorders.Shader = Shader;
        }

        public void UpdateBorder(bool[,] border, int side, bool runUpdate = true)
        {
            _borders[side] = border;
            if(runUpdate)
                OnChunkUpdated();
        }

        public void UpdateBorder(bool[][,] border, bool runUpdate = true)
        {
            _borders = border;
            for (int i = 0; i < 6; i++)
            {
                if(_borders[i] == null)
                    _borders[i] = new bool[ChunkSize,ChunkSize];
            }
            if (runUpdate)
                OnChunkUpdated();
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
            GreedyMeshing.CreateMesh(out vertecies, out triangles, out colors, out normals, Voxels, _borders, Pos, Scale);
            ChunkBorders.SetActive(triangles.Length != 0);

            CreateMesh(vertecies, triangles, colors, normals);
        }

        public bool HasSolidBorder(int dir, out bool[,] border)
        {
            border = new bool[ChunkSize,ChunkSize];
            var solid = true;
            switch (dir)
            {
                case 1: //+x
                    for (var y = 0; y < ChunkSize; y++)
                    {
                        for (var z = 0; z < ChunkSize; z++)
                        {
                            border[y, z] = Voxels[ChunkSize - 1, y, z].IsActive;
                            solid = solid && Voxels[ChunkSize - 1, y, z].IsActive;
                        }
                    }
                    return solid;
                case 2: //-x
                    for (var y = 0; y < ChunkSize; y++)
                    {
                        for (var z = 0; z < ChunkSize; z++)
                        {
                            border[y, z] = Voxels[0, y, z].IsActive;
                            solid = solid && Voxels[0, y, z].IsActive;
                        }
                    }
                    return solid;
                case 3: //+y
                    for (var x = 0; x < ChunkSize; x++)
                    {
                        for (var z = 0; z < ChunkSize; z++)
                        {
                            border[x, z] = Voxels[x, ChunkSize - 1, z].IsActive;
                            solid = solid && Voxels[x, ChunkSize - 1, z].IsActive;
                        }
                    }
                    return solid;
                case 4: //-y
                    for (var x = 0; x < ChunkSize; x++)
                    {
                        for (var z = 0; z < ChunkSize; z++)
                        {
                            border[x, z] = Voxels[x, 0, z].IsActive;
                            solid = solid && Voxels[x, 0, z].IsActive;
                        }
                    }
                    return true;
                case 5: //+z
                    for (var x = 0; x < ChunkSize; x++)
                    {
                        for (var y = 0; y < ChunkSize; y++)
                        {
                            border[x, y] = Voxels[x, y, ChunkSize - 1].IsActive;
                            solid = solid && Voxels[x, y, ChunkSize - 1].IsActive;
                        }
                    }
                    return solid;
                case 6: //-z
                    for (var x = 0; x < ChunkSize; x++)
                    {
                        for (var y = 0; y < ChunkSize; y++)
                        {
                            border[x,y] = Voxels[x, y, 0].IsActive;
                            solid = solid && Voxels[x, y, 0].IsActive;
                        }
                    }
                    return solid;
            }
            return false;
        }
    }
}
