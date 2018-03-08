using System;
using System.Collections.Generic;
using System.Text;
using GameCore.Map;

namespace GameCore.Generator
{
    /// <summary>
    /// Interface which specifies methods for map generators. 
    /// </summary>
    public interface IMapGenerator
    {

        /// <summary>
        /// Generates whole map object. This method should call GenerateGrid internaly.
        /// </summary>
        /// <param name="width">Width. Must be greater or equal to 1.</param>
        /// <param name="height">Height. Must be greater or equal to 1.</param>
        /// <returns></returns>
        Map.Map GenerateMap(int width, int height);

        /// <summary>
        /// Generates grid with given dimensions. This method should only generate map itself not fill it with items / monsters.
        /// </summary>
        /// <param name="widht">Width. Must be greater or equal to 1.</param>
        /// <param name="height">Height. Must be greater or equal to 1.</param>
        /// <returns>Generated map.</returns>
        MapBlock[,] GenerateGrid(int width, int height);
    }
}
