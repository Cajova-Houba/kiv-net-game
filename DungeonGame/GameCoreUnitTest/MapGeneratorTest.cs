using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameCore.Generator;
using GameCore.Map;

namespace GameCoreUnitTest
{
    /// <summary>
    /// Test cases for map generator are here.
    /// </summary>
    [TestClass]
    public class MapGeneratorTest
    {
        /// <summary>
        /// Generate open map anc check if it was generated correctly.
        /// </summary>
        [TestMethod]
        public void TestOpenMapGenerator()
        {
            IMapGenerator openMapGenerator = new OpenMapGenerator();
            int w = 10;
            int h = 15;
            Direction[] allDirections = DirectionMethods.GetAllDirections();

            MapBlock[,] grid = openMapGenerator.GenerateGrid(w, h, 0);
            Assert.IsNotNull(grid, "Null grid returned!");
            Assert.AreEqual(w, grid.GetLength(0), "Wrong width of map grid!");
            Assert.AreEqual(h, grid.GetLength(1), "Wrong height of map grid!");
            for(int i = 0; i < w; i ++)
            {
                for(int j = 0; j < h; j++)
                {
                    foreach (Direction dir in allDirections)
                    {
                        Assert.IsTrue(grid[i, j].EntranceInDirection(dir).IsOpen(), $"Entrance in direction {dir} of block [{i},{j}] should be open!");
                    }
                }
            }

            Map map = openMapGenerator.GenerateMap(w, h, 0);
            Assert.IsNotNull(map, "Null map returned!");
            Assert.AreEqual(w, map.Width, "Wrong map width!");
            Assert.AreEqual(h, map.Height, "Wrong map height!");
            MapBlock[,] grid2 = map.Grid;
            Assert.AreEqual(grid.GetLength(0), grid.GetLength(0), "Widths of grids don't match!");
            Assert.AreEqual(grid.GetLength(1), grid.GetLength(1), "Widths of grids don't match!");
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    foreach (Direction dir in allDirections)
                    {
                        Assert.IsTrue(grid2[i, j].EntranceInDirection(dir).IsOpen(), $"Entrance in direction {dir} of block [{i},{j}] should be open!");
                    }
                }
            }
        }

        /// <summary>
        /// Test simple map generator.
        /// </summary>
        [TestMethod]
        public void TestSimpleMapGenerator()
        {
            IMapGenerator simpleMapGenerator = new SimpleMapGenerator();
            int w = 5;
            int h = 10;
            Direction[] allDirections = DirectionMethods.GetAllDirections();

            MapBlock[,] grid = simpleMapGenerator.GenerateGrid(w, h, IMapGeneratorConstants.NO_SEED);

            Assert.IsNotNull(grid, "Null grid returned!");
            Assert.AreEqual(w, grid.GetLength(0), "Wrong width of map grid!");
            Assert.AreEqual(h, grid.GetLength(1), "Wrong height of map grid!");

            // test that no map block has all entrances closed
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    int entrances = 0;
                    foreach (Direction dir in allDirections)
                    {
                        if(grid[i,j].EntranceInDirection(dir).Exists())
                        {
                            entrances++;
                        }
                    }

                    Assert.IsTrue(entrances > 0, $"Block at [{i},{j}] has no entrance!");
                }
            }

        }

        /// <summary>
        /// Generate map with seed and test that same map will be generated with same seed.
        /// </summary>
        [TestMethod]
        public void TestSimpleMapGeneratorSeed()
        {
            IMapGenerator simpleMapGenerator = new SimpleMapGenerator();
            int w = 5;
            int h = 10;
            int seed = 87452;
            Direction[] allDirections = DirectionMethods.GetAllDirections();

            MapBlock[,] grid = simpleMapGenerator.GenerateGrid(w, h, seed);
            MapBlock[,] grid2 = simpleMapGenerator.GenerateGrid(w, h, seed);

            Assert.IsNotNull(grid, "Grid 1 is null!");
            Assert.IsNotNull(grid2, "Grid 2 is null!");
            Assert.AreEqual(w, grid.GetLength(0), "Wrong width of map grid!");
            Assert.AreEqual(h, grid.GetLength(1), "Wrong height of map grid!");
            Assert.AreEqual(w, grid2.GetLength(0), "Wrong width of map grid 2!");
            Assert.AreEqual(h, grid2.GetLength(1), "Wrong height of map grid 2!");
            
            // all map block should be same
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    int entrances = 0;
                    foreach (Direction dir in allDirections)
                    {
                        Assert.AreEqual(grid[i, j].EntranceInDirection(dir).Exists(), grid2[i, j].EntranceInDirection(dir).Exists(), $"Map block at position [{i},{j}] has different entrance in direction {dir}.");
                        if (grid[i, j].EntranceInDirection(dir).Exists())
                        {
                            entrances++;
                        }
                    }

                    Assert.IsTrue(entrances > 0, $"Block at [{i},{j}] has no entrance!");
                }
            }
        }
    }
}
