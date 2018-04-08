using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameCore.Map;
using GameCore.Map.Generator;

namespace GameCoreUnitTest
{
    [TestClass]
    public class MapSerializerTest
    {
        /// <summary>
        /// Create simple map and save it to xml file.
        /// </summary>
        [TestMethod]
        public void TestSerialize()
        {
            Direction[] allDirections = DirectionMethods.GetAllDirections();
            int w = 10;
            int h = 15;
            Map map = new SimpleMapGenerator().GenerateMap(w, h, IMapGeneratorConstants.NO_SEED);
            string jsonString = MapSerializer.Serialize(map);

            Assert.IsFalse(jsonString.Length == 0, "Empty string returned!");

            Map deserialized = MapSerializer.Deserialize(jsonString);

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
    }
}
