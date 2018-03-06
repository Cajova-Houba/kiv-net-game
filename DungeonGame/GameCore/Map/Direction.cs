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
        WEST
    }
}
