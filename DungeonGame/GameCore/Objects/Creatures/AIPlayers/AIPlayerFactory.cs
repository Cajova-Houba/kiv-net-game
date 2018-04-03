using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Objects.Creatures.AIPlayers
{
    /// <summary>
    /// Factory class for various AI player implementations.
    /// </summary>
    public class AIPlayerFactory
    {
        public static AbstractPlayer CreateSimpleAIPLayer(string name, MapBlock position) 
        {
            return new SimpleAIPlayer(name, position);
        }

        public static AbstractPlayer CreateEmptyAIPlayer(string name, MapBlock position)
        {
            return new EmptyAIPlayer(name, position);
        }
    }
}
