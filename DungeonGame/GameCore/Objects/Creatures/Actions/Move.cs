using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Objects.Creatures.Actions
{
    /// <summary>
    /// Moves the actor in given direction.
    /// </summary>
    public class Move : AbstractAction
    {
        public Direction Direction { get; set; }

        public override void Execute()
        {
            AbstractCreature actor = Actor;
            MapBlock targetBlock = actor.Position.NextBlock(Direction);

            // check that actor can mvoe in this way
            if (!actor.Position.EntranceInDirection(Direction).IsOpen() || targetBlock == null || targetBlock.Occupied)
            {
                throw new CantMoveInThisDirectionException(actor, Direction);
            }

            // perform move
            actor.MoveTo(targetBlock);
        }
    }
}
