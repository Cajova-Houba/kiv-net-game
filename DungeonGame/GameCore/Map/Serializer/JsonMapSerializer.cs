using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Newtonsoft.Json;

namespace GameCore.Map.Serializer
{
    /// <summary>
    /// Serializes map to json and back.
    /// </summary>
    public class JsonMapSerializer : IMapSerializer<string, string>
    {
        /// <summary>
        /// Serializes Map object as json string.
        /// </summary>
        /// <param name="map">Map to be serialized.</param>
        /// <returns>Json string.</returns>
        public String Serialize(Map map)
        {
            string output = JsonConvert.SerializeObject(map);
            return output;
        }

        /// <summary>
        /// Deserialize map from json string.
        /// </summary>
        /// <param name="jsonString">Json string to be deserialized.</param>
        /// <returns>Deserialized map.</returns>
        public Map Deserialize(string jsonString)
        {
            return JsonConvert.DeserializeObject<Map>(jsonString);
        }
    }
}
