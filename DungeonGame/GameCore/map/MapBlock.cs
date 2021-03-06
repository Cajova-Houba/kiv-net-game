﻿using GameCore.Map.Serializer;
using GameCore.Objects;
using GameCore.Objects.Creatures;
using GameCore.Objects.Items;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameCore.Map
{
    /// <summary>
    /// Map is represented by grid of MapBlocks.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn, Title = "mapBlock")]
    public class MapBlock //: IBinarySerializable
    {
        /// <summary>
        /// X coordinate of this block.
        /// </summary>
        private int x;

        /// <summary>
        /// Y coordinate of this block.
        /// </summary>
        private int y;

        /// <summary>
        /// Array to store possible entrances.
        /// </summary>
        private Entrance[] entrances;

        /// <summary>
        /// Creature which may occupy this map block.
        /// </summary>
        private AbstractCreature creature;

        /// <summary>
        /// Reference to parent map;
        /// </summary>
        public Map ParentMap { get; protected set; }

        /// <summary>
        /// Optional game object placed in this map block.
        /// </summary>
        [JsonProperty]
        public AbstractItem Item { get; set; }
        
        /// <summary>
        /// North entrance to this block.
        /// </summary>
        [JsonProperty]
        public Entrance North { get { return entrances[(int)Direction.NORTH]; } }

        /// <summary>
        /// East entrance to this block.
        /// </summary>
        [JsonProperty]
        public Entrance East { get { return entrances[(int)Direction.EAST]; } }

        /// <summary>
        /// South entrance to this block.
        /// </summary>
        [JsonProperty]
        public Entrance South { get { return entrances[(int)Direction.SOUTH]; } }

        /// <summary>
        /// West entrance to this block.
        /// </summary>
        [JsonProperty]
        public Entrance West { get { return entrances[(int)Direction.WEST]; } }

        /// <summary>
        /// Returns X coordinate of this block.
        /// </summary>
        [JsonProperty]
        public int X { get { return x; } set { x = value; } }

        /// <summary>
        /// Returns Y coordinate of this block.
        /// </summary>
        [JsonProperty]
        public int Y { get { return y; } set { y = value; } }

        /// <summary>
        /// Returns true if this map block is occupied by any non-dead creature (or player).
        /// </summary>
        public bool Occupied { get { return creature != null && creature.Alive;} }

        /// <summary>
        /// Gets or sets creature which occupies this block.
        /// </summary>
        public AbstractCreature Creature { get { return creature; } set { creature = value; } }

        /// <summary>
        /// Default constructor which initializes map block with 4 NONEXISTENT entrances and [0,0] coordinates.
        /// </summary>
        public MapBlock() : this(0, 0)
        {
        }

        /// <summary>
        /// Initializes map block with 4 NONEXISTENT entrances and coordinates.
        /// </summary>
        /// <param name="x">X coordinate of this block.</param>
        /// <param name="y">Y coordinate of this block.</param>
        public MapBlock(int x, int y)
        {
            entrances = new Entrance[4];
            for(int i = 0; i < entrances.Length; i++)
            {
                entrances[i] = new Entrance();
            }
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
        /// Uses parent map to determine next block and if the parent map is null, null is returned.
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
        /// Returns the block thich lies next to this block in given direction and is accessible (entrance is OPEN).
        /// If such block doesn't exist null is returned.
        /// 
        /// Uses parent map to determine next block and if the parent map is null, null is returned.
        /// </summary>
        /// <param name="direction">Direction of next block.</param>
        /// <returns>Adjacent, accessible block or null.</returns>
        public MapBlock NextOpenBlock(Direction direction)
        {
            MapBlock nextOpenBlock = NextBlock(direction);

            if(nextOpenBlock == null)
            {
                return null;
            } else
            {
                if (EntranceInDirection(direction).IsOpen() && nextOpenBlock.EntranceInDirection(direction.OppositeDirection()).IsOpen())
                {
                    return nextOpenBlock;
                } else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns all accessible neighbours of this map block.
        /// Neighbour is acessible if there's no wall/locked gate between this block and the neighbour.
        /// </summary>
        /// <returns>Neighbour blocks.</returns>
        public List<MapBlock> AccessibleNeighbours()
        {
            Direction[] allDirections = DirectionMethods.GetAllDirections();
            List<MapBlock> neighbours = new List<MapBlock>();
            foreach(Direction dir in allDirections)
            {
                MapBlock neighbour = NextOpenBlock(dir);
                if (neighbour != null)
                {
                    neighbours.Add(neighbour);
                }
            }

            return neighbours;
        }

        /// <summary>
        /// Creates OPEN entrance in given direction.
        /// </summary>
        /// <param name="direction">Direction in which entrance will be created.</param>
        public void CreateEntrance(Direction direction)
        {
            entrances[(int)direction] = new Entrance(EntranceState.OPEN);
        }

        /// <summary>
        /// Picks up item from this block and returns it. Item property will return null after this method.
        /// If there's no item, null is returned.
        /// </summary>
        /// <returns>Returns item in this block.</returns>
        public AbstractItem PickUpItem()
        {
            AbstractItem item = Item;
            Item = null;
            return item;
        }

        /// <summary>
        /// Two map blocks are equal if their coordinates match.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var block = obj as MapBlock;
            return block != null &&
                   x == block.x &&
                   y == block.y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"MapBlock [{X},{Y}]";
        }

        //public void SerializeBinary(List<byte> serialized)
        //{
        //    byte tmp = 0;
        //    byte mask = 1;

        //    if (upper)
        //    {
        //        mask = (byte)(mask << 4);
        //    }

        //    if (mapBlock.North.IsOpen()) { tmp = (byte)(tmp | mask); }
        //    if (mapBlock.East.IsOpen()) { tmp = (byte)(tmp | (mask << 1)); }
        //    if (mapBlock.South.IsOpen()) { tmp = (byte)(tmp | (mask << 2)); }
        //    if (mapBlock.West.IsOpen()) { tmp = (byte)(tmp | (mask << 3)); }

        //    res = (byte)(source | tmp);
        //    throw new System.NotImplementedException();
        //}
    }
}
