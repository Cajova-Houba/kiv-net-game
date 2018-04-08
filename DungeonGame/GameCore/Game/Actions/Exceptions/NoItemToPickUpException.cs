using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Objects.Creatures;

namespace GameCore.Game.Actions.Exceptions
{
    /// <summary>
    /// Exception thrown when trying to pick up item from empty (or null) map block.
    /// </summary>
    public class NoItemToPickUpException : Exception
    {
        /// <summary>
        /// Actor who tried to perform pick up action.
        /// </summary>
        public AbstractPlayer Actor { get; private set; }

        /// <summary>
        /// Creates new exception with actor who tried to pick up item.
        /// </summary>
        /// <param name="actor">Actor who tried to perform pick up action.</param>
        public NoItemToPickUpException(AbstractPlayer actor)
        {
            Actor = actor;
        }
    }
}
