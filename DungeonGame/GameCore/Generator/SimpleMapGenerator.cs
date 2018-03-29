using GameCore.Map;
using System.Collections.Generic;
using System;

namespace GameCore.Generator
{
    /// <summary>
    /// This generator creates map as a simple maze. At least one path exists between every two cells in this maze.
    /// </summary>
    public class SimpleMapGenerator : IMapGenerator
    {
        public MapBlock[,] GenerateGrid(int width, int height, int seed)
        {
            // start with open maze
            MapBlock[,] grid = new MapBlock[width, height];
            // array to hold information about cells which were already visited by algorithm
            bool[,] visitedGrid = new bool[width, height];
            Direction[] allDirections = DirectionMethods.GetAllDirections();
            for(int i = 0; i < width; i ++)
            {
                for (int j =0; j < height; j++)
                {
                    visitedGrid[i, j] = false;
                    grid[i, j] = new MapBlock(i,j);
                }
            }

            // go through maze and demolish walls to create paths
            Stack<MapBlock> stack = new Stack<MapBlock>();
            Random r;
            if(seed == IMapGeneratorConstants.NO_SEED)
            {
                r = new Random();
            } else
            {
                r = new Random(seed);
            }
            MapBlock start = grid[r.Next(width), r.Next(height)];
            stack.Push(start);
            
            while(stack.Count > 0)
            {
                MapBlock current = stack.Peek();

                // select neighbours of current block which were not visited yet
                List<MapBlock> notVisited = new List<MapBlock>();
                if(current.Y > 0 && !visitedGrid[current.X, current.Y -1])
                {
                    notVisited.Add(grid[current.X, current.Y - 1]);
                }
                if(current.Y < height -1 && !visitedGrid[current.X, current.Y +1])
                {
                    notVisited.Add(grid[current.X, current.Y + 1]);
                }
                if(current.X > 0 && !visitedGrid[current.X -1, current.Y]) {
                    notVisited.Add(grid[current.X -1, current.Y]);
                }
                if (current.X < width - 1 && !visitedGrid[current.X + 1, current.Y])
                {
                    notVisited.Add(grid[current.X + 1, current.Y]);
                }

                // there are some neighbours which were not visited
                // choose one a create entrance between current block and neighbour block
                if (notVisited.Count > 0)
                {
                    int neighbourIndex = r.Next(notVisited.Count);
                    MapBlock neighbour = notVisited[neighbourIndex];
                    notVisited.RemoveAt(neighbourIndex);
                    DemolishWalls(current, neighbour);

                    // add neighbour to the stack so the path creation will continue this way
                    stack.Push(neighbour);

                    // mark current cell and neighbour as visited
                    visitedGrid[current.X, current.Y] = true;
                    visitedGrid[neighbour.X, neighbour.Y] = true;
                } else
                {
                    // every neighbour was visited, pop current block and continue with next block in the stack
                    stack.Pop();
                }
            }

            return grid;
        }

        /// <summary>
        /// Demolishes walls between two blocks. Determines fromDirection - direction in which the wall in fromBlock will be demolished.
        /// </summary>
        /// <param name="fromBlock">Wall which lies in fromDirection in this block will be demolished.</param>
        /// <param name="toBlock">Wall which lies in oposite to fromDirection in this block will be demolished.</param>
        private void DemolishWalls(MapBlock fromBlock, MapBlock toBlock)
        {
            // determine fromDirection
            Direction fromDirection = Direction.NORTH;
            if(fromBlock.X < toBlock.X)
            {
                fromDirection = Direction.EAST;
            } else if (fromBlock.X > toBlock.X)
            {
                fromDirection = Direction.WEST;
            } else if (fromBlock.Y < toBlock.Y)
            {
                fromDirection = Direction.SOUTH;
            } else
            {
                fromDirection = Direction.NORTH;
            }

            fromBlock.EntranceInDirection(fromDirection).DemolishWall();
            toBlock.EntranceInDirection(fromDirection.OppositeDirection()).DemolishWall();
        }

        public Map.Map GenerateMap(int width, int height, int seed)
        {
            Map.Map map = new Map.Map();
            map.InitializeMap(GenerateGrid(width, height, seed));
            return map;
        }
    }
}
