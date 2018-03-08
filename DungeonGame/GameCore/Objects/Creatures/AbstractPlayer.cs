using GameCore.Map;
using GameCore.Objects.Items;
using System.Collections.Generic;

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
        /// Player's inventory. 
        /// </summary>
        public LinkedList<IInventoryItem> Inventory { get; protected set; }
        
        /// <summary>
        /// Armor equipped by player. Null if no armor is equipped.
        /// </summary>
        public AbstractArmor Armor { get; protected set; }

        /// <summary>
        /// Weapon equipped by player. Null if no weapon is equipped.
        /// </summary>
        public AbstractWeapon Weapon { get; protected set; }

        /// <summary>
        /// Constructor which initializes player with name, position and default values.
        /// </summary>
        /// <param name="name">Player's name.</param>
        /// <param name="position">Players position.</param>
        public AbstractPlayer(string name, MapBlock position) : base(name, position, DEFAULT_BASE_HP, DEFAULT_BASE_ATTACK, DEFAULT_BASE_DEFFENSE)
        {
            Inventory = new LinkedList<IInventoryItem>();
        }
    }
}
