using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Objects.Items
{
    /// <summary>
    /// Base class for all non-living items.
    /// </summary>
    public abstract class AbstractItem : GameObject
    {
        public AbstractItem(string name, MapBlock position) : base(name, position)
        {
        }
    }
}
