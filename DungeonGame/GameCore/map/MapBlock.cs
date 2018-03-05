using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.map
{
    /// <summary>
    /// Map is represented by grid of MapBlocks.
    /// </summary>
    public class MapBlock
    {
        /// <summary>
        /// Constant for accessing north entrance to the block.
        /// </summary>
        public const int NORTH = 0;

        /// <summary>
        /// Constant for accessing east entrance to the block.
        /// </summary
        public const int EAST = 1;

        /// <summary>
        /// Constant for accessing south entrance to the block.
        /// </summary
        public const int SOUTH = 2;

        /// <summary>
        /// Constant for accessing west entrance to the block.
        /// </summary
        public const int WEST = 3;

        /// <summary>
        /// Array to store possible entrances.
        /// </summary>
        private Entrance[] entrances;

        /// <summary>
        /// North entrance to this block.
        /// </summary>
        public Entrance North { get { return entrances[NORTH]; } }

        /// <summary>
        /// East entrance to this block.
        /// </summary>
        public Entrance East { get { return entrances[EAST]; } }

        /// <summary>
        /// South entrance to this block.
        /// </summary>
        public Entrance South { get { return entrances[SOUTH]; } }

        /// <summary>
        /// West entrance to this block.
        /// </summary>
        public Entrance West { get { return entrances[WEST]; } }

        public MapBlock()
        {
            entrances = new Entrance[4];
        }

    }
}
