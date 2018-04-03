using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Newtonsoft.Json;

namespace GameCore.Map
{
    /// <summary>
    /// Library class to de/serialize Map entity from/to xml file.
    /// </summary>
    public class MapSerializer
    {
        /// <summary>
        /// Serializes Map object as json string.
        /// </summary>
        /// <param name="map">Map to be serialized.</param>
        /// <returns>Json string.</returns>
        public static String Serialize(Map map)
        {
            string output = JsonConvert.SerializeObject(map);
            return output;
        }

        /// <summary>
        /// Deserialize map from json string.
        /// </summary>
        /// <param name="jsonString">Json string to be deserialized.</param>
        /// <returns>Deserialized map.</returns>
        public static Map Deserialize(string jsonString)
        {
            return JsonConvert.DeserializeObject<Map>(jsonString);
        }
    }
}
