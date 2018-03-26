using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;
using GameCore.Objects.Creatures.Actions.Exceptions;

namespace GameCore.Objects.Creatures.Actions
{
    /// <summary>
    /// Makes the actor attack in given direction.
    /// </summary>
    public class Attack : AbstractAction
    {
        /// <summary>
        /// Direction in which the actor attacks.
        /// </summary>
        public Direction Direction {get; set;}


        public override void Execute()
        {
            AbstractCreature actor = Actor;
            MapBlock targetBlock = actor.Position.NextBlock(Direction);

            // check that something to attack exists in this direction
            if (!actor.Position.EntranceInDirection(Direction).IsOpen() || targetBlock == null)
            {
                throw new MapBlockInDirectionNotAccessibleException(actor, Direction);
            }
            if (!targetBlock.Occupied)
            {
                throw new CantAttackInThisDirectionException(actor, Direction);
            }

            // calculate attack
            AbstractCreature targetCreature = targetBlock.Creature;
            double totalAttack = actor.TotalAttack();
            double totalDeffense = targetCreature.TotalDeffense();
            double totalDamage = Math.Max(0, totalAttack - totalDeffense);


            // perform attack
            targetCreature.TakeDamage(totalDamage);
        }
    }
}
