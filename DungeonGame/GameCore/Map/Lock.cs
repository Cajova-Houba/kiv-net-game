using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Map
{
    /// <summary>
    /// Lock which may be used to lock entrance. If the entrance is locked a key with same LockColor is needed to unlock it.
    /// </summary>
    public class Lock
    {
        /// <summary>
        /// Color of this lock.
        /// </summary>
        public LockColor LockColor { get; set; }

        public override bool Equals(object obj)
        {
            var @lock = obj as Lock;
            return @lock != null &&
                   LockColor == @lock.LockColor;
        }

        public override int GetHashCode()
        {
            return -1556827747 + LockColor.GetHashCode();
        }

        public override string ToString()
        {
            return LockColor.ToString();
        }

        
    }

    public enum LockColor
    {
        /// <summary>
        /// Entrance is locked by green lock.
        /// </summary>
        GREEN,

        /// <summary>
        /// Entrance is locked by blue lock.
        /// </summary>
        BLUE,

        /// <summary>
        /// Entrance is locked by black lock.
        /// </summary>
        BLACK,

        /// <summary>
        /// Entrance is locked by red lock.
        /// </summary>
        RED
    }
}
