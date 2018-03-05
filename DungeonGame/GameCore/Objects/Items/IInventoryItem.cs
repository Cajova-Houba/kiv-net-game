using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Objects.Items
{
    /// <summary>
    /// Tagging interface for all items which can be placed in player's inventory. 
    /// Those are different from weapons / armors which are placed in separate 'slots'.
    /// </summary>
    public interface IInventoryItem : IItem
    {
    }
}
