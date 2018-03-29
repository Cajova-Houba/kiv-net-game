using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameCore.Map;
using GameCore.Objects.Creatures;
using GameCore.Generator;
using GameCore.Objects.Creatures.Actions;
using System.Collections.Generic;

namespace GameCoreUnitTest
{
    [TestClass]
    public class SimpleAIPlayerTest
    {
        /// <summary>
        /// Test that hash function of VisitedBlock works as expected.
        /// </summary>
        [TestMethod]
        public void VisitedBlockTest()
        {
            HashSet<VisitedBlock> set = new HashSet<VisitedBlock>();
            VisitedBlock vb1 = new VisitedBlock() { x = 0, y = 0, state = BlockState.NEW };
            VisitedBlock vb2 = new VisitedBlock() { x = 0, y = 0, state = BlockState.CLOSED };
            set.Add(vb1);

            Assert.AreEqual(vb1.GetHashCode(), vb2.GetHashCode(), "Has codes are not equal!");
            Assert.IsTrue(set.Contains(vb2));
        }

        /// <summary>
        /// Create simple map and test if the AI is able to find its way.
        /// </summary>
        [TestMethod]
        public void DfsAiTest()
        {
            IMapGenerator simpleMapGenerator = new OpenMapGenerator();
            int w = 5;
            int h = 1;
            Map map = simpleMapGenerator.GenerateMap(w, h, IMapGeneratorConstants.NO_SEED);

            AbstractPlayer simpleAIPlayer = new SimpleAIPlayer("Test simple AI player", map.Grid[0, 0]);

            // player should get to the block [4,0] in 4 turns
            for(int i = 0; i < w-1; i ++)
            {
                simpleAIPlayer.Think();

                // check taht correct action was produced
                AbstractAction nextAction = simpleAIPlayer.NextAction;
                Assert.IsNotNull(nextAction, "Next action is nul in "+i+" iteration!");
                Assert.AreEqual(typeof(Move), nextAction.GetType(), "Wrong type of action in "+i+" iteration!");
                Assert.AreEqual(Direction.EAST, ((Move)nextAction).Direction, "Wrong direction in " + i + " iteration!");

                // execute the action and check the position is correct
                nextAction.Execute();
                Assert.AreEqual(0, simpleAIPlayer.Position.Y, "Wrong Y coordinate in " + i + "direction!");
                Assert.AreEqual(i+1, simpleAIPlayer.Position.X, "Wrong X coordinate in " + i + "direction!");
            }
        }
    }
}
