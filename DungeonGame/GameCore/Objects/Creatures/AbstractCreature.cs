using System;
using GameCore.Map;
using System.Collections.Generic;
using GameCore.Game.Actions;

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
        /// Current HP of this creature.
        /// </summary>
        public int CurrentHitPoints { get; protected set; }

        /// <summary>
        /// Base attack. Total attack might be changed accordingly to equipped items.
        /// </summary>
        public int BaseAttack { get; protected set; }

        /// <summary>
        /// Base deffense. Total deffense might be changes accordingly to equipped items.
        /// </summary>
        public int BaseDeffense { get; protected set; }

        /// <summary>
        /// Actions to be performed in game loop. Rather than direct access (public get is for testing purposes), NextAction property should be used.
        /// </summary>
        public Queue<AbstractAction> ActionQueue {get; protected set;}

        /// <summary>
        /// Returns true if this creature is still alive (CurrentHitPoints > 0).
        /// </summary>
        public bool Alive { get { return CurrentHitPoints > 0; } }

        /// <summary>
        /// Returns the action to be performed (if any) and adds new action to the action queue.
        /// </summary>
        public AbstractAction NextAction {
            get
            {
                if (ActionQueue.Count > 0)
                {
                    return ActionQueue.Dequeue();
                } else
                {
                    return null;
                }
            }
            set {
                ActionQueue.Enqueue(value);
            }
        }

        /// <summary>
        /// Returns total attack of this creature with all bonuses applied.
        /// </summary>
        /// <returns>Total attack value of this creature.</returns>
        public abstract double TotalAttack { get; }


        /// <summary>
        /// Returns total deffense of this creature with all bonuses applied.
        /// </summary>
        /// <returns>Total deffense value of this creature.</returns>
        public abstract double TotalDeffense { get; }

        /// <summary>
        /// Returns max possible HP of this creature (so base HP increased / decreased by bonuses / degrading effects).
        /// </summary>
        /// <returns>Max possible HP of this creature.</returns>
        public abstract double MaxHitPoints { get; }

        public AbstractCreature(string name, MapBlock position, int baseHitPoints, int baseAttack, int baseDeffense) : base(name, position)
        {
            BaseHitPoints = baseHitPoints;
            CurrentHitPoints = baseHitPoints;
            BaseAttack = baseAttack;
            BaseDeffense = baseDeffense;
            ActionQueue = new Queue<AbstractAction>();
            if (position != null)
            {
                position.Creature = this;
            }
        }

        /// <summary>
        /// Moves creature to given block.
        /// </summary>
        /// <param name="mapBlock">New position.</param>
        public void MoveTo(MapBlock mapBlock)
        {
            // move from current position
            Position.Creature = null;

            // to the new position
            Position = mapBlock;
            mapBlock.Creature = this;
        }

        /// <summary>
        /// Decreases HP of this creature by rounded damage. If the damage is greater than current HP, HP is set to 0.
        /// </summary>
        /// <param name="damage">Damage this boy is going to take.</param>
        public void TakeDamage(double damage)
        {
            int roundDamage = (int)Math.Round(damage);
            CurrentHitPoints = Math.Max(0, CurrentHitPoints - roundDamage);
        }

        /// <summary>
        /// Performs 'AI thinking'. Result of this method should be action(s) added to action queue.
        /// Leave this method empty for human players as their thinking is replaced by user input.
        /// </summary>
        public abstract void Think();
    }
}
