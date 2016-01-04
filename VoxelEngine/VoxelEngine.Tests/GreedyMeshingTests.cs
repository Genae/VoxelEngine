using NUnit.Framework;
using VoxelEngine.Algorithmen.GreedyMeshing;
using VoxelEngine.GameData;

namespace VoxelEngine.Tests
{
    [TestFixture]
    public class GreedyMeshingTests
    {
        [Test]
        public void GreedyMeshingCanCreatePlanes()
        {
            //arange
            var voxels = new Voxel[4, 4, 4];
            for (int i = 0; i < voxels.Length; i++)
            {
                voxels[i/16, (i/4)%4, i%4] = new Voxel();
            }
            voxels[0, 0, 0].IsActive = true;
            var borders = new bool[6][,];
            for (int i = 0; i < 6; i++)
            {
                borders[i] = new bool[4,4];
            }

            //act
            var planes = GreedyMeshing.InitializePlanes(voxels, borders);

            //assert
            Assert.That(planes[0][0][0, 0], Is.EqualTo(1));
            Assert.That(planes[1][0][0, 0], Is.EqualTo(1));
            Assert.That(planes[2][0][0, 0], Is.EqualTo(1));
            Assert.That(planes[3][0][0, 0], Is.EqualTo(1));
            Assert.That(planes[4][0][0, 0], Is.EqualTo(1));
            Assert.That(planes[5][0][0, 0], Is.EqualTo(1));
            Assert.That(planes[5][1][0, 0], Is.EqualTo(0));
        }

        [Test]
        public void GreedyMeshingCanSetVisited()
        {
            //arange
            var visited = new bool[4,4];
            var curRectangle = new Rect(1,1);
            var curRectangle2 = new Rect(2, 0) {Height = 3};

            //act
            GreedyMeshing.SetVisited(curRectangle, visited);
            GreedyMeshing.SetVisited(curRectangle2, visited);

            //assert
            Assert.That(visited, Is.EqualTo(new bool[,]
            {
                {false, false, false, false},
                {false, true, false, false},
                {true, true, true, false},
                {false, false, false, false}
            }));
        }

        [Test]
        public void GreedyMeshingCanGreedyMeshPlanes()
        {
            //arange
            var plane = new int[,]
            {
                {0, 0, 0, 0 },
                {1, 1, 0, 0 },
                {0, 1, 1, 1 },
                {0, 1, 1, 1 },
            };

            //act
            var rect = GreedyMeshing.CreateRectsForPlane(plane);

            //assert
            Assert.That(rect[0], Is.EqualTo(new Rect(1,0) {Height = 2}));
            Assert.That(rect[1], Is.EqualTo(new Rect(2, 1) { Height = 3, Width = 2}));
        }

        [Test]
        public void GreedyMeshingCanCreateMeshes()
        {
        }
    }
}
