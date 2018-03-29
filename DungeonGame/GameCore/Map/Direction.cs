using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Map
{
    /// <summary>
    /// Class containing constants which represents directions.
    /// Can be used as index to array of length 4.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// North direction. Represents 0.
        /// </summary>
        NORTH,

        /// <summary>
        /// East direction. Represents 1.
        /// </summary>
        EAST,

        /// <summary>
        /// South direction. Represents 2.
        /// </summary>
        SOUTH,

        /// <summary>
        /// West direction. Represents 3.
        /// </summary>
        WEST,

        /// <summary>
        /// Special direction to be used instead of null.
        /// </summary>
        NO_DIRECTION

    }

    /// <summary>
    /// Extension class for Direciton enum.
    /// </summary>
    public static class DirectionMethods
    {
        /// <summary>
        /// Returns all directions. Must be done via item in enum so it's not exactly a nice way.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static Direction[] GetAllDirections()
        {
            return new Direction[] { Direction.NORTH, Direction.EAST, Direction.SOUTH, Direction.WEST };
        }

        /// <summary>
        /// Returns direction which is opposite.
        /// </summary>
        /// <param name="dir">Source direction.</param>
        /// <returns>Opposite direction.</returns>
        public static Direction OppositeDirection(this Direction dir)
        {
            switch(dir)
            {
                case Direction.NORTH:
                    return Direction.SOUTH;

                case Direction.EAST:
                    return Direction.WEST;

                case Direction.SOUTH:
                    return Direction.NORTH;

                case Direction.WEST:
                    return Direction.EAST;

                default:
                    throw new ArgumentException($"Opposite direction for {dir} not implemented.");
            }
        }

        /// <summary>
        /// Returns true if this direciton is NO_DIRECTION.
        /// </summary>
        /// <param name="dir">This direction.</param>
        /// <returns>True if NO_DIRECTION.</returns>
        public static bool IsNoDirection(this Direction dir)
        {
            return dir == Direction.NO_DIRECTION;
        }

        /// <summary>
        /// Returns direction from one block to another. Uses only block's coordinates to calculate the direction.
        /// Returns NO_DIRECTION if the blocks have same coordinates.
        /// Works best for adjacent blocks.
        /// </summary>
        /// <param name="from">Start block<./param>
        /// <param name="to">Target block.</param>
        /// <returns>Direction between two blocks.</returns>
        public static Direction GetDirection(MapBlock from, MapBlock to)
        {
            if (from.X < to.X)
            {
                return Direction.EAST;
            }
            else if (from.X > to.X)
            {
                return Direction.WEST;
            }
            else if (from.Y < to.Y)
            {
                return Direction.SOUTH;
            }
            else if (from.Y > to.Y)
            {
                return Direction.NORTH;
            } 
            else
            {
                return Direction.NO_DIRECTION;
            }
        }
    }
}
