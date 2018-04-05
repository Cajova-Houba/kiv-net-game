using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameCore.Generator;
using GameCore.Map;
using GameCore.Objects.Creatures;
using GameCore.Objects.Creatures.AIPlayers;
using GameCore.Game;

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
    }
}
