using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;
using Newtonsoft.Json;

namespace GameCore.Objects
{
    /// <summary>
    /// Base class for all objects which can be placed on the map - player, monsters, items, ...
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class GameObject
    {
        /// <summary>
        /// Object's displayed name.
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// Object's parent block.
        /// </summary>
        public MapBlock Position { get; set; }

        /// <summary>
        /// Initializes this game object with name and position.
        /// </summary>
        /// <param name="name">Displayed name of this object.</param>
        /// <param name="position">Position of this object.</param>
        public GameObject(string name, MapBlock position)
        {
            Name = name;
            Position = position;
        }
    }
}
