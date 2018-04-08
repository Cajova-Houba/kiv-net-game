using GameCore.Objects.Creatures;
using GameCore.Objects.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Game.Actions.Exceptions
{
    /// <summary>
    /// Class thrown when trying to pick up uknown item. Works as a brake when new item is added, but programmer forgets to implement
    /// picking it up resulting in failed tests.
    /// </summary>
    public class PickUpUnknownItemException : Exception
    {
        // <summary>
        /// Actor who tried to perform pick up action.
        /// </summary>
        public AbstractPlayer Actor { get; private set; }

        /// <summary>
        /// Unknown item.
        /// </summary>
        public AbstractItem Item { get; private set; }

        public PickUpUnknownItemException(AbstractPlayer actor, AbstractItem item)
        {
            Actor = actor;
            Item = item;
        }
    }
}
