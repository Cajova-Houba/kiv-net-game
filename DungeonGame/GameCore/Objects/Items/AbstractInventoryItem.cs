using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Objects.Items
{
    /// <summary>
    /// Base class for all items which can be placed into player's inventory.
    /// </summary>
    public abstract class AbstractInventoryItem : AbstractItem
    {
        /// <summary>
        /// Value of this utem, used for score calculations.
        /// 0 by default.
        /// </summary>
        public int ItemValue { get; set; }

        public AbstractInventoryItem(string name, MapBlock position) : base(name, position)
        {
            ItemValue = 0;
        }
    }
}
