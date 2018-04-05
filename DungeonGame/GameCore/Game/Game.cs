﻿using GameCore.Objects.Creatures;
using System.Collections.Generic;

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
        public LinkedList<AbstractPlayer> HumanPlayers { get; protected set; }

        /// <summary>
        /// AI players (same as human but controlled by computer).
        /// </summary>
        public LinkedList<AbstractPlayer> AiPlayers { get; protected set; }

        /// <summary>
        /// Monsters - spiders, rats, ... stupid but possibly deadly to both
        /// human and AI players.
        /// </summary>
        public LinkedList<AbstractCreature> Monsters { get; protected set; }

        /// <summary>
        /// Game map. 
        /// </summary>
        public Map.Map GameMap;

        /// <summary>
        /// Winner of the game.
        /// </summary>
        public AbstractPlayer Winner { get; set; }

        /// <summary>
        /// Returns true if there is a winner. Should be checked after every game loop step.
        /// </summary>
        public bool IsWinner { get { return Winner != null; } }

        /// <summary>
        /// Default constructor. Initializes structures for players and monsters. Map is kept null.
        /// </summary>
        public Game()
        {
            HumanPlayers = new LinkedList<AbstractPlayer>();
            AiPlayers = new LinkedList<AbstractPlayer>();
            Monsters = new LinkedList<AbstractCreature>();
        }
        
        /// <summary>
        /// Adds a human player.
        /// </summary>
        /// <param name="humanPlayer">Human player to be added.</param>
        public void AddHumanPlayer(AbstractPlayer humanPlayer)
        {
            HumanPlayers.AddLast(humanPlayer);
        }

        /// <summary>
        /// Adds AI player.
        /// </summary>
        /// <param name="aiPlayer">AI player to be added.</param>
        public void AddAIPlayer(AbstractPlayer aiPlayer)
        {
            AiPlayers.AddLast(aiPlayer);
        }
        
        /// <summary>
        /// Adds monster.
        /// </summary>
        /// <param name="monster">Monster to be added.</param>
        public void AddMonster(AbstractCreature monster)
        {
            Monsters.AddLast(monster);
        }

        /// <summary>
        /// One cycle of game loop. If some player wins, game loop will exit after winning condition check.
        /// </summary>
        public void GameLoopStep()
        {
            // perform player's action first
            foreach(AbstractPlayer player in HumanPlayers)
            {
                player.NextAction?.Execute();
                if (CheckWinningConditions(player))
                {
                    Winner = player;
                    return;
                }
            }

            // AI 'thinking' and actions
            foreach(AbstractPlayer aiPlayer in AiPlayers)
            {
                aiPlayer.Think();
                aiPlayer.NextAction?.Execute();
                if (CheckWinningConditions(aiPlayer))
                {
                    Winner = aiPlayer;
                    return;
                }
            }

            // monster 'thinking' and acitons
            foreach(AbstractCreature monster in Monsters)
            {
                monster.Think();
                monster.NextAction?.Execute();
            }
        }

        /// <summary>
        /// Check if the player meets winning criteria. Works for both human and AI players.
        /// Current criteria for player to be a winer is to be on the winning block (if it's defined).
        /// </summary>
        /// <param name="player">Player to be checked.</param>
        /// <returns>Returns true if player wins. This doesn't have to mean the game end though.</returns>
        public bool CheckWinningConditions(AbstractPlayer player)
        {
            if (GameMap.WinningBlock == null)
            {
                return false;
            } else
            {
                return GameMap.WinningBlock.Equals(player.Position);
            }
        }
    }
}
