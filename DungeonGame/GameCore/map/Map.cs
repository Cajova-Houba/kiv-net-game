using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.map
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
    }
}
