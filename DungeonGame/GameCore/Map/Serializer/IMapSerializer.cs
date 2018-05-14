using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Map.Serializer
{
    /// <summary>
    /// Common interface for map serializers.
    /// <typeparamref name="T">Type of output object for serialization.</typeparamref>
    /// <typeparamref name="K">Type of input object for deserialization.</typeparamref>
    /// </summary>
    public interface IMapSerializer<T,K>
    {
        /// <summary>
        /// Serialize map and its' game objects.
        /// </summary>
        /// <param name="map">Map to be serialized.</param>
        /// <returns>Serialized map.</returns>
        T Serialize(Map map);

        /// <summary>
        /// Deserialize object back to map. May throw exception is input object is malformed.
        /// </summary>
        /// <param name="input">Input to be deserialized.</param>
        /// <returns>Deserialized map.</returns>
        Map Deserialize(K input);
    }
}
