using System;
using GameCore.Map;
using System.Collections.Generic;
using GameCore.Game.Actions;
using GameCore.Map.Serializer;
using GameCore.Map.Serializer.Binary;

namespace GameCore.Objects.Creatures
{
    /// <summary>
    /// Base class for all 'living' objects placed on map.
    /// Declares move() method so all children can move around map.
    /// </summary>
    public abstract class AbstractCreature : GameObject
    {
        public const byte MONSTER_TYPE = 0;
        public const byte HUMAN_PLAYER_TYPE = 1;
        public const byte EMPTY_AI_TYPE = 2;
        public const byte SIMPLE_AI_TYPE = 3;

        /// <summary>
        /// Internal type of creature. Every implementing class should set this.
        /// </summary>
        public byte CreatureType { get; set; }

        /// <summary>
        /// Whether shoudl AI ignore its' speed. Should be used while implementing Think() method.
        /// </summary>
        public bool IgnoreSpeed { get; set; }

        /// <summary>
        /// Base HP. Total HP might be changed accordingly to equipped items.
        /// </summary>
        public int BaseHitPoints { get;  set; }

        /// <summary>
        /// Current HP of this creature.
        /// </summary>
        public int CurrentHitPoints { get; protected set; }

        /// <summary>
        /// Base attack. Total attack might be changed accordingly to equipped items.
        /// </summary>
        public int BaseAttack { get;  set; }

        /// <summary>
        /// Base deffense. Total deffense might be changes accordingly to equipped items.
        /// </summary>
        public int BaseDeffense { get;  set; }

        /// <summary>
        /// Actions to be performed in game loop. Rather than direct access (public get is for testing purposes), NextAction property should be used.
        /// </summary>
        public Queue<AbstractAction> ActionQueue {get; protected set;}

        /// <summary>
        /// Returns true if this creature is still alive (CurrentHitPoints > 0).
        /// </summary>
        public bool Alive { get { return CurrentHitPoints > 0; } }

        private int moveCounter;

        /// <summary>
        /// Returns the number of move actions which were executed over this creature.
        /// </summary>
        public int MoveCounter
        {
            get { return moveCounter; }
        }

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

        public AbstractCreature(string name, MapBlock position, int baseHitPoints, int baseAttack, int baseDeffense, byte creatureType) : base(name, position)
        {
            BaseHitPoints = baseHitPoints;
            CurrentHitPoints = baseHitPoints;
            BaseAttack = baseAttack;
            BaseDeffense = baseDeffense;
            ActionQueue = new Queue<AbstractAction>();
            moveCounter = 0;
            CreatureType = creatureType;
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
            moveCounter++;
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

        public override List<byte> SerializeBinary()
        {
            List<byte> crBytes = new List<byte>();

            // uid
            crBytes.AddRange(BinarySerializerUtils.IntToBytes(UniqueId));

            // name
            crBytes.AddRange(BinarySerializerUtils.StrToBytes(Name, GameObject.MAX_NAME_LENGTH));

            // position
            crBytes.AddRange(BinarySerializerUtils.IntToBytes(Position.X));
            crBytes.AddRange(BinarySerializerUtils.IntToBytes(Position.Y));

            // creature attributes
            byte type = CreatureType;
            crBytes.AddRange(BinarySerializerUtils.IntToBytes(BaseHitPoints));
            crBytes.AddRange(BinarySerializerUtils.IntToBytes(BaseAttack));
            crBytes.AddRange(BinarySerializerUtils.IntToBytes(BaseDeffense));
            crBytes.Add(type);

            return crBytes;
        }
    }
}
