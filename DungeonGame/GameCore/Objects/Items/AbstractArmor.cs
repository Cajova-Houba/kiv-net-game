using GameCore.Map;

namespace GameCore.Objects.Items
{
    /// <summary>
    /// Base class for all armors.
    /// </summary>
    public abstract class AbstractArmor : GameObject, IItem
    {
        /// <summary>
        /// Defense which is added to player's base defense.
        /// </summary>
        public int Defense { get; protected set; }

        public AbstractArmor(string name, MapBlock position, int defense) : base(name, position)
        {
            Defense = defense;
        }
    }
}
