using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Map
{
    /// <summary>
    /// Class which contains game map.
    /// </summary>
    public class Map
    {
        /// <summary>
        /// Number of MapBlocks in horizontal direction.
        /// </summary>
        private int width;

        /// <summary>
        /// Number of MapBlocks in vertical direction.
        /// </summary>
        private int height;

        /// <summary>
        /// Game map.
        /// </summary>
        private MapBlock[][] grid;
        
        /// <summary>
        /// Returns MapBlock which is adjacent to current block given by [curX,curY] and lies in a given
        /// direction from current block.
        /// </summary>
        /// <param name="curX">X coordinate of current map block.</param>
        /// <param name="curY">Y coordinate of current map block.</param>
        /// <param name="direction">Directino of adjacent map block.</param>
        /// <returns>Adjacent map block or null if such block doesn't exists.</returns>
        public MapBlock AdjacentBlock(int curX, int curY, Direction direction)
        {
            if (curX < 0 || curX >= width || curY < 0 || curY >= height)
            {
                throw new ArgumentOutOfRangeException($"Current block coordinates [{curX},{curY}] are out of boundaries.");
            }

            switch (direction)
            {

                case Direction.NORTH:
                    if (curY == 0)
                    {
                        return null;
                    } else
                    {
                        return grid[curX][curY - 1];
                    }

                case Direction.EAST:
                    if (curX == width -1)
                    {
                        return null;
                    } else
                    {
                        return grid[curX + 1][curY];
                    }

                case Direction.SOUTH:
                    if (curY == height -1)
                    {
                        return null;
                    } else
                    {
                        return grid[curX][curY + 1];
                    }

                case Direction.WEST:
                    if (curX == 0)
                    {
                        return null;
                    } else
                    {
                        return grid[curX - 1][curY];
                    }

                default:
                    throw new NotSupportedException($"Direction {direction} is not supported!");
            }
        }
    }
}
