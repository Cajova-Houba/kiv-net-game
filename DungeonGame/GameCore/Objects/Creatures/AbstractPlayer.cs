using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Objects.Creatures
{
    /// <summary>
    /// Base class for all 'player-like' objects. Those are AI player and human player.
    /// </summary>
    public abstract class AbstractPlayer : AbstractCreature
    {
        /// <summary>
        /// Default base HP for all players.
        /// </summary>
        public const int DEFAULT_BASE_HP = 100;

        /// <summary>
        /// Default base attack for all players.
        /// </summary>
        public const int DEFAULT_BASE_ATTACK = 20;

        /// <summary>
        /// Default base defense for all players.
        /// </summary>
        public const int DEFAULT_BASE_DEFFENSE = 15;

        /// <summary>
        /// Constructor which initializes player with name, position and default values.
        /// </summary>
        /// <param name="name">Player's name.</param>
        /// <param name="position">Players position.</param>
        public AbstractPlayer(string name, MapBlock position) : base(name, position, DEFAULT_BASE_HP, DEFAULT_BASE_ATTACK, DEFAULT_BASE_DEFFENSE)
        {
        }
    }
}
