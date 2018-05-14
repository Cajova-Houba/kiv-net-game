using GameCore.Map.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.ViewModel
{
    /// <summary>
    /// Constants shared across multiple view models and windows.
    /// </summary>
    public class ViewModelConstants
    {
        public const int DEF_MAP_WIDTH = 10;
        public const int DEF_MAP_HEIGHT = 10;
        public const int DEF_MAP_SEED = IMapGeneratorConstants.NO_SEED;
        public const int MIN_MAP_WIDTH = 3;
        public const int MAX_MAP_WIDTH = 100;
        public const int MIN_MAP_HEIGHT = 3;
        public const int MAX_MAP_HEIGHT = 100;
    }
}
