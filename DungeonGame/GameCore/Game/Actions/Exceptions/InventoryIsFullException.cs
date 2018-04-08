using GameCore.Objects.Creatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Game.Actions.Exceptions
{
    /// <summary>
    /// Exception thrown when trying to add item to full inventory.
    /// </summary>
    public class InventoryIsFullException : Exception
    {
        // <summary>
        /// Actor who tried to perform pick up action.
        /// </summary>
        public AbstractPlayer Actor { get; private set; }

        /// <summary>
        /// Create new exception for player with full inventory.
        /// </summary>
        /// <param name="actor">Player with full inventory.</param>
        public InventoryIsFullException(AbstractPlayer actor)
        {
            Actor = actor;
        }
    }
}
