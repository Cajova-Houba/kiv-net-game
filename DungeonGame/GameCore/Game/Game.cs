using GameCore.Map;
using GameCore.Objects.Creatures;
using GameCore.Objects.Items;
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
        public List<AbstractPlayer> HumanPlayers { get; protected set; }

        /// <summary>
        /// AI players (same as human but controlled by computer).
        /// </summary>
        public List<AbstractPlayer> AiPlayers { get; protected set; }

        /// <summary>
        /// Monsters - spiders, rats, ... stupid but possibly deadly to both
        /// human and AI players.
        /// </summary>
        public List<AbstractCreature> Monsters { get; protected set; }

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
            HumanPlayers = new List<AbstractPlayer>();
            AiPlayers = new List<AbstractPlayer>();
            Monsters = new List<AbstractCreature>();
        }
        
        /// <summary>
        /// Adds a human player.
        /// </summary>
        /// <param name="humanPlayer">Human player to be added.</param>
        public void AddHumanPlayer(AbstractPlayer humanPlayer)
        {
            HumanPlayers.Add(humanPlayer);
            GameMap.AddCreature(humanPlayer);
        }

        /// <summary>
        /// Adds AI player.
        /// </summary>
        /// <param name="aiPlayer">AI player to be added.</param>
        public void AddAIPlayer(AbstractPlayer aiPlayer)
        {
            AiPlayers.Add(aiPlayer);
            GameMap.AddCreature(aiPlayer);
        }
        
        /// <summary>
        /// Adds monster.
        /// </summary>
        /// <param name="monster">Monster to be added.</param>
        public void AddMonster(AbstractCreature monster)
        {
            Monsters.Add(monster);
            GameMap.AddCreature(monster);
        }

        /// <summary>
        /// Adds item to the game. Item should have its position set.
        /// </summary>
        /// <param name="item">Item to be added.</param>
        public void AddItem(AbstractItem item)
        {
            MapBlock pos = item.Position;
            if (pos != null && pos.X >=0 && pos.X < GameMap.Width && pos.Y >=0 && pos.Y < GameMap.Height)
            {
                GameMap.Grid[pos.X, pos.Y].Item = item;
            }
        }

        /// <summary>
        /// One cycle of game loop. If some player wins, game loop will exit after winning condition check.
        /// </summary>
        public void GameLoopStep()
        {
            // perform player's action first
            foreach(AbstractPlayer player in HumanPlayers)
            {
                if(!player.Alive)
                {
                    continue;
                }

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
                if (!aiPlayer.Alive)
                {
                    continue;
                }

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
                if(!monster.Alive)
                {
                    continue;
                }

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

        /// <summary>
        /// Method called when player dies. Override it if you want but be sure to call the super method first.
        /// </summary>
        /// <param name="player">Player who just died.</param>
        public void OnPlayerDeath(AbstractPlayer player)
        {

        }
    }
}
