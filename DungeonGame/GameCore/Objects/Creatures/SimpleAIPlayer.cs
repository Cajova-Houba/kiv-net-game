using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;
using GameCore.Objects.Creatures.Actions;

namespace GameCore.Objects.Creatures
{
    /// <summary>
    /// This class represents computer player with simple AI which will wander around the maze, avoid fights and pick up better items.
    /// Wandering around the map will be realized by DFS algorithm.
    /// </summary>
    public class SimpleAIPlayer : AbstractPlayer
    {

        private Stack<VisitedBlock> blockStack;
        private HashSet<VisitedBlock> visitedBlocks;

        public SimpleAIPlayer(string name, MapBlock position) : base(name, position)
        {
            blockStack = new Stack<VisitedBlock>();
            blockStack.Push(new VisitedBlock(position));
            visitedBlocks = new HashSet<VisitedBlock>();
        }

        public override void Think()
        {
            Map.Map map = Position.ParentMap;

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
                    // neighbour wasn't visited and is not occupied -> add it to the stack
                    blockStack.Push(neighbourVb);
                    nextBlock = neighbour;

                }
            }


            // if some neighbours were found, create move action to the last block on the stack
            if (nextBlock != null)
            {
                NextAction = new Move() { Actor = this, Direction = DirectionMethods.GetDirection(Position, nextBlock) };
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
                if (!(blockStack.Count > 0))
                {
                    VisitedBlock prevBlock = blockStack.Peek();
                    nextBlock = map.Grid[prevBlock.x, prevBlock.y];
                    NextAction = new Move() { Actor = this, Direction = DirectionMethods.GetDirection(Position, nextBlock) };
                }
            }
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
