using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Objects.Creatures.Actions.Exceptions
{
    /// <summary>
    /// An exception thrown when trying to access block in direction which either doesn't exist, is blocked or is occupied by some other creature.
    /// </summary>
    public class MapBlockInDirectionNotAccessibleException : Exception
    {
        /// <summary>
        /// Actor who tried to access block in some direction.
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
        public MapBlockInDirectionNotAccessibleException(AbstractCreature actor, Direction direction)
        {
            Actor = actor;
            Direction = direction;
        }
    }
}
