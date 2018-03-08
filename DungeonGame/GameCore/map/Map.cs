using System;

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
        public int Width { get; protected set; }

        /// <summary>
        /// Number of MapBlocks in vertical direction.
        /// </summary>
        public int Height { get; protected set; }

        /// <summary>
        /// Game map.
        /// </summary>
        public MapBlock[,] Grid { get; protected set; }


        /// <summary>
        /// Initializes map with dimensions 0x0 and empty grid.
        /// </summary>
        public Map()
        {
            InitializeMap(new MapBlock[0, 0]);
        }

        /// <summary>
        /// Initializes this map with given values and assigns parentMap to each MapBlock.
        /// </summary>
        /// <param name="grid">Map, dimensions should match with width and height.</param>
        public void InitializeMap(MapBlock[,] grid)
        {
            Width = grid.GetLength(0);
            Height = grid.GetLength(1);
            Grid = grid;

            for(int i = 0; i < Width; i++)
            {
                for( int j = 0; j < Height; j++)
                {
                    grid[i, j].AssignToMap(this);
                }
            }
        }

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
            if (curX < 0 || curX >= Width || curY < 0 || curY >= Height)
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
                        return Grid[curX,curY - 1];
                    }

                case Direction.EAST:
                    if (curX == Width -1)
                    {
                        return null;
                    } else
                    {
                        return Grid[curX + 1,curY];
                    }

                case Direction.SOUTH:
                    if (curY == Height -1)
                    {
                        return null;
                    } else
                    {
                        return Grid[curX,curY + 1];
                    }

                case Direction.WEST:
                    if (curX == 0)
                    {
                        return null;
                    } else
                    {
                        return Grid[curX - 1,curY];
                    }

                default:
                    throw new NotSupportedException($"Direction {direction} is not supported!");
            }
        }
    }
}
