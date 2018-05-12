using GameCore.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Render
{
    /// <summary>
    /// Common interface for map renderers.
    /// </summary>
    public interface IMapRenderer
    {
        /// <summary>
        /// Renders map as a collection of UI elements which can be added to canvas.
        /// Map blocks will have constant size.
        /// </summary>
        /// <param name="map">Map to be rendered.</param>
        /// <param name="centerBlock">Center block of map. Method will try to render this block in a center with the rest of displayable part of map (dependens on canvasW, canvasH) around it.</param>
        /// <param name="canvasW">Width of target canvas.</param>
        /// <param name="canvasH">Height of target canvas.</param>
        /// <returns>Map rendered as a set of UI elements.</returns>
        List<System.Windows.UIElement> RenderMap(Map map, MapBlock centerBlock, double canvasW, double canvasH);
    }

    /// <summary>
    /// Constants for map rendering.
    /// </summary>
    public class MapRendererConstants
    {
        /// <summary>
        /// Default height and width of one map block.
        /// </summary>
        public const double DEF_BLOCK_SIZE = 50;
    }
}
