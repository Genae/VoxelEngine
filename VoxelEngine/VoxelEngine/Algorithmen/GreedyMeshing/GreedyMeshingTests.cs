using NUnit.Framework;
using NUnit.Framework.Internal;
using VoxelEngine.GameData;

namespace VoxelEngine.Algorithmen.GreedyMeshing
{
    [TestFixture]
    public class GreedyMeshingTests
    {
        [Test]
        public void GreedyMeshingCanCreatePlanes()
        {
            //arange
            var voxels = new Voxel[4, 4, 4];
            voxels[0, 0, 0] = new Voxel() {IsActive = true};
            var borders = new bool[6][,];
            for (int i = 0; i < 6; i++)
            {
                borders[i] = new bool[4,4];
            }

            //act
            var planes = GreedyMeshing.InitializePlanes(voxels, borders);

            //assert
            
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
