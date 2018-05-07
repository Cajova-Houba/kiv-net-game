using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;
using GameCore.Game.Actions;

namespace GameCore.Objects.Creatures.AIPlayers
{
    /// <summary>
    /// This class represents computer player with simple AI which will wander around the maze, avoid fights and pick up better items.
    /// Wandering around the map will be realized by DFS algorithm.
    /// 
    /// Note that when the only possible block this AI can go to is occupied by a creature, AI won't move (to avoid fights).
    /// </summary>
    public class SimpleAIPlayer : AbstractPlayer
    {
        /// <summary>
        /// Default AI speed measured in actions per seconds.
        /// </summary>
        public const double DEF_AI_SPEED = 1.0/2;

        /// <summary>
        /// Time of the last action. Used to set speed of "action per seconds".
        /// </summary>
        private double lastActionTime;

        /// <summary>
        /// Speed of this AI, measured in actions per second.
        /// </summary>
        private double aiSpeed;

        private Stack<VisitedBlock> blockStack;
        private HashSet<VisitedBlock> visitedBlocks;

        public SimpleAIPlayer(string name, MapBlock position) : base(name, position)
        {
            blockStack = new Stack<VisitedBlock>();
            blockStack.Push(new VisitedBlock(position));
            visitedBlocks = new HashSet<VisitedBlock>();
            lastActionTime = 0;
            aiSpeed = DEF_AI_SPEED;
        }

        public override void Think()
        {
            // TODO: algorithm is not optimal because when returning from child to parent, child nodes will be listed again (not so big problem, but it something to improve)

            // timing
            double currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            double mspa = 1000 / aiSpeed;
            if ((currentTime - lastActionTime) < mspa)
            {
                return;
            }

            Map.Map map = Position.ParentMap;

            // if the stack is empty AI may be in dead end
            // put its current position to the stack and run the DFS again
            if (blockStack.Count == 0)
            {
                blockStack.Push(new VisitedBlock(Position));
            }

            // get current node and his neighbours
            VisitedBlock currVisitedBlock = blockStack.Peek();
            currVisitedBlock.state = BlockState.OPEN;
            visitedBlocks.Add(currVisitedBlock);
            MapBlock curr = map.Grid[currVisitedBlock.x, currVisitedBlock.y];
            List<MapBlock> neighbours = curr.AccessibleNeighbours();

            // choose the next neighbour to go to
            // those neighbours must not be in CLOSED state and should not contain monster
            MapBlock nextBlock = null;
            foreach(MapBlock neighbour in neighbours)
            {
                // check if the neighbour wasn't already visited and is not occupied
                // visited block hash code is calculated only by its coordinates so this can be used
                VisitedBlock neighbourVb = new VisitedBlock(neighbour);
                if (!visitedBlocks.Contains(neighbourVb) && !neighbour.Occupied) {
                    // get the last not-visited, unoccupied neighbour
                    // add it to the stack later
                    nextBlock = neighbour;

                }
            }


            // if some neighbours were found, create move action to the last block on the stack
            if (nextBlock != null)
            {
                blockStack.Push(new VisitedBlock(nextBlock));
                Direction nextDirection = DirectionMethods.GetDirection(Position, nextBlock);
                NextAction = new Move() { Actor = this, Direction = nextDirection };
            } else
            {
                // no possible neighbours were found
                // this opens up two possibilities
                // 1. assume current block is a dead end
                // 2. check if there are monster to fight in neighbour blocks and try to fight them
                // for now lets just pick the first one

                // mark current block as closed and pop it from the stack
                currVisitedBlock.state = BlockState.CLOSED;
                blockStack.Pop();

                // move one step back
                if (blockStack.Count > 0)
                {
                    VisitedBlock prevBlock = blockStack.Peek();
                    nextBlock = map.Grid[prevBlock.x, prevBlock.y];
                    NextAction = new Move() { Actor = this, Direction = DirectionMethods.GetDirection(Position, nextBlock) };
                }
            }

            lastActionTime = currentTime;
        }
    }

    /// <summary>
    /// Helper class for 'remembering' visited blocks.
    /// It contains coordinates of particular map block and state (used by DFS algorithm).
    /// </summary>
    public class VisitedBlock
    {
        public int x;

        public int y;

        public BlockState state;

        public VisitedBlock()
        {

        }

        public VisitedBlock(MapBlock mapBlock)
        {
            x = mapBlock.X;
            y = mapBlock.Y;
            state = BlockState.NEW;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is VisitedBlock))
            {
                return false;
            }

            var coordinates = (VisitedBlock)obj;
            return x == coordinates.x &&
                   y == coordinates.y;
        }

        /// <summary>
        /// Uses x,y coordinates to calculate hash. State is not used in hash function.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"VisitedBlock [{x}, {y}]";
        }
    }

    /// <summary>
    /// Possible states for map blocks used by DFS algorithm.
    /// </summary>
    public enum BlockState
    {
        NEW,
        OPEN,
        CLOSED
    }
}
