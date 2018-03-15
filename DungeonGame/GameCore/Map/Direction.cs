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
        public static Direction[] GetAllDirections(this Direction dir)
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
    }
}
