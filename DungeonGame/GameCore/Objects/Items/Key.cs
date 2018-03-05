using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Objects.Items
{
    /// <summary>
    /// Key which can be used to open lock of a certain color.
    /// </summary>
    public class Key : GameObject, AbstractInventoryItem
    {
        /// <summary>
        /// Color this key can open.
        /// </summary>
        public LockColor KeyColor { get; protected set; }

        /// <summary>
        /// Initializes this key with position and color.
        /// Key [keyColor] is used as name.
        /// 
        /// </summary>
        /// <param name="keyColor">Color of this key.</param>
        /// <param name="position">Position of this key.</param>
        public Key(LockColor keyColor, MapBlock position) : base($"Key [{keyColor}]", position)
        {
            KeyColor = keyColor;
        }

        /// <summary>
        /// Checks if the entrance can be opened by this key.
        /// If the entrance is NONEXISTENT or colors don't match, false is returned, otherwise true is returned.
        /// </summary>
        /// <param name="entrance">Entrance to be checked.</param>
        /// <returns>True if the lock on the provided entrance is unlockable by this key.</returns>
        public Boolean CanUnlock(Entrance entrance)
        {
            if (entrance.IsLocked())
            {
                return entrance.EntranceLock.LockColor == KeyColor;
            } else if (!entrance.Exists())
            {
                return false;
            }

            // entrance is open
            return true;
        }
    }
}
