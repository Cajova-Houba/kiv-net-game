using GameCore.Map;
using GameCore.Objects.Creatures.Actions;
using System.Collections.Generic;

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

        /// <summary>
        /// Actions to be performed in game loop. Rather than direct access (public get is for testing purposes), NextAction property should be used.
        /// </summary>
        public Queue<AbstractAction> ActionQueue {get; protected set;}

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

        public AbstractCreature(string name, MapBlock position, int baseHitPoints, int baseAttack, int baseDeffense) : base(name, position)
        {
            BaseHitPoints = baseHitPoints;
            BaseAttack = baseAttack;
            BaseDeffense = baseDeffense;
            ActionQueue = new Queue<AbstractAction>();
        }

        /// <summary>
        /// Moves creature to given block.
        /// </summary>
        /// <param name="mapBlock">New position.</param>
        public void MoveTo(MapBlock mapBlock)
        {
            Position = mapBlock;
        }


        /// <summary>
        /// Performs 'AI thinking'. Result of this method should be action(s) added to action queue.
        /// Leave this method empty for human players as their thinking is replaced by user input.
        /// </summary>
        public abstract void Think();
    }
}
