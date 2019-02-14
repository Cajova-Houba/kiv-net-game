using GameCore.Map;

namespace GameCore.Objects.Items
{
    /// <summary>
    /// Base class for all weapons.
    /// </summary>
    public abstract class AbstractWeapon :  AbstractItem
    {

        /// <summary>
        /// Damage this weapon addes to player's base damage.
        /// </summary>
        public int Damage
        {
            get
            {
                return ItemParameter;
            }

            set
            {
                ItemParameter = value;
            }
        }

        public AbstractWeapon(string name, MapBlock position, int damage) : base(name, position, AbstractItem.WEAPON_TYPE, damage)
        {
        }
    }
}
