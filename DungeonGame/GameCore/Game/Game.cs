using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Objects.Creatures;
using GameCore.Map;

namespace GameCore.Game
{
    /// <summary>
    /// Main Game class. Contains game loop and game-related data.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Human players present in game.
        /// </summary>
        private LinkedList<AbstractPlayer> humanPlayers;

        /// <summary>
        /// AI players (same as human but controlled by computer).
        /// </summary>
        private LinkedList<AbstractPlayer> aiPlayers;

        /// <summary>
        /// Monsters - spiders, rats, ... stupid but possibly deadly to both
        /// human and AI players.
        /// </summary>
        private LinkedList<AbstractCreature> monsters;

        /// <summary>
        /// Game map. 
        /// </summary>
        private Map.Map gameMap;

        /// <summary>
        /// One cycle of game loop.
        /// </summary>
        public void GameLoopStep()
        {

        }
    }
}
