using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Objects.Creatures
{
    /// <summary>
    /// This class represents human player. Its think() method does nothing as setting next actions relies on player himself.
    /// </summary>
    public class HumanPlayer : AbstractPlayer
    {
        public HumanPlayer(string name, MapBlock position) : base(name, position)
        {
        }

        public override void Think()
        {
        }
    }
}
