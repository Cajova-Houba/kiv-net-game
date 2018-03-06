using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameCore.Map;

namespace GameCoreUnitTest
{
    /// <summary>
    /// Class for Map-related unit tests.
    /// </summary>
    [TestClass]
    public class MapTest
    {
        /// <summary>
        /// Test map initialization.
        /// </summary>
        [TestMethod]
        public void TestInitializeMap()
        {
            // base map
            Map map = new Map();

            // create some grind to initialize map with
            int w = 10;
            int h = 10;
            MapBlock[,] grid = new MapBlock[w, h];
            for(int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    grid[i,j] = new MapBlock(i,j);
                }
            }
            map.InitializeMap(grid.GetLength(0), grid.GetLength(1), grid);

            // test initialitazion
            Assert.IsNotNull(map.Grid, "Grid is not initialized!");
            Assert.AreEqual(w, map.Width, "Wrong width!");
            Assert.AreEqual(h, map.Height, "Wrong height!");
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    MapBlock curBlock = map.Grid[i, j];
                    Assert.IsNotNull(curBlock, $"Map block at [{i},{j}] not initialized!");
                    Assert.AreEqual(map, curBlock.ParentMap, $"Parent map of block [{i},{j}] set incorrectly!");
                }
            }
        }

        /// <summary>
        /// Test AdjacentBlock() method of map with only one block.
        /// </summary>
        [TestMethod]
        public void TestAdjacentBlockOneBlockMap()
        {
            // base map
            Map map = new Map();

            // create grid, initialize map
            MapBlock[,] grid = new MapBlock[1, 1];
            grid[0, 0] = new MapBlock(0, 0);
            map.InitializeMap(grid.GetLength(0), grid.GetLength(1), grid);

            // test
            Direction[] directions = new Direction[] { Direction.NORTH, Direction.EAST, Direction.SOUTH, Direction.WEST };
            foreach(Direction direction in directions)
            {
                Assert.IsNull(map.AdjacentBlock(0, 0, direction), $"Adjacent block in diretion {direction} should be null!");
            }
        }
    }
}
