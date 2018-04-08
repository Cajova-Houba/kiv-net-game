using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Map.Generator
{
    /// <summary>
    /// Factory class for various map generators.
    /// </summary>
    public class MapGeneratorFactory
    {
        public static IMapGenerator CreateOpenMapGenerator()
        {
            return new OpenMapGenerator();
        }

        public static IMapGenerator CreateSimpleMapGenerator()
        {
            return new SimpleMapGenerator();
        }
    }
}
