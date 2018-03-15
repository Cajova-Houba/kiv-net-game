using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Map;
using GameCore.Generator;
using GameCore.Objects.Creatures;
using GameCore.Objects.Creatures.Actions;

namespace GameCoreUnitTest
{
    /// <summary>
    /// Unit tests for Monster class, especially its AI.
    /// </summary>
    [TestClass]
    public class MonsterTest
    {
        /// <summary>
        /// Create map 5x1 and check that monster will vander around these blocks correctly.
        /// </summary>
        [TestMethod]
        public void SimpleMonsterPathTest()
        {
            int w = 5;
            int h = 1;
            OpenMapGenerator mapGenerator = new OpenMapGenerator();
            Map map = mapGenerator.GenerateMap(w, h, IMapGeneratorConstants.NO_SEED);

            // create monster and let it plan its next action 4 times
            // monster should generate 4 'move to EAST' actions 
            Monster monster = new Monster("Test monster", map.Grid[0, 0], 100, 0, 0,w-1);
            Stack<Direction> dirStack = new Stack<Direction>();
            for(int i = 0; i < w-1; i++)
            {
                monster.Think();
                AbstractAction action = monster.NextAction;
                Assert.IsNotNull(action, $"Action in {i} iteration is null!");
                Assert.AreEqual(typeof(Move), action.GetType(), $"Aciton in {i} iteration is not move!");
                Move moveAction = (Move)action;
                Assert.IsFalse(moveAction.Direction.IsNoDirection(), "Direction in move action is NO_DIRECTION.");
                dirStack.Push(moveAction.Direction);
                moveAction.Execute();
            }

            // think again - monster should stay still now and next action should be null
            monster.Think();
            Assert.IsNull(monster.NextAction, "Next action should be null if the stack is empty!");

            // think again 4 times, monster should now return to its initial position
            for(int i = 0; i < w-1; i++)
            {
                monster.Think();
                AbstractAction action = monster.NextAction;
                Assert.IsNotNull(action, $"Action in {i} iteration is null!");
                Assert.AreEqual(typeof(Move), action.GetType(), $"Aciton in {i} iteration is not move!");
                Move moveAction = (Move)action;
                Assert.AreEqual(dirStack.Pop().OppositeDirection(), moveAction.Direction, $"Action in {i} iteration has wrong direction {moveAction.Direction} when moving backwards");
                moveAction.Execute();
            }

            // check current position of monster, it should be [0,0]
            Assert.AreEqual(0, monster.Position.X, "X coordinate of monster's final position is not correct!");
            Assert.AreEqual(0, monster.Position.Y, "Y coordinate of monster's final position is not correct!");
        }

        /// <summary>
        /// Create map with just one block and check that monster's next action will be always null.
        /// </summary>
        [TestMethod]
        public void NoPathTest()
        {
            // 1x1 map
            int w = 1;
            int h = 1;
            OpenMapGenerator mapGenerator = new OpenMapGenerator();
            Map map = mapGenerator.GenerateMap(w, h, IMapGeneratorConstants.NO_SEED);

            // create monster and let it plan its next action 4 times
            // monster should generate 4 'move to EAST' actions 
            Monster monster = new Monster("Test monster", map.Grid[0, 0], 100, 0, 0, w - 1);
            for(int i = 0; i < 2*(w-1); i++)
            {
                monster.Think();
                AbstractAction action = monster.NextAction;
                Assert.IsNull(action, "Monster's action is not null!");
            }

        }
    }
}
