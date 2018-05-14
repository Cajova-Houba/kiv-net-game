﻿using GameCore.Objects.Creatures;
using GameCore.Objects.Creatures.AIPlayers;
using GameCore.Objects.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameCore.Map.Serializer
{
    /// <summary>
    /// Serializes to map to byte array so that it can be saved to binary file later.
    /// 
    /// If the version doesn't match during deserialization, exception is thrown.
    /// </summary>
    public class BinaryMapSerializer : IMapSerializer<byte[], byte[]>
    {
        /// <summary>
        /// First two bytes of file.
        /// </summary>
        public static readonly byte[] PREFIX = new byte[] { Convert.ToByte('D'), Convert.ToByte('M') };

        /// <summary>
        /// Supported version of binary data structure.
        /// </summary>
        public const byte VERSION = 1;

        public Map Deserialize(byte[] input)
        {
            Stream byteStream = new MemoryStream(input);
            ReadHeader(byteStream);

            Map map = new Map();
            ReadMap(byteStream, map);

            return map;
        }

        public byte[] Serialize(Map map)
        {
            List<byte> serializedMap = new List<byte>();

            AddHeader(map, serializedMap);
            AddMapHeader(map, serializedMap);
            AddMapData(map, serializedMap);
            AddGameObjects(map, serializedMap);

            return serializedMap.ToArray();
        }

        /// <summary>
        /// Reads header from stream. Throws exception if the header is bad or version is incorrect.
        /// </summary>
        /// <param name="byteStream">Stream to read bytes from.</param>
        private void ReadHeader(Stream byteStream)
        {
            byte[] expectedHeader = new byte[] { PREFIX[0], PREFIX[1], VERSION };
            for(int i = 0; i < expectedHeader.Length; i++)
            {
                if (byteStream.ReadByte() != expectedHeader[i])
                {
                    throw new Exception($"Wrong {i} byte of header!");
                }
            }
        }

        /// <summary>
        /// Reads map header and data from byte stream.
        /// </summary>
        /// <param name="byteStream">Byte stream with data.</param>
        /// <param name="map">Map object to write into.</param>
        private void ReadMap(Stream byteStream, Map map)
        {
            int w = ByteToInt(byteStream);
            int h = ByteToInt(byteStream);
            int wx = ByteToInt(byteStream);
            int wy = ByteToInt(byteStream);

            map.InitializeMap(ReadMapBlocks(byteStream, w, h));
            map.WinningBlock = map.Grid[wx, wy];

            List<AbstractCreature> creatures = ReadCreatures(byteStream);
            List<AbstractItem> items = ReadItems(byteStream);
        }

        /// <summary>
        /// Reads grid of map blocks from stream.
        /// </summary>
        /// <param name="byteStream">Stream to read from.</param>
        /// <param name="w">Width of the grid.</param>
        /// <param name="h">Height of the grid.</param>
        /// <returns>Grid with map blocks.</returns>
        private MapBlock[,] ReadMapBlocks(Stream byteStream, int w, int h)
        {
            MapBlock[,] blocks = new MapBlock[w, h];
            int i = 0;
            int j = 0;

            byte blockByte = 0;
            bool upper = false;
            while((i*j) <= (w-1)*(h-1))
            {
                blocks[i, j] = new MapBlock(i, j);
                byte mask = (byte)(1 << 4);

                // read byte only every second iteration
                if (!upper)
                {
                    blockByte = (byte)byteStream.ReadByte();
                    mask = 1;
                }

                byte res = (byte)(blockByte & mask);
                res = (byte)(blockByte & (mask << 1));
                res = (byte)(blockByte & (mask << 2));
                res = (byte)(blockByte & (mask << 3));
                if ((blockByte & mask) == mask) { blocks[i, j].CreateEntrance(Direction.NORTH); }
                if ((blockByte & (mask << 1)) == (mask << 1)) { blocks[i, j].CreateEntrance(Direction.EAST); }
                if ((blockByte & (mask << 2)) == (mask << 2)) { blocks[i, j].CreateEntrance(Direction.SOUTH); }
                if ((blockByte & (mask << 3)) == (mask << 3)) { blocks[i, j].CreateEntrance(Direction.WEST); }

                upper = !upper;
                j++;
                if (j == h )
                {
                    j = 0;
                    i++;
                    if (i == w)
                    {
                        break;
                    }
                }
            }

            return blocks;
        }

        /// <summary>
        /// Read creatures from byte stream.
        /// </summary>
        /// <param name="byteStream">Byte stream to read creatures from.</param>
        /// <returns>Creatures.</returns>
        private List<AbstractCreature> ReadCreatures(Stream byteStream)
        {
            int creatureCount = ByteToInt(byteStream);
            List<AbstractCreature> creatures = new List<AbstractCreature>();

            return creatures;
        }

        /// <summary>
        /// Read items from byte stream.
        /// </summary>
        /// <param name="byteStream">Byte stream to read items from.</param>
        /// <returns>Items.</returns>
        private List<AbstractItem> ReadItems(Stream byteStream)
        {
            int itemCount = ByteToInt(byteStream);
            List<AbstractItem> items = new List<AbstractItem>();

            return items;
        }

        /// <summary>
        /// Adds bytes with file header to list of bytes.
        /// </summary>
        /// <param name="map">Map which is being serialized.</param>
        /// <param name="serialized">List with serialized data.</param>
        private void AddHeader(Map map, List<byte> serialized)
        {
            serialized.AddRange(PREFIX);
            serialized.Add(VERSION);
        }

        /// <summary>
        /// Adds header for map data.
        /// </summary>
        /// <param name="map">Map which is being serialized.</param>
        /// <param name="serialized">List with serialized data.</param>
        private void AddMapHeader(Map map, List<byte> serialized)
        {
            serialized.AddRange(IntToBytes(map.Width));
            serialized.AddRange(IntToBytes(map.Height));
            serialized.AddRange(IntToBytes(map.WinningBlock.X));
            serialized.AddRange(IntToBytes(map.WinningBlock.Y));
        }

        /// <summary>
        /// Adds map blocks. Each block is represented as 4 bits: [N,E,S,W]. 1 means entrance, 0 means wall.
        /// </summary>
        /// <param name="map">Map which is being serialized.</param>
        /// <param name="serialized">List with serialized data.</param>
        private void AddMapData(Map map, List<byte> serialized)
        {
            bool upper = false;
            byte source = 0;
            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    ConvertMapBlockToByte(map.Grid[i, j], upper, source, out source);
                    upper = !upper;

                    // upper is false after negation => two map blocks were processed already
                    if (!upper)
                    {
                        serialized.Add(source);
                        source = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Converts map block to 4 bits and stores in lower or upper part of source byte.
        /// </summary>
        /// <param name="mapBlock">Map block to be converted.</param>
        /// <param name="upper">If true upper part of byte will be used. The other part will remain intact.</param>
        /// <param name="source">Byte with source data.</param>
        /// <param name="res">Byte used for output.</param>
        private void ConvertMapBlockToByte(MapBlock mapBlock, bool upper, byte source, out byte res)
        {
            byte tmp = 0;
            byte mask = 1;

            if (upper)
            {
                mask = (byte)(mask << 4);
            }

            if (mapBlock.North.IsOpen()) { tmp = (byte)(tmp | mask); }
            if (mapBlock.East.IsOpen()) { tmp = (byte)(tmp | (mask << 1)); }
            if (mapBlock.South.IsOpen()) { tmp = (byte)(tmp | (mask << 2)); }
            if (mapBlock.West.IsOpen()) { tmp = (byte)(tmp | (mask << 3)); }

            res = (byte)(source | tmp);
        }

        /// <summary>
        /// Adds headers and data fro game objects.
        /// </summary>
        /// <param name="map">Map which is begin serialized.</param>
        /// <param name="serialized">List with serialized data.</param>
        private void AddGameObjects(Map map, List<byte> serialized)
        {
            List<AbstractCreature> creatures = new List<AbstractCreature>();
            List<AbstractItem> items = new List<AbstractItem>();

            foreach (MapBlock mapBlock in map.Grid)
            {
                if (mapBlock.Occupied)
                {
                    creatures.Add(mapBlock.Creature);
                }

                if (mapBlock.Item != null)
                {
                    items.Add(mapBlock.Item);
                }
            }

            AddCreatures(creatures, serialized);
            AddItems(items, serialized);
        }

        /// <summary>
        /// Adds header data for creatures, then creature data itself.
        /// </summary>
        /// <param name="creatures">Creatures to be serialized.</param>
        /// <param name="serialized">List with serialized data.</param>
        private void AddCreatures(List<AbstractCreature> creatures, List<byte> serialized)
        {
            serialized.AddRange(IntToBytes(creatures.Count));
            creatures.ForEach(creature => serialized.AddRange(CreatureToBytes(creature)));
        }

        /// <summary>
        /// Converts creature to list of bytes.
        /// </summary>
        /// <param name="creature">Creature to be created.</param>
        /// <returns></returns>
        private List<byte> CreatureToBytes(AbstractCreature creature)
        {
            List<byte> crBytes = new List<byte>();

            // uid
            crBytes.AddRange(IntToBytes(creature.UniqueId));

            // name
            byte[] encodedName = Encoding.UTF8.GetBytes(creature.Name);
            crBytes.AddRange(IntToBytes(encodedName.Length));
            crBytes.AddRange(encodedName);

            // position
            crBytes.AddRange(IntToBytes(creature.Position.X));
            crBytes.AddRange(IntToBytes(creature.Position.Y));

            // creature type and attributes
            byte type = 255;
            if (creature is Monster) { type = 0; }
            else if (creature is HumanPlayer) { type = 1; }
            else if (creature is EmptyAIPlayer) { type = 2; }
            else if (creature is SimpleAIPlayer) { type = 3; }
            crBytes.Add(type);
            crBytes.AddRange(IntToBytes(creature.BaseHitPoints));
            crBytes.AddRange(IntToBytes(creature.BaseAttack));
            crBytes.AddRange(IntToBytes(creature.BaseDeffense));

            return crBytes;
        }

        /// <summary>
        /// Adds header data for item, then item data itself.
        /// </summary>
        /// <param name="map">Map which is being serialized.</param>
        /// <param name="serialized">List with serialized data.</param>
        private void AddItems(List<AbstractItem> items, List<byte> serialized)
        {
            serialized.AddRange(IntToBytes(items.Count));
            items.ForEach(item => serialized.AddRange(ItemToByte(item)));
        }

        /// <summary>
        /// Converts item to list of bytes.
        /// </summary>
        /// <param name="item">Item to be converted.</param>
        /// <returns>Item converted to list of bytes.</returns>
        private List<byte> ItemToByte(AbstractItem item)
        {
            List<byte> itBytes = new List<byte>();

            // uid
            itBytes.AddRange(IntToBytes(item.UniqueId));

            // name
            byte[] encodedName = Encoding.UTF8.GetBytes(item.Name);
            itBytes.AddRange(IntToBytes(encodedName.Length));
            itBytes.AddRange(encodedName);

            // position
            itBytes.AddRange(IntToBytes(item.Position.X));
            itBytes.AddRange(IntToBytes(item.Position.Y));

            // type and attributes
            byte type = 255;
            int param = 0;
            if (item is AbstractWeapon) { type = 0; param = ((AbstractWeapon)item).Damage; }
            else if (item is AbstractArmor) { type = 1; param = ((AbstractArmor)item).Defense; }
            else if (item is AbstractInventoryItem) { type = 2; param = ((AbstractInventoryItem)item).ItemValue; }
            itBytes.Add(type);
            itBytes.AddRange(IntToBytes(param));

            return itBytes;
        }

        /// <summary>
        /// Converts integer value to byte array of length 4 using little endian format:
        ///            0     1   2   3
        /// int 4 =   [4]   [0] [0] [0]
        /// int 256 = [0]   [1] [0] [0]
        /// int 511 = [255] [1] [0] [0]
        /// 
        /// </summary>
        /// <param name="intVal"></param>
        /// <returns></returns>
        private byte[] IntToBytes(int intVal)
        {
            return new byte[]
            {
                (byte)(intVal & 255),
                (byte)((intVal >> 8) & 255),
                (byte)((intVal >> 16) & 255),
                (byte)((intVal >> 24) & 255)
            };
        }

        /// <summary>
        /// Reads four bytes from stream and converts them to int.
        /// Little endian.
        /// </summary>
        /// <param name="byteStrem">Stream to read from.</param>
        /// <returns>Result.</returns>
        private int ByteToInt(Stream byteStrem)
        {
            byte b0 = (byte)byteStrem.ReadByte();
            byte b1 = (byte)byteStrem.ReadByte();
            byte b2 = (byte)byteStrem.ReadByte();
            byte b3 = (byte)byteStrem.ReadByte();

            return (((((((((0 | b3 << 8) | b2) << 8) | b1) << 8) | b0))));
        }

        /// <summary>
        /// Convers array of four bytes to int.
        /// Little endian.
        /// </summary>
        /// <param name="bytes">Bytes to be converted.</param>
        /// <returns>Result.</returns>
        private int ByteToInt(byte[] bytes)
        {
            int intVal = 0;

            intVal = (((((((intVal | bytes[3]) << 8) | bytes[2]) << 8) | bytes[1]) << 8) | bytes[0]);

            return intVal;
        }
    }
}
