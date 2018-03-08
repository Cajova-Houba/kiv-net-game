﻿using System;
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
        public Map ParentMap { get; protected set; }

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
        public MapBlock() : this(0, 0)
        {
        }

        /// <summary>
        /// Initializes map block with 4 NONEXISTENT entrances, parent map and position in that map..
        /// </summary>
        /// <param name="x">X coordinate of this block.</param>
        /// <param name="y">Y coordinate of this block.</param>
        public MapBlock(int x, int y)
        {
            entrances = new Entrance[4];
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Assigns this block to a new map. Coordinates will remain unchanged.
        /// </summary>
        /// <param name="map">New parent map.</param>
        public void AssignToMap(Map map)
        {
            ParentMap = map;
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
            if (ParentMap == null)
            {
                return null;
            }

            return ParentMap.AdjacentBlock(x, y, direction);
        }

        /// <summary>
        /// Creates OPEN entrance in given direction.
        /// </summary>
        /// <param name="direction">Direction in which entrance will be created.</param>
        public void CreateEntrance(Direction direction)
        {
            entrances[(int)direction] = new Entrance(EntranceState.OPEN);
        }

    }
}
