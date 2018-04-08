using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;
using GameCore.Objects.Creatures;

namespace GameCore.Game.Actions.Exceptions
{
    /// <summary>
    /// Exception thrown when actor tries to attack in direction in which there is nothing to attack.
    /// </summary>
    public class CantAttackInThisDirectionException : Exception
    {
        /// <summary>
        /// Actor who tried to perform attack action.
        /// </summary>
        public AbstractCreature Actor { get; private set; }

        /// <summary>
        /// Direciton which is blocked or not occupied.
        /// </summary>
        public Direction Direction { get; private set; }

        /// <summary>
        /// Create new exception with actor and direction.
        /// </summary>
        /// <param name="actor">Actor who tried to perform move action.</param>
        /// <param name="direction">Blocked direction.</param>
        public CantAttackInThisDirectionException(AbstractCreature actor, Direction direction)
        {
            Actor = actor;
            Direction = direction;
        }
    }
}
