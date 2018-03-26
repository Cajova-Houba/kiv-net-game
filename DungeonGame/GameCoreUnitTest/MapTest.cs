using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameCore.Map;
using GameCore.Objects.Creatures;

namespace GameCoreUnitTest
{
    /// <summary>
    /// Class for Map-related unit tests.
    /// </summary>
    [TestClass]
    public class MapTest
    {
        /// <summary>
        /// Test that map block with creatures are occupied.
        /// </summary>
        [TestMethod]
        public void TestOccupiedBlock()
        {
            MapBlock mb1 = new MapBlock();
            Assert.IsFalse(mb1.Occupied, "Empty map block shouldn't be occupied!");

            mb1.Creature = new Monster("Test monster", mb1, 10, 10, 10);
            Assert.IsTrue(mb1.Occupied, "Block with creature should be occupied!");
        }

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
            map.InitializeMap( grid);

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
        /// Test AdjacentBlock() method on map with only one block.
        /// </summary>
        [TestMethod]
        public void TestAdjacentBlockOneBlockMap()
        {
            // base map
            Map map = new Map();

            // create grid, initialize map
            MapBlock[,] grid = new MapBlock[1, 1];
            grid[0, 0] = new MapBlock(0, 0);
            map.InitializeMap(grid);

            // test
            Direction[] directions = new Direction[] { Direction.NORTH, Direction.EAST, Direction.SOUTH, Direction.WEST };
            foreach(Direction direction in directions)
            {
                Assert.IsNull(map.AdjacentBlock(0, 0, direction), $"Adjacent block in diretion {direction} should be null!");
            }
        }

        /// <summary>
        /// Test AdjacentBlock() method on map with 3x3 blocks.
        /// </summary>
        [TestMethod]
        public void TestAdjacentBlock()
        {
            // base map
            Map map = new Map();

            // create grid, initialize map
            int w = 3;
            int h = 3;
            MapBlock[,] grid = new MapBlock[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    grid[i, j] = new MapBlock(i, j);
                }
            }
            map.InitializeMap(grid);

            // tests
            //middle block [2,2] should have all adjacent blocks non-null
            Direction[] directions = new Direction[] { Direction.NORTH, Direction.EAST, Direction.SOUTH, Direction.WEST };
            foreach (Direction direction in directions)
            {
                Assert.IsNotNull(map.AdjacentBlock(1, 1, direction), $"Adjacent block in diretion {direction} should exist for middle block!");
            }

            // top blocks [x,0] should have no NORTH adjacent blocks
            // right blocks [w-1,y] should have no EAST adjacent blocks
            // bottom blocks [x,h-1] should have no SOUTH adjacent blocks
            // right blocks [0,y] should have no WEST adjacent blocks
            for (int i = 0; i < w; i++)
            {
                Assert.IsNull(map.AdjacentBlock(i, 0, Direction.NORTH), $"Block [{i},{0}] should not have {Direction.NORTH} adjacent block!");
                Assert.IsNull(map.AdjacentBlock(i, h - 1, Direction.SOUTH), $"Block [{i},{h - 1}] should not have {Direction.SOUTH} adjacent block!");
            }
            for (int j = 0; j < h; j++)
            {
                Assert.IsNull(map.AdjacentBlock(w-1, j, Direction.EAST), $"Block [{w-1},{j}] should not have {Direction.EAST} adjacent block!");
                Assert.IsNull(map.AdjacentBlock(0, j, Direction.WEST), $"Block [{0},{j}] should not have {Direction.WEST} adjacent block!");
            }
        }
    }
}
