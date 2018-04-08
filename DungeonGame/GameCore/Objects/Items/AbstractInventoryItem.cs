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
        public AbstractInventoryItem(string name, MapBlock position) : base(name, position)
        {
        }
    }
}
