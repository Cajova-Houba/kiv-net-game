using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.map
{
    /// <summary>
    /// Thic class represents entrance to the MapBlock.
    /// </summary>
    public class Entrance
    {
        public EntranceState State { get; protected set; }

        /// <summary>
        /// Initializes this entrance with NONEXISTENT state.
        /// </summary>
        public Entrance()
        {
            State = EntranceState.NONEXISTENT;
        }

        /// <summary>
        /// Method used to check if the entrance is open.
        /// </summary>
        /// <returns>Returns true if the entrance state is OPEN.</returns>
        public Boolean IsOpen()
        {
            return State == EntranceState.OPEN;
        }

        /// <summary>
        /// Method used to check if the entrance exists (even a locked one).
        /// </summary>
        /// <returns>True if the entrance state is not NONEXISTENT.</returns>
        public Boolean Exists()
        {
            return State != EntranceState.NONEXISTENT;
        }

        /// <summary>
        /// Marks this entrance as opened. If the current state is NONEXISTENT, exception is raised.
        /// </summary>
        public void Open()
        {
            if(State == EntranceState.NONEXISTENT)
            {
                throw new Exception($"Entrance with {State} state can't be opened!");
            }

            State = EntranceState.OPEN;
        }
    }

    /// <summary>
    /// This enum represents state of one entrance to the MapBlock. 
    /// The entrace can be ither nonexistent, open or locked by a gate of ceratain color.
    /// </summary>
    public enum EntranceState
    {
        /// <summary>
        /// This entrance doesn't exist.
        /// </summary>
        NONEXISTENT,

        /// <summary>
        /// Open entrance = open gate or hole in a wall.
        /// Entrace state should be switched to OPEN after it was unlocked.
        /// </summary>
        OPEN,

        /// <summary>
        /// Entrance is locked by green gate.
        /// </summary>
        GREEN,

        /// <summary>
        /// Entrance is locked by blue gate.
        /// </summary>
        BLUE,

        /// <summary>
        /// Entrance is locked by black gate.
        /// </summary>
        BLACK,

        /// <summary>
        /// Entrance is locked by red gate.
        /// </summary>
        RED
    }
}
