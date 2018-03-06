using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;
using GameCore.Objects;

namespace GameCore.Map
{
    /// <summary>
    /// Map is represented by grid of MapBlocks.
    /// </summary>
    public class MapBlock
    {
        /// <summary>
        /// X coordinate of this block.
        /// </summary>
        private readonly int x;

        /// <summary>
        /// Y coordinate of this block.
        /// </summary>
        private readonly int y;

        /// <summary>
        /// Array to store possible entrances.
        /// </summary>
        private Entrance[] entrances;

        /// <summary>
        /// Optional game object placed in this map block.
        /// </summary>
        private GameObject gameObject;

        /// <summary>
        /// Reference to parent map;
        /// </summary>
        protected Map parentMap;

        /// <summary>
        /// North entrance to this block.
        /// </summary>
        public Entrance North { get { return entrances[(int)Direction.NORTH]; } }

        /// <summary>
        /// East entrance to this block.
        /// </summary>
        public Entrance East { get { return entrances[(int)Direction.EAST]; } }

        /// <summary>
        /// South entrance to this block.
        /// </summary>
        public Entrance South { get { return entrances[(int)Direction.SOUTH]; } }

        /// <summary>
        /// West entrance to this block.
        /// </summary>
        public Entrance West { get { return entrances[(int)Direction.WEST]; } }

        /// <summary>
        /// Default constructor which initializes map block with 4 NONEXISTENT entrances and [0,0] coordinates.
        /// </summary>
        public MapBlock() : this(null, 0, 0)
        {
        }

        /// <summary>
        /// Initializes map block with 4 NONEXISTENT entrances, parent map and position in that map..
        /// </summary>
        /// <param name="parentMap">Reference to map this block lies in.</param>
        /// <param name="x">X coordinate of this block.</param>
        /// <param name="y">Y coordinate of this block.</param>
        public MapBlock(Map parentMap, int x, int y)
        {
            entrances = new Entrance[4];
            this.parentMap = parentMap;
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Returns entance to this map block in given direction.
        /// </summary>
        /// <param name="direction">Direction of the entrance.</param>
        /// <returns>Entrance.</returns>
        public Entrance EntranceInDirection(Direction direction)
        {
            return entrances[(int)direction];
        }

        /// <summary>
        /// Returns the block which lies next to this block in given direction.
        /// If such block doesn't exist null is returned.
        /// </summary>
        /// <param name="direction">Direction of next block.</param>
        /// <returns>Adjacent block or null.</returns>
        public MapBlock NextBlock(Direction direction)
        {
            if (parentMap == null)
            {
                return null;
            }

            return parentMap.AdjacentBlock(x, y, direction);
        }

    }
}
