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
        /// <param name="seed">Seed for random map generation. Use -1 to ignore this.</param>
        /// <returns>Generated map.</returns>
        Map.Map GenerateMap(int width, int height, int seed);

        /// <summary>
        /// Generates grid with given dimensions. This method should only generate map itself not fill it with items / monsters.
        /// </summary>
        /// <param name="widht">Width. Must be greater or equal to 1.</param>
        /// <param name="height">Height. Must be greater or equal to 1.</param>
        /// <param name="seed">Seed for random map generation. Use -1 to ignore this.</param>
        /// <returns>Generated map.</returns>
        MapBlock[,] GenerateGrid(int width, int height, int seed);
    }
    
    /// <summary>
    /// Constants common for all map generators.
    /// </summary>
    public class IMapGeneratorConstants
    {
        /// <summary>
        /// Use this as a seed argument when you don't want to specify the seed.
        /// </summary>
        public const int NO_SEED = -1;
    }
}
