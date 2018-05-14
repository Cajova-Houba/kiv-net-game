using GameCore.Map.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.ViewModel
{
    /// <summary>
    /// Constants shared across multiple view models.
    /// </summary>
    public class ViewModelConstants
    {
        public const int DEF_MAP_WIDTH = 10;
        public const int DEF_MAP_HEIGHT = 10;
        public const int DEF_MAP_SEED = IMapGeneratorConstants.NO_SEED;
    }
}
