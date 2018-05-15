using GameCore.Objects.Creatures;
using GameCore.Objects.Items;
using Newtonsoft.Json;
using System;

namespace GameCore.Map
{
    /// <summary>
    /// Class which contains game map.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Map
    {
        /// <summary>
        /// Optional map name. Used to identify it in game (this name will be displayed).
        /// </summary>
        public string MapName { get; set; }

        /// <summary>
        /// Number of MapBlocks in horizontal direction.
        /// </summary>
        [JsonProperty]
        public int Width { get; protected set; }
        
        /// <summary>
        /// Number of MapBlocks in vertical direction.
        /// </summary>
        [JsonProperty]
        public int Height { get; protected set; }
        
        /// <summary>
        /// Winning block of this map.
        /// </summary>
        public MapBlock WinningBlock { get; set; }

        /// <summary>
        /// Game map.
        /// </summary>
        [JsonProperty]
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
        /// Adds crature to this map. Assumes that creature's position is not null.
        /// If the block is already occupied, exception is thrown.
        /// </summary>
        /// <param name="creature">Creature to be placed.</param>
        public void AddCreature(AbstractCreature creature)
        {
            int x = creature.Position.X;
            int y = creature.Position.Y;
            if (Grid[x, y].Occupied)
            {
                throw new Exception($"Na bloku [{x},{y}] je již umístěna postava {Grid[x, y].Creature.Name} a nelze na něj umístit {creature.Name}!");
            }

            Grid[x, y].Creature = creature;
        }

        /// <summary>
        /// Adds item to this map. Assumes that item's position is not null.
        /// If the block is already occupied, exception is thrown.
        /// </summary>
        /// <param name="item">Item to be placed.</param>
        public void AddItem(AbstractItem item)
        {
            int x = item.Position.X;
            int y = item.Position.Y;
            if (Grid[x, y].Item != null)
            {
                throw new Exception($"Na bloku [{x},{y}] je již umístěn předmět {Grid[x, y].Item.Name} a nelze na něj umístit {item.Name}!");
            }

            Grid[x, y].Item = item;
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

        /// <summary>
        /// Removes object from map and returns true if the object was found and removed.
        /// Note that in case of players, this will also remove their inventory content.
        /// This method should be used for editor purposes only and not in real game.
        /// </summary>
        /// <param name="uid">Uid of object to be removed.</param>
        /// <returns>True if the object was removed.</returns>
        public bool RemoveObjectFromMapByUid(int uid)
        {
            bool found = false;
            for(int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (Grid[i,j].Occupied && Grid[i,j].Creature.UniqueId == uid)
                    {
                        Grid[i, j].Creature.Position = null;
                        Grid[i, j].Creature = null;
                        found = true;
                        break;
                    }

                    else if (Grid[i,j].Item != null && Grid[i,j].Item.UniqueId == uid)
                    {
                        Grid[i, j].Item.Position = null;
                        Grid[i, j].Item = null;
                        found = true;
                        break;
                    }

                }

                if (found) { break; }
            }

            return found;
        }
    }
}
