using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameCore.Map;
using GameCore.Map.Generator;
using GameCore.Map.Serializer;

namespace GameCoreUnitTest
{
    [TestClass]
    public class MapSerializerTest
    {
        /// <summary>
        /// Create simple map and serialize it.
        /// </summary>
        [TestMethod]
        public void TestJsonSerialize()
        {
            Direction[] allDirections = DirectionMethods.GetAllDirections();
            int w = 10;
            int h = 15;
            Map map = new SimpleMapGenerator().GenerateMap(w, h, IMapGeneratorConstants.NO_SEED);
            IMapSerializer<string, string> jsonSerializer = new JsonMapSerializer();
            string jsonString = jsonSerializer.Serialize(map);

            Assert.IsFalse(jsonString.Length == 0, "Empty string returned!");

            Map deserialized = jsonSerializer.Deserialize(jsonString);

            // compare pre-serialization vs post-serialization
            Assert.AreEqual(map.Width, deserialized.Width, "Wrong width!");
            Assert.AreEqual(map.Height, deserialized.Height, "Wrong width!");
            for(int i = 0; i < map.Width; i ++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    MapBlock orig = map.Grid[i, j];
                    MapBlock deser = deserialized.Grid[i, j];

                    Assert.AreEqual(orig, deser, $"Blocks at [{i},{j}] have wrong coordinates!");
                    foreach(Direction d in allDirections)
                    {
                        Assert.AreEqual(orig.EntranceInDirection(d), deser.EntranceInDirection(d), $"Blocks at [{i},{j}] have different entrance in direction {d}!");
                    }
                }
            }
        }

        /// <summary>
        /// Create simple map and serialize it.
        /// </summary>
        [TestMethod]
        public void TestByteSerialize()
        {
            int w = 4;
            int h = 4;
            Map map = MapGeneratorFactory.CreateOpenMapGenerator().GenerateMap(w, h, IMapGeneratorConstants.NO_SEED);
            IMapSerializer<byte[], byte[]> byteSerializer = new BinaryMapSerializer();
            byte[] serializedMap = byteSerializer.Serialize(map);
            // 3 bytes for header, 16 bytes for map header, (w*h) / 2 bytes for map data, 4 bytes for creature count, 4 bytes for item count
            int expectedSize = 3 + 16 + (w * h) / 2 + 4 + 4;

            Assert.AreEqual(expectedSize, serializedMap.Length, "Wrong number of bytes returned!");

            // check header
            Assert.AreEqual(Convert.ToByte('D'), serializedMap[0], "Wrong first byte of the header!");
            Assert.AreEqual(Convert.ToByte('M'), serializedMap[1], "Wrong second byte of the header!");
            Assert.AreEqual(BinaryMapSerializer.VERSION, serializedMap[2], "Wrong third byte of the header!");

            // check map header
            int sw = serializedMap[3] + 256*serializedMap[4] + 256*256*serializedMap[5] + 256*256*256*serializedMap[6];
            int sh = serializedMap[7] + 256 * serializedMap[8] + 256 * 256 * serializedMap[9] + 256 * 256 * 256 * serializedMap[10];
            int wx = serializedMap[11] + 256 * serializedMap[12] + 256 * 256 * serializedMap[13] + 256 * 256 * 256 * serializedMap[14];
            int wy = serializedMap[15] + 256 * serializedMap[16] + 256 * 256 * serializedMap[17] + 256 * 256 * 256 * serializedMap[18];
            Assert.AreEqual(w, sw, "Wrong map width!");
            Assert.AreEqual(h, sh, "Wrong map height!");
            Assert.AreEqual(wx, map.WinningBlock.X, "Wrong x coordinate of winning block!");
            Assert.AreEqual(wy, map.WinningBlock.Y, "Wrong y coordinate of winning block!");

            // check map data
            for(int i = 19; i < 19+8; i++)
            {
                Assert.AreEqual(255, serializedMap[i], $"Wrong {i-18} byte of map data!");
            }

            // check creature and item counts
            Assert.AreEqual(0, serializedMap[33], "Wrong creature count!");
            Assert.AreEqual(0, serializedMap[34], "Wrong item count!");

            // try to deserialize
            Map deserializedMap = byteSerializer.Deserialize(serializedMap);

            // check map 
            Assert.AreEqual(w, deserializedMap.Width, "Wrong width after deserialization!");
            Assert.AreEqual(h, deserializedMap.Width, "Wrong height after deserialization!");
            Assert.AreEqual(map.WinningBlock.X, deserializedMap.WinningBlock.X, "Wrong x coordinate of winning block!");
            Assert.AreEqual(map.WinningBlock.Y, deserializedMap.WinningBlock.Y, "Wrong Y coordinate of winning block!");

            // check map blocks
            for(int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    MapBlock origBlock = map.Grid[i, j];
                    MapBlock testedBlock = deserializedMap.Grid[i, j];

                    Assert.IsTrue(testedBlock.North.IsOpen(), $"North entrance of block [{i},{j}] is not open!");
                    Assert.IsTrue(testedBlock.East.IsOpen(), $"East entrance of block [{i},{j}] is not open!");
                    Assert.IsTrue(testedBlock.South.IsOpen(), $"South entrance of block [{i},{j}] is not open!");
                    Assert.IsTrue(testedBlock.West.IsOpen(), $"West entrance of block [{i},{j}] is not open!");
                }
            }
        }

        /// <summary>
        /// Create simple maze and try serialization / deserialization.
        /// </summary>
        [TestMethod]
        public void TestSerializeMaze()
        {
            int w = 4;
            int h = 4;
            Map map = MapGeneratorFactory.CreateSimpleMapGenerator().GenerateMap(w, h, IMapGeneratorConstants.NO_SEED);
            IMapSerializer<byte[], byte[]> byteSerializer = new BinaryMapSerializer();

            byte[] serializedMap = byteSerializer.Serialize(map);
            Map deserializedMap = byteSerializer.Deserialize(serializedMap);

            // check map 
            Assert.AreEqual(w, deserializedMap.Width, "Wrong width after deserialization!");
            Assert.AreEqual(h, deserializedMap.Width, "Wrong height after deserialization!");
            Assert.AreEqual(map.WinningBlock.X, deserializedMap.WinningBlock.X, "Wrong x coordinate of winning block!");
            Assert.AreEqual(map.WinningBlock.Y, deserializedMap.WinningBlock.Y, "Wrong Y coordinate of winning block!");

            // check map blocks
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    MapBlock origBlock = map.Grid[i, j];
                    MapBlock testedBlock = deserializedMap.Grid[i, j];

                    foreach(Direction dir in  DirectionMethods.GetAllDirections())
                    {
                        Assert.AreEqual(origBlock.EntranceInDirection(dir).IsOpen(), testedBlock.EntranceInDirection(dir).IsOpen(), $"Wrong entrance in direction {dir} in block [{i},{j}].");
                    }
                }
            }
        }
    }
}
