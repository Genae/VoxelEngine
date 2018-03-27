using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Assets.Scripts.VoxelEngine.DataAccess;
using Assets.Scripts.VoxelEngine.Materials;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Assets.Scripts.WorldGeneration
{
    public class WorldGenerator
    {
        public static int WorldSeed = 0;
        public static void GenerateWorld(out World world, MaterialCollection collection, BiomeConfiguration configuration)
        {
            Random.InitState(WorldSeed);
            world = new World(collection);

            var mapSize = 300;
            var mapHeight = 100;

            var centerSize = 100;
            var centerHeight = 30;

            configuration.Init(collection);

            world.StartBatch();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var cuboidCount = 100;
            var cuboids = new List<Cuboid>();
            var tasks = new List<Task>();
            cuboids.AddRange(SplitCenterCube(centerSize, centerHeight));
            for (var c = 0; c < cuboidCount || cuboids.Count < cuboidCount/2; c++)
            {
                var posx = Random.Range(-mapSize / 2, mapSize / 2);
                var posy = Random.Range(-mapHeight / 2, mapHeight / 2);
                var posz = Random.Range(-mapSize / 2, mapSize / 2);
                if (posx > -centerSize && posy > -centerHeight && posz > -centerSize &&
                    posx < centerSize && posy < centerHeight && posz < centerSize)
                    continue;
                
                var posCentered = (mapSize - Mathf.Abs(posx)) * (mapHeight - Mathf.Abs(posy)) * (mapSize - Mathf.Abs(posz)) /
                                  (float)(mapSize * mapSize * mapHeight);

                var cubeSize = mapSize * posCentered;

                var width = Random.Range(cubeSize / 3, cubeSize) * 0.75f;
                var height = Random.Range(cubeSize / 5, cubeSize / 2.5f) * 0.75f;
                var depth = Random.Range(cubeSize / 3, cubeSize) * 0.75f;

                posx -= (int)width / 2;
                posy -= (int)width / 2;
                posz -= (int)width / 2;

                cuboids.Add(new Cuboid()
                {
                    Pos = new Vector3Int(posx, posy, posz),
                    Size = new Vector3Int((int)width, (int)height, (int)depth)
                });
            }
            foreach (var cuboid in cuboids)
            {
                tasks.Add(BuildRect(world, cuboid, configuration));
            }
            Task.WaitAll(tasks.ToArray());
            stopwatch.Stop();
            Debug.Log("finished Stone " + stopwatch.ElapsedMilliseconds);

            var waterCube = new Cuboid()
            {
                Pos = new Vector3Int(0, 50, 0),
                Size = new Vector3Int(1, 1, 1)
            };
            BuildRect(world, waterCube, null, configuration.GetFluid()).Wait();
            world.FinishBatch();
        }

        private static Cuboid[] SplitCenterCube(int centerSize, int centerHeight)
        {
            var size = new Vector3Int(centerSize, centerHeight, centerSize);
            return new []
            {
                new Cuboid{Pos = new Vector3Int(-centerSize, -centerHeight, -centerSize), Size = size},
                new Cuboid{Pos = new Vector3Int(0, -centerHeight, -centerSize), Size = size},
                new Cuboid{Pos = new Vector3Int(-centerSize, 0, -centerSize), Size = size},
                new Cuboid{Pos = new Vector3Int(0, 0, -centerSize), Size = size},
                new Cuboid{Pos = new Vector3Int(-centerSize, -centerHeight, 0), Size = size},
                new Cuboid{Pos = new Vector3Int(0, -centerHeight, 0), Size = size},
                new Cuboid{Pos = new Vector3Int(-centerSize, 0, 0), Size = size},
                new Cuboid{Pos = new Vector3Int(0, 0, 0), Size = size}
            };
        }

        private static Task BuildRect(World world, Cuboid c, BiomeConfiguration configuration = null, LoadedVoxelMaterial mat = null)
        {
            return Task.Run(() =>
            {
                for (var x = c.Pos.x; x < c.Pos.x + c.Size.x; x++)
                {
                    for (var z = c.Pos.z; z < c.Pos.z + c.Size.z; z++)
                    {
                        var i = 0;
                        for (var y = c.Pos.y + c.Size.y-1; y >= c.Pos.y; y--)
                        {
                            if (configuration != null)
                            {
                                mat = configuration.GetLayer(i);
                            }
                            world.SetVoxel(mat, new Vector3Int(x, y, z));
                            i++;
                        }
                    }
                    
                }
            });
        }


        /*
        private static Task BuildChunks(int x, int z, int mapHeight, ChunkCloud cloud, BiomeConfiguration configuration)
        {
            return Task.Run(() =>
            {
                for (var ix = x * ChunkDataSettings.XSize; ix < x * ChunkDataSettings.XSize + ChunkDataSettings.XSize; x++)
                {
                    for (var iz = z * ChunkDataSettings.ZSize; iz < z * ChunkDataSettings.ZSize + ChunkDataSettings.ZSize; z++)
                    {
                        var top = cloud.GetTopVoxel(ix, iz, mapHeight, -mapHeight);
                        if (top == null)
                            continue;
                        var i = 0;
                        for (var y = top.Value; y < mapHeight; y++)
                        {
                            var mat = configuration.GetLayer(i);
                            if (mat == null)
                                break;
                            cloud.SetVoxel(mat, new Vector3Int(ix, y, iz));
                        }
                    }
                }
            });
        }*/
    }

    internal class Cuboid
    {
        public Vector3Int Pos;
        public Vector3Int Size;

        public bool ContainsPos(int x, int y, int z)
        {
            return x > Pos.x && y > Pos.y && z > Pos.z && 
                   x < Pos.x + Size.x && y < Pos.y + Size.y && z < Pos.z + Size.z;
        }
        
        public bool Intersects(Cuboid a)
        {
            if (!IntersectsDimension(a.Pos.x, a.Pos.x + a.Size.x, Pos.x, Pos.x + Size.x))
                return false;

            if (!IntersectsDimension(a.Pos.y, a.Pos.y + a.Size.y, Pos.y, Pos.y + Size.y))
                return false;

            if (!IntersectsDimension(a.Pos.z, a.Pos.z + a.Size.z, Pos.z, Pos.z + Size.z))
                return false;

            return true;
        }

        private static bool IntersectsDimension(int aMin, int aMax, int bMin, int bMax)
        {
            return aMin <= bMax && aMax >= bMin;
        }
    }
}
