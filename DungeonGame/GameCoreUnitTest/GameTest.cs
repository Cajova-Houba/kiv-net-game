using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameCore.Map.Generator;
using GameCore.Map;
using GameCore.Objects.Creatures;
using GameCore.Objects.Creatures.AIPlayers;
using GameCore.Game;
using GameCore.Objects.Items;
using GameCore.Game.Actions;
using GameCore.Game.Actions.Exceptions;
using GameCore.Objects.Items.Weapons;
using GameCore.Objects.Items.Armors;

namespace GameCoreUnitTest
{
    /// <summary>
    /// Test class for game control mechanincs - game loop, winning condition, ...
    /// </summary>
    [TestClass]
    public class GameTest
    {
        /// <summary>
        /// Generate 1x1 map, place one player and do one step of game loop. Player should win the game.
        /// </summary>
        [TestMethod]
        public void TestWinningCondition()
        {
            // 1x1 map
            int w = 1;
            int h = 1;
            IMapGenerator mapGenerator = MapGeneratorFactory.CreateOpenMapGenerator();
            Map map = mapGenerator.GenerateMap(w, h, IMapGeneratorConstants.NO_SEED);

            // place player
            AbstractPlayer player = AIPlayerFactory.CreateEmptyAIPlayer("Test empty AI", map.Grid[0, 0]);

            // create game instance
            Game game = new Game() { GameMap = map };
            game.AddAIPlayer(player);

            // perform one game loop step
            game.GameLoopStep();

            // check winner
            Assert.IsTrue(game.IsWinner, "No winner after game loop step!");
            Assert.IsNotNull(game.Winner, "Winner is null!");
        }

        /// <summary>
        /// Try to pick up item from the map block.
        /// </summary>
        [TestMethod]
        public void TestPickUpItem()
        {
            // prepare map block with one item, player and pick up action
            MapBlock mapBlock = new MapBlock(0, 0);
            AbstractInventoryItem item = new BasicItem("Test item", mapBlock, 10);
            mapBlock.Item = item;
            AbstractPlayer testPlayer = new EmptyAIPlayer("Test player", mapBlock);
            PickUp pickUpAction = new PickUp() { Actor = testPlayer };

            // check that inventory is empty
            Assert.AreEqual(0, testPlayer.Inventory.Count, "Inventory should be empty!");
            Assert.IsNotNull(mapBlock.Item, "Item should be placed in map block!");

            // perform pick up action
            pickUpAction.Execute();

            // check that item was picked up
            Assert.AreEqual(1, testPlayer.Inventory.Count, "There should be one item in inventory!");
            Assert.IsNull(mapBlock.Item, "There should be no item in the map block!");
            Assert.AreEqual(item, testPlayer.GetItemFromInventory(0), "Wrong item picked up!");
        }

        /// <summary>
        /// Try to pick up item from empty block.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NoItemToPickUpException))]
        public void TestPickUpNoItem()
        {
            // prepare map block, player and pick up action
            MapBlock mapBlock = new MapBlock(0, 0);
            AbstractPlayer testPlayer = new EmptyAIPlayer("Test player", mapBlock);
            PickUp pickUpAction = new PickUp() { Actor = testPlayer };

            // perform pick up action
            pickUpAction.Execute();
        }

        /// <summary>
        /// Try to pick up item from block with full inventory.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InventoryIsFullException))]
        public void TestPickUpFullInventory()
        {
            // prepare map block with one item, player and pick up action
            MapBlock mapBlock = new MapBlock(0, 0);
            AbstractInventoryItem item = new BasicItem("Test item", mapBlock, 10);
            mapBlock.Item = item;
            AbstractPlayer testPlayer = new FullInventoryPlayer("Test player", mapBlock);
            PickUp pickUpAction = new PickUp() { Actor = testPlayer };

            // perform pickup action
            pickUpAction.Execute();
        }

        /// <summary>
        /// Try to pick up weapon. Then check the swapping functionality works.
        /// </summary>
        [TestMethod]
        public void TestPickUpWeapon()
        {
            // prepare map block with one item, player and pick up action
            MapBlock mapBlock = new MapBlock(0, 0);
            AbstractItem sword = new Axe("Test item", mapBlock);
            mapBlock.Item = sword;
            AbstractPlayer testPlayer = new EmptyAIPlayer("Test player", mapBlock);
            PickUp pickUpAction = new PickUp() { Actor = testPlayer };

            // check that player has no weapon
            Assert.IsNull(testPlayer.Weapon, "No weapon should be equipped!");

            // perform pick up action
            pickUpAction.Execute();

            // check that weapon was equipped
            Assert.IsNotNull(testPlayer.Weapon, "Weapon should be equipped!");
            Assert.AreEqual(sword, testPlayer.Weapon, "Wrong weapon picked up");
            Assert.IsNull(mapBlock.Item, "Item was not picked up from map block");

            // add new sword to the map block and check that swapping works
            AbstractItem sword2 = new Axe("Test sword 2", mapBlock);
            mapBlock.Item = sword2;
            pickUpAction.Execute();

            // check swap
            Assert.IsNotNull(testPlayer.Weapon, "Weapon should be equipped!");
            Assert.AreEqual(sword2, testPlayer.Weapon, "Wrong weapon picked up!");
            Assert.IsNotNull(mapBlock.Item, "First sword should be placed back to map block!");
            Assert.AreEqual(sword, mapBlock.Item, "Wrong sword placed in map block!");
        }

        /// <summary>
        /// Try to pick up armor. Then check the swapping functionality works.
        /// </summary>
        [TestMethod]
        public void TestPickUpArmor()
        {
            // prepare map block with one item, player and pick up action
            MapBlock mapBlock = new MapBlock(0, 0);
            AbstractItem armor = new LeatherArmor("Test amor", mapBlock);
            mapBlock.Item = armor;
            AbstractPlayer testPlayer = new EmptyAIPlayer("Test player", mapBlock);
            PickUp pickUpAction = new PickUp() { Actor = testPlayer };

            // check that player has no armor
            Assert.IsNull(testPlayer.Armor, "No armor should be equipped!");

            // perform pick up action
            pickUpAction.Execute();

            // check that armor was equipped
            Assert.IsNotNull(testPlayer.Armor, "Armor should be equipped!");
            Assert.AreEqual(armor, testPlayer.Armor, "Wrong armor picked up");
            Assert.IsNull(mapBlock.Item, "Item was not picked up from map block");

            // add new sword to the map block and check that swapping works
            AbstractItem armor2 = new LeatherArmor("Test armor 2", mapBlock);
            mapBlock.Item = armor2;
            pickUpAction.Execute();

            // check swap
            Assert.IsNotNull(testPlayer.Armor, "Amor should be equipped!");
            Assert.AreEqual(armor2, testPlayer.Armor, "Wrong armor picked up!");
            Assert.IsNotNull(mapBlock.Item, "First armor should be placed back to map block!");
            Assert.AreEqual(armor, mapBlock.Item, "Wrong armor placed in map block!");
        }


        /// <summary>
        /// Helper class with inventory size set to 0.
        /// </summary>
        private class FullInventoryPlayer : EmptyAIPlayer
        {
            public override int InventorySize { get { return 0; } }

            public FullInventoryPlayer(string name, MapBlock position) : base(name, position)
            {
            }


        }
    }
}
