using GameCore.Map;
using GameCore.Objects.Items.Armors;
using GameCore.Objects.Items.Weapons;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Objects.Items
{
    /// <summary>
    /// Factory class for generating items.
    /// </summary>
    public class ItemFactory
    {
        public const int BASE_ITEM_VALUE = 20;

		/// <summary>
        /// Possible names of basic items.
        /// </summary>
        public static readonly string[] randomItemNames = new string[] {
			"Zlatý prsten",
			"Zlatý náhrdelník",
			"Diamantová brož",
			"Měšec stříbra",
			"Stříbrný náramek",
			"Bronzový prstem s rubínem",
			"Zlatý svícen",
			"Stříbrná váza",
			"Stříbrná lžička",
			"Zlaté pečetidlo",
			"Měšec drahokamů"
        };

		public static AbstractWeapon CreateAxe(MapBlock position)
        {
            return new Axe(position);
        }

		public static AbstractArmor CreateLeatherArmor(MapBlock position)
        {
            return new LeatherArmor(position);
        }

		/// <summary>
        /// Creates new pickable item with random name and value.
        /// </summary>
        /// <param name="position">Position of the item</param>
        /// <returns>Item which can be picked up by player.</returns>
		public static AbstractInventoryItem CreateBasicItem(MapBlock position)
        {
            Random r = new Random();
			// value = base +- base/5
            int value = BASE_ITEM_VALUE - (BASE_ITEM_VALUE / 5 + r.Next(2 * BASE_ITEM_VALUE / 5));
            return new BasicItem(randomItemNames[r.Next(randomItemNames.Length)], position, value);
        }

		/// <summary>
        /// Creates random item. 
        /// 1/4 = weapon
        /// 1/4 = armor
        /// 2/4 = item
        /// </summary>
        /// <param name="position">Position of item</param>
        /// <returns></returns>
		public static AbstractItem CreateRandomItem(MapBlock position)
        {
            Random r = new Random((int)(DateTime.Now.Ticks));
            int randVal = r.Next(4);

			switch(randVal)
            {
                case 0:
                    return CreateAxe(position);
                case 1:
                    return CreateLeatherArmor(position);
                default:
                    return CreateBasicItem(position);
            }
        }
		
    }
}
