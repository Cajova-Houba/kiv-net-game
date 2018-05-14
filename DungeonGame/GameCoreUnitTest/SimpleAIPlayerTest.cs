using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameCore.Map;
using GameCore.Objects.Creatures;
using GameCore.Objects.Creatures.AIPlayers;
using GameCore.Map.Generator;
using System.Collections.Generic;
using GameCore.Game.Actions;

namespace GameCoreUnitTest
{
    [TestClass]
    public class SimpleAIPlayerTest
    {
        /// <summary>
        /// Test that hash function of VisitedBlock works as expected.
        /// </summary>
        [TestMethod]
        public void TestVisitedBlock()
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
        public void TestDfsAi()
        {
            IMapGenerator simpleMapGenerator = new OpenMapGenerator();
            int w = 5;
            int h = 1;
            Map map = simpleMapGenerator.GenerateMap(w, h, IMapGeneratorConstants.NO_SEED);

            AbstractPlayer simpleAIPlayer = new SimpleAIPlayer("Test simple AI player", map.Grid[0, 0]) { IgnoreSpeed = true };
            map.AddCreature(simpleAIPlayer);

            // player should get to the block [4,0] in 4 turns
            for(int i = 0; i < w-1; i ++)
            {
                simpleAIPlayer.Think();

                // check taht correct action was produced
                AbstractAction nextAction = simpleAIPlayer.NextAction;
                Assert.IsNotNull(nextAction, $"Next action is nul in {i} iteration!");
                Assert.AreEqual(typeof(Move), nextAction.GetType(), $"Wrong type of action in {i} iteration!");
                Assert.AreEqual(Direction.EAST, ((Move)nextAction).Direction, $"Wrong direction in {i} iteration!");

                // execute the action and check the position is correct
                nextAction.Execute();
                Assert.AreEqual(0, simpleAIPlayer.Position.Y, $"Wrong Y coordinate in {i} direction!");
                Assert.AreEqual(i+1, simpleAIPlayer.Position.X, $"Wrong X coordinate in {i} direction!");
            }
        }

        /// <summary>
        /// Create 2x1 map and place SimpleAI next to Monster. SimpleAI should not move.
        /// </summary>
        [TestMethod]
        public void TestOccupiedBlock()
        {
            IMapGenerator openMapGenerator = new OpenMapGenerator();
            int w = 2;
            int h = 1;
            Map map = openMapGenerator.GenerateMap(w, h, IMapGeneratorConstants.NO_SEED);

            AbstractPlayer simpleAIPlayer = new SimpleAIPlayer("Test simple AI player", map.Grid[0, 0]) { IgnoreSpeed = true };
            map.AddCreature(simpleAIPlayer);
            Monster monster = new Monster("Test monster", map.Grid[1, 0], 10, 10, 10);
            map.AddCreature(monster);

            simpleAIPlayer.Think();

            // check that next action is null
            Assert.IsNull(simpleAIPlayer.NextAction, "Next action should be null!");
        }


        /// <summary>
        /// Create maze and let the AI find path to some block.
        /// AI should be able to do it in fixed number of iterations.
        /// </summary>
        [TestMethod]
        public void TestDfsMaze()
        {
            int w = 50;
            int h = 50;
            int maxIter = 2*(50*50)+1;
            IMapGenerator simpleMapGenerator = new SimpleMapGenerator();
            Map map = simpleMapGenerator.GenerateMap(w, h, IMapGeneratorConstants.NO_SEED);
            AbstractPlayer simpleAIPlayer = new SimpleAIPlayer("Simple maze solver", map.Grid[0, 0]) { IgnoreSpeed = true };
            map.AddCreature(simpleAIPlayer);
            MapBlock finish = map.Grid[w - 1, h - 1];
            bool finishReached = false;
            int iter = 0;
            int nullActions = 0;

            while(!finishReached && iter < maxIter)
            {
                simpleAIPlayer.Think();

                AbstractAction action = simpleAIPlayer.NextAction;
                if (action == null)
                {
                    nullActions++;
                } else
                {
                    Assert.AreEqual(typeof(Move), action.GetType(), $"Wrong type of action in {iter} iteration!");
                    ((Move)action).Execute();

                    finishReached = finish.Equals(simpleAIPlayer.Position);
                }

                iter++;
            }

            Assert.IsTrue(finishReached, $"Finish not reached in {maxIter} iterations! Null actions: {nullActions}.");
        }

    }
}
