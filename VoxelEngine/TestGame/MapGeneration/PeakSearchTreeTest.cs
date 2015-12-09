/*using System;
using System.Collections.Generic;
using Assets.Scripts.TerrainGeneration.Heightmap;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Scripts.Global
{
    [TestFixture]
    class TestRangeTree
    {
        [Test]
        public void TestUnitIsInRange()
        {
            var list = new List<Peak>
            {
               new Peak()
               {
                   PeakPoint = new Vector2(0,0)
               },
               new Peak()
               {
                   PeakPoint = new Vector2(10,0)
               },
               new Peak()
               {
                   PeakPoint = new Vector2(0,10)
               },
               new Peak()
               {
                   PeakPoint = new Vector2(20,20)
               },
               new Peak()
               {
                   PeakPoint = new Vector2(20,0)
               },
               new Peak()
               {
                   PeakPoint = new Vector2(0,20)
               },
            };
            var tree = new PeakSearchTree(list);

            //Act
            var listFound = tree.GetNearestPeaks(2, 2, 2);

            foreach (var peak in listFound)
            {
                Console.WriteLine(peak.PeakPoint);
            }

            //Assert
            Assert.IsTrue(listFound.Count > 1);
        }
    }
}*/

