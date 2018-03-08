using System;

namespace GameCore.Map
{
    /// <summary>
    /// Thic class represents entrance to the MapBlock.
    /// </summary>
    public class Entrance
    {
        /// <summary>
        /// State of this entrance.
        /// </summary>
        public EntranceState State { get; protected set; }

        /// <summary>
        /// Returns lock if this entrance is locked.
        /// </summary>
        public Lock EntranceLock { get; protected set; }

        /// <summary>
        /// Initializes this entrance with NONEXISTENT state.
        /// </summary>
        public Entrance() : this(EntranceState.NONEXISTENT)
        {
        }

        /// <summary>
        /// Creates entrance with given state.
        /// </summary>
        /// <param name="state">State of the entrance.</param>
        public Entrance(EntranceState state)
        {
            EntranceLock = null;
            State = state;
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
        /// Method used to check if the entrance is locked by a lock.
        /// </summary>
        /// <returns></returns>
        public Boolean IsLocked()
        {
            return EntranceLock != null;
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
        
        /// <summary>
        /// Sets the state of this entrance to OPEN no matter what. If there was any lock, it's set to null.
        /// </summary>
        public void DemolishWall()
        {
            EntranceLock = null;
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
        OPEN
    }
}
