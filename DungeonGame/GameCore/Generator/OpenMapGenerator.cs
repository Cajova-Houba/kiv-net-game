using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Generator
{
    /// <summary>
    /// This class will generate open map - a map which consists of blocks with all entrances open.
    /// </summary>
    public class OpenMapGenerator : IMapGenerator
    {
        public MapBlock[,] GenerateGrid(int width, int height)
        {
            MapBlock[,] grid = new MapBlock[width, height];
            Direction[] allDirections = new Direction[] { Direction.NORTH, Direction.EAST, Direction.SOUTH, Direction.WEST };
            for(int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    grid[i, j] = new MapBlock(i, j);
                    foreach(Direction direction in  allDirections)
                    {
                        grid[i, j].CreateEntrance(direction);
                    }
                }
            }

            return grid;
        }

        public Map.Map GenerateMap(int width, int height)
        {
            Map.Map map = new Map.Map();
            map.InitializeMap(GenerateGrid(width, height));

            return map;
        }
    }
}
