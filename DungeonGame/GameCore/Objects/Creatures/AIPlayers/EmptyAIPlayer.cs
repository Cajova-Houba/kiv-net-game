using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Objects.Creatures.AIPlayers
{
    /// <summary>
    /// AI player which does nothing and just stands on its position.
    /// </summary>
    public class EmptyAIPlayer : AbstractPlayer
    {
        public EmptyAIPlayer(string name, MapBlock position) : base(name, position)
        {

        }

        public override void Think()
        {
            
        }
    }
}
