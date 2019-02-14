using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;
using GameCore.Map.Serializer;
using GameCore.Map.Serializer.Binary;

namespace GameCore.Objects.Items
{

    /// <summary>
    /// Base class for all non-living items.
    /// </summary>
    public abstract class AbstractItem : GameObject
    {
        public const byte WEAPON_TYPE = 0;
        public const byte ARMOR_TYPE = 1;
        public const byte INVENTORY_ITEM_TYPE = 2;

        /// <summary>
        /// Type of item - armor, item, weapon, ...
        /// </summary>
        public byte ItemType { get; set; }

        /// <summary>
        /// Item parameter - defense of armor, value of item, attack of a weapon.
        /// </summary>
        public int ItemParameter { get; set; }

        public AbstractItem(string name, MapBlock position, byte type, int parameter) : base(name, position)
        {
            ItemType = type;
            ItemParameter = parameter;
        }

        public override List<byte> SerializeBinary()
        {
            List<byte> itBytes = new List<byte>();

            // uid
            itBytes.AddRange(BinarySerializerUtils.IntToBytes(UniqueId));

            // name
            itBytes.AddRange(BinarySerializerUtils.StrToBytes(Name, GameObject.MAX_NAME_LENGTH));

            // position
            itBytes.AddRange(BinarySerializerUtils.IntToBytes(Position.X));
            itBytes.AddRange(BinarySerializerUtils.IntToBytes(Position.Y));

            // attributes parameters
            byte type = ItemType;
            int param = ItemParameter;
            itBytes.AddRange(BinarySerializerUtils.IntToBytes(param));
            itBytes.Add(type);

            return itBytes;
        }
    }
}
