using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Objects.Items
{
    /// <summary>
    /// Base class for all weapons.
    /// </summary>
    public abstract class AbstractWeapon : GameObject, IItem
    {

        /// <summary>
        /// Damage this weapon addes to player's base damage.
        /// </summary>
        public int Damage { get; protected set; }

        public AbstractWeapon(string name, MapBlock position, int damage) : base(name, position)
        {
            Damage = damage;
        }
    }
}
