using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Objects.Creatures;

namespace GameCore.Game.Actions
{
    /// <summary>
    /// Base class for every action creature (actor) may perform.
    /// Based on the Command pattern.
    /// 
    /// Note: same effect could be accomplished by using lambdas / delegates but I prefer this approach.
    /// </summary>
    public abstract class AbstractAction
    {
        /// <summary>
        /// Creature which performs this action. Extending class should make sure this is always set.
        /// </summary>
        public AbstractCreature Actor { get; set; }

        /// <summary>
        /// Executes this action. 
        /// </summary>
        public abstract void Execute();
    }
}
