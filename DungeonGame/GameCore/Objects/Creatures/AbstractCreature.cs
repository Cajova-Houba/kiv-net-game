using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Objects.Creatures
{
    /// <summary>
    /// Base class for all 'living' objects placed on map.
    /// Declares move() method so all children can move around map.
    /// </summary>
    public abstract class AbstractCreature : GameObject
    {
        /// <summary>
        /// Base HP. Total HP might be changed accordingly to equipped items.
        /// </summary>
        public int BaseHitPoints { get; protected set; }

        /// <summary>
        /// Base attack. Total attack might be changed accordingly to equipped items.
        /// </summary>
        public int BaseAttack { get; protected set; }

        /// <summary>
        /// Base deffense. Total deffense might be changes accordingly to equipped items.
        /// </summary>
        public int BaseDeffense { get; protected set; }


        public AbstractCreature(string name, MapBlock position, int baseHitPoints, int baseAttack, int baseDeffense) : base(name, position)
        {
            BaseHitPoints = baseHitPoints;
            BaseAttack = baseAttack;
            BaseDeffense = baseDeffense;
        }

        /// <summary>
        /// Moves creature to given block.
        /// </summary>
        /// <param name="mapBlock">New position.</param>
        public void MoveTo(MapBlock mapBlock)
        {
            Position = mapBlock;
        }
    }
}
