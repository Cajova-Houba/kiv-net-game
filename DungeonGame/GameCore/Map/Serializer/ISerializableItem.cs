using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Map.Serializer
{
    /// <summary>
    /// Common interface for all serializable items (not game entities, just items).
    /// </summary>
    public  interface ISerializableItem
    {
        int GetItemType();

        int GetItemParameter();
    }
}
