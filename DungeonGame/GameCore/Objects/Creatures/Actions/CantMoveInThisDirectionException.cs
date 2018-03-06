using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Objects.Creatures.Actions
{
    /// <summary>
    /// An exception thrown when trying to perform Move action in direction which is blocked.
    /// </summary>
    public class CantMoveInThisDirectionException : Exception
    {
        /// <summary>
        /// Actor who tried to perform move action.
        /// </summary>
        public AbstractCreature Actor { get; private set; }

        /// <summary>
        /// Direciton which is blocked.
        /// </summary>
        public Direction Direction { get; private set; }

        /// <summary>
        /// Create new exception with actor and direction.
        /// </summary>
        /// <param name="actor">Actor who tried to perform move action.</param>
        /// <param name="direction">Blocked direction.</param>
        public CantMoveInThisDirectionException(AbstractCreature actor, Direction direction)
        {
            Actor = actor;
            Direction = direction;
        }
    }
}
