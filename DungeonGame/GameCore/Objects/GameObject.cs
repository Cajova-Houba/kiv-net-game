using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;
using GameCore.Map.Serializer;
using GameCore.Map.Serializer.Binary;
using Newtonsoft.Json;

namespace GameCore.Objects
{
    /// <summary>
    /// Base class for all objects which can be placed on the map - player, monsters, items, ...
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class GameObject : IBinarySerializable
    {
        /// <summary>
        /// Max length of object name.
        /// </summary>
        public const int MAX_NAME_LENGTH = 100;

        /// <summary>
        /// Id counter used to give unique id to every object in the game.
        /// </summary>
        protected static int idCounter = 0;

        /// <summary>
        /// Returns unique id for game object.
        /// </summary>
        /// <returns>Unique game id.</returns>
        protected static int GetUID()
        {
            idCounter++;
            return idCounter;
        }

        public abstract List<byte> SerializeBinary();

        /// <summary>
        /// Unique id of this object.
        /// </summary>
        public int UniqueId { get; set; }

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
            UniqueId = GameObject.GetUID();
        }
    }
}
