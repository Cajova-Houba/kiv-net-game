using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Objects.Items
{
    /// <summary>
    /// Basic item which can be picked up and placed to inventory.
    /// </summary>
    public class BasicItem : AbstractInventoryItem
    {

        /// <summary>
        /// Initializes this basic item with name, position and value.
        /// Item is played into the map block in this constructor.
        /// 
        /// </summary>
        /// <param name="name">Displayed name.</param>
        /// <param name="position">Map block this item lies in.</param>
        /// <param name="itemValue">value of this item used in score calculations.</param>
        public BasicItem(string name, MapBlock position, int itemValue) : base(name, position)
        {
            ItemValue = itemValue;
        }

        public override string ToString()
        {
            return $"BasicItem [name={Name}, itemValue={ItemValue}]";
        }

        public override bool Equals(object obj)
        {
            var item = obj as BasicItem;
            return item != null &&
                   ItemValue == item.ItemValue &&
                   Name == item.Name;
        }

        public override int GetHashCode()
        {
            var hashCode = 1091982821;
            hashCode = hashCode * -1521134295 + ItemValue.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }
    }
}
