using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameCore.Map;
using GameCore.Map.Generator;
using GameCore.Map.Serializer;
using GameCore.Objects.Creatures;
using GameCore.Objects.Creatures.AIPlayers;
using GameCore.Objects.Items;

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

            // add creatures to map
            Monster origMonster = new Monster("Test monster", map.Grid[0, 0], 4, 3654123, 87621235);
            map.AddCreature(origMonster);
            SimpleAIPlayer aiPlayer = new SimpleAIPlayer("Test player", map.Grid[3, 2]);
            map.AddCreature(aiPlayer);
            HumanPlayer hPlayer = new HumanPlayer("Příliš žluťoučký kůň úpěl ďábelské ódy", map.Grid[1, 3]) { BaseHitPoints = 98432156, BaseAttack = 112348, BaseDeffense = 41226987 };
            map.AddCreature(hPlayer);

            // add items to map
            AbstractWeapon weapon = ItemFactory.CreateAxe(map.Grid[1, 3]);
            map.AddItem(weapon);
            AbstractArmor armor = ItemFactory.CreateLeatherArmor(map.Grid[1, 1]);
            map.AddItem(armor);
            AbstractInventoryItem item = new BasicItem("Příliš žluťoučký kůň úpěl ďábelské ódy.!?_/()':123456789<>&@{}[]", map.Grid[2, 2], 514) { UniqueId = 6284 };
            map.AddItem(item);


            // serialize - deserialize
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


            // check creatures
            Monster m = (Monster)deserializedMap.Grid[0, 0].Creature;
            CheckCreature(origMonster, m);

            SimpleAIPlayer p = (SimpleAIPlayer)deserializedMap.Grid[3, 2].Creature;
            CheckCreature(aiPlayer, p);

            HumanPlayer hp = (HumanPlayer)deserializedMap.Grid[1, 3].Creature;
            CheckCreature(hPlayer, hp);


            // check items
            AbstractWeapon weap = (AbstractWeapon)map.Grid[1, 3].Item;
            CheckItem(weap, weapon);

            AbstractArmor arm = (AbstractArmor)map.Grid[1, 1].Item;
            CheckItem(arm, armor);

            AbstractInventoryItem itm = (AbstractInventoryItem)map.Grid[2, 2].Item;
            CheckItem(item, itm);
        }

        private void CheckItem(AbstractItem expected, AbstractItem actual)
        {
            Assert.IsNotNull(actual, "Actual item is null");
            Assert.AreEqual(expected.Name, actual.Name, "Wrong creature name!");
            Assert.AreEqual(expected.UniqueId, actual.UniqueId, "Wrong creature uid!");
            if (actual is AbstractWeapon)
            {
                Assert.IsTrue(expected is AbstractWeapon, "Actual item is not a weapon!");
                Assert.AreEqual(((AbstractWeapon)expected).Damage, ((AbstractWeapon)actual).Damage, "Actual item does not have correct damage!");
            }

            else if (actual is AbstractArmor)
            {
                Assert.IsTrue(expected is AbstractArmor, "Actual item is not an armor!");
                Assert.AreEqual(((AbstractArmor)expected).Defense, ((AbstractArmor)actual).Defense, "Actual item does not have correct deffense!");
            }

            else if (actual is AbstractInventoryItem)
            {
                Assert.IsTrue(expected is AbstractInventoryItem, "Actual item is not an inventory item!");
                Assert.AreEqual(((AbstractInventoryItem)expected).ItemValue, ((AbstractInventoryItem)actual).ItemValue, "Actual item does not have correct item value!");
            }
        }

        private void CheckCreature(AbstractCreature expected, AbstractCreature actual)
        {
            Assert.IsNotNull(actual, "Actual creature is null!");
            Assert.AreEqual(expected.Name, actual.Name, "Wrong creature name!");
            Assert.AreEqual(expected.UniqueId, actual.UniqueId, "Wrong creature uid!");
            Assert.AreEqual(expected.BaseHitPoints, actual.BaseHitPoints, "Wrong creature hp!");
            Assert.AreEqual(expected.BaseAttack, actual.BaseAttack, "Wrong creature attack!");
            Assert.AreEqual(expected.BaseDeffense, actual.BaseDeffense, "Wrong creature deffense!");
        }
    }
}
