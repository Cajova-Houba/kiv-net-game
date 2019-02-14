using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Map.Serializer.Binary
{
    /// <summary>
    /// Common interface for entities which can be serialized by binary serializer.
    /// </summary>
    public interface IBinarySerializable
    {
        /// <summary>
        /// Serializes this entity to byte list.
        /// </summary>
        /// <returns>Byte representation of this entity.</returns>
        List<byte> SerializeBinary();
    }
}
