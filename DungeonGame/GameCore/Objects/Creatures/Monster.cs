using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;
using GameCore.Objects.Creatures.Actions;

namespace GameCore.Objects.Creatures
{
    /// <summary>
    /// This class represents simple creatures such as spiders, rats and goblins which lurk in
    /// the dungeon. Monsters can't win, they can only lose (=die).
    /// </summary>
    public abstract class Monster : AbstractCreature
    {
        /// <summary>
        /// Default maximum of stack capacity.
        /// </summary>
        public const int DEF_MAX_STACK_CAPACITY = 20;

        /// <summary>
        /// Max number of tries. After this number of attemts is reached, monster will stop on choosing next block and stays still.
        /// </summary>
        public const int DEF_MAX_RANDOM_TRIES = 10;


        /// <summary>
        /// This stack will contain monster's path. Monster will keep on randomly selecting next directions until the stack is full. Then they'll pop the stack until its empty. This is
        /// the whole monster AI.
        /// </summary>
        private Stack<Direction> pathStack;

        /// <summary>
        /// Max stack capacity. Should be >=1.
        /// </summary>
        private int stackCapacity;

        /// <summary>
        /// Random object used in thinking.
        /// </summary>
        private Random randomizer;

        /// <summary>
        /// Flag indicating whether the creature should add next blocks to path or if it should just pop the stack.
        /// </summary>
        private Boolean goBackwardFlag;

        /// <summary>
        /// Initializes this monster with given values and stack capacity randomly in range [1;10].
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="baseHitPoints"></param>
        /// <param name="baseAttack"></param>
        /// <param name="baseDeffense"></param>
        public Monster(string name, MapBlock position, int baseHitPoints, int baseAttack, int baseDeffense) : base(name, position, baseHitPoints, baseAttack, baseDeffense)
        {
            goBackwardFlag = true;
            randomizer = new Random();
            stackCapacity = randomizer.Next(DEF_MAX_STACK_CAPACITY) + 1;
            pathStack = new Stack<Direction>();
        }

        /// <summary>
        /// Initializes this monster with given values and stack capacity.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="baseHitPoints"></param>
        /// <param name="baseAttack"></param>
        /// <param name="baseDeffense"></param>
        /// <param name="stackCapacity">Max capacity of stack. Should be greater than 0.</param>
        public Monster(string name, MapBlock position, int baseHitPoints, int baseAttack, int baseDeffense, int stackCapacity) : base(name, position, baseHitPoints, baseAttack, baseDeffense)
        {
            goBackwardFlag = true;
            this.stackCapacity = stackCapacity;
            pathStack = new Stack<Direction>();
        }

        /// <summary>
        /// Monster will keep on picking random blocks until the stack is full, then returns to its initial position and repeats.
        /// </summary>
        public override void Think()
        {
            Direction nextDirection;
            if(goBackwardFlag)
            {
                nextDirection = GoBackward();
            } else
            {
                nextDirection = GoForward();
            }

            if(!nextDirection.IsNoDirection())
            {
                // direction returned, add move action
                NextAction = new Move() { Actor = this, Direction = nextDirection };
            }
        }

        /// <summary>
        /// Decides whether to choose direction of next block or to switch backwardFlag.
        /// </summary>
        /// <return>Next direction. May be null.</return>
        private Direction GoForward()
        {
            Direction nextDirection = Direction.NO_DIRECTION;
            Direction[] allDirections = Direction.NORTH.GetAllDirections();
            if (pathStack.Count != stackCapacity)
            {
                // stack is not full, pick next direction
                // keep trying random directions until one is selected or max attempt count is reached
                int attempt = 0;
                while (nextDirection.IsNoDirection() && attempt < DEF_MAX_RANDOM_TRIES)
                {
                    Direction randomDirection = allDirections[randomizer.Next(allDirections.Length)];
                    if(Position.NextOpenBlock(randomDirection) != null)
                    {
                        nextDirection = randomDirection;
                    }
                    attempt++;
                }

                // some direction was picked -> add it to stack
                if(!nextDirection.IsNoDirection())
                {
                    pathStack.Push(nextDirection);
                }
            } else
            {
                // stack is full set backward flag and do nothing
                goBackwardFlag = true;
            }
            return nextDirection;
        }

        /// <summary>
        /// Decides whether to pop direction from stack or to switch backwardFlag.
        /// </summary>
        /// <returns>Next direction (opposite to the one popped from stack). May be null.</returns>
        private Direction GoBackward()
        {
            Direction nextDirection = Direction.NO_DIRECTION;
            if (pathStack.Count > 0)
            {
                // stack is not empty, go backwards
                nextDirection = pathStack.Pop().OppsiteDirection();
            } else
            {
                // stack is empty, unset backward flag
                goBackwardFlag = false;
            }
            return nextDirection;
        }
    }
}
