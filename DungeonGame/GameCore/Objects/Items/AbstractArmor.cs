using System.Collections.Generic;
using GameCore.Map;

namespace GameCore.Objects.Items
{
    /// <summary>
    /// Base class for all armors.
    /// </summary>
    public abstract class AbstractArmor : AbstractItem
    {
        /// <summary>
        /// Defense which is added to player's base defense.
        /// Property is mapped to AbstractItem.ItemParameter.
        /// </summary>
        public int Defense {
            get
            {
                return ItemParameter;
            }

            set
            {
                ItemParameter = value;
            }
        }

        public AbstractArmor(string name, MapBlock position, int defense) : base(name, position, AbstractItem.ARMOR_TYPE, defense)
        {
        }
    }
}
