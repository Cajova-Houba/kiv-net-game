using DungeonGame.Render.Configuration;
using GameCore.Map;
using GameCore.Objects.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DungeonGame.Render
{
    /// <summary>
    /// Class for rendering map. There should be one renderer per game instance.
    /// </summary>
    public class MapRenderer
    {
        /// <summary>
        /// Configuration used for rendering.
        /// </summary>
        private RenderConfiguration configuration;

        /// <summary>
        /// X coordinate of the winning block.
        /// </summary>
        private int finalBlockX;

        /// <summary>
        /// Y coordinate of the winning block.
        /// </summary>
        private int finalBlockY;

        /// <summary>
        /// Initializes this map renderer with configuration.
        /// </summary>
        /// <param name="renderConfiguration"></param>
        public MapRenderer(RenderConfiguration renderConfiguration, MapBlock winningBlock)
        {
            configuration = renderConfiguration;
            this.finalBlockX = winningBlock.X;
            this.finalBlockY = winningBlock.Y;
        }

        /// <summary>
        /// Render map (with background) and returns it as a set of shapes.
        /// </summary>
        /// <param name="mapGrid">Map to be rendered.</param>
        /// <param name="canvasW">Width of target canvas.</param>
        /// <param name="canvasH">Height of target canvas.</param>
        /// <param name="minX">Min x coordinate of mapGrid to be rendered.</param>
        /// <param name="minY">Min y coordinate of mapGrid to be rendered.</param>
        /// <param name="maxX">Max x coordinate of mapGrid to be rendered.</param>
        /// <param name="maxY">Max y coordinate of mapGrid to be rendered.</param>
        /// <returns>Map rendered as a list of shapes.</returns>
        public List<Shape> RenderMapBlocks(MapBlock[,] mapGrid, double canvasW, double canvasH, int minX, int minY, int maxX, int maxY)
        {
            List<Shape> shapes = new List<Shape>();
            
            int blockCount = Math.Max(maxX + 1 - minX, maxY + 1 - minY);
            double blockSize = Math.Min(canvasW / (double)blockCount, canvasH / (double)blockCount);
            shapes.Add(new Rectangle() { Height = blockCount * blockSize, Width = blockCount * blockSize, Fill = new SolidColorBrush(Color.FromRgb(255, 204, 102)) });

            for (int i = minX; i <= maxX; i++)
            {
                for (int j = minY; j <= maxY; j++)
                {
                    List<Shape> renderedMapBlock = RenderMapBlock(mapGrid[i, j], (i-minX) * blockSize, (j-minY) * blockSize, blockSize);
                    shapes.AddRange(renderedMapBlock);
                }
            }

            return shapes;
        }

        /// <summary>
        /// Renders one map block to the gameCanvas.
        /// </summary>
        /// <param name="mapBlock">Map block to be rendered.</param>
        /// <param name="x">Top left corner x-coordinate.</param>
        /// <param name="y">Top left corner y-coordinate.</param>
        /// <param name="blockSize">Size of the block (=width=height).</param>
        /// <returns>Block rendered as a set of shapes.</returns>
        private List<Shape> RenderMapBlock(MapBlock mapBlock, double x, double y, double blockSize)
        {
            List<Shape> renderedBlock = new List<Shape>();
            double innerMargin = 2;
            x += innerMargin;
            y += innerMargin;
            blockSize -= 2 * innerMargin;
            // if there is entrace, draw it as line-empty space-line
            double entranceSize = blockSize / 3;

            // decide border color
            Color roomBorderColor;
            try
            {
                if (mapBlock.X == finalBlockX && mapBlock.Y == finalBlockY)
                {
                    roomBorderColor = (Color)ColorConverter.ConvertFromString(configuration.FinalRoomColor);
                } else
                {
                    roomBorderColor = (Color)ColorConverter.ConvertFromString(configuration.RoomColor);
                }
            } catch(Exception ex)
            {
                // todo: log
                roomBorderColor = Color.FromRgb(0, 0, 0);
            }

            renderedBlock.AddRange(RenderHorizontalEntrance(mapBlock.North, x, y, entranceSize, roomBorderColor));
            renderedBlock.AddRange(RenderHorizontalEntrance(mapBlock.South, x, y + blockSize, entranceSize, roomBorderColor));
            renderedBlock.AddRange(RenderVerticalEntrance(mapBlock.East, x + blockSize, y, entranceSize, roomBorderColor));
            renderedBlock.AddRange(RenderVerticalEntrance(mapBlock.West, x, y, entranceSize, roomBorderColor));

            if (mapBlock.Creature != null)
            {
                renderedBlock.Add(RenderCreature(mapBlock.Creature, x, y, blockSize));
            }

            return renderedBlock;
        }

        /// <summary>
        /// Renders horizontal entrance.
        /// </summary>
        /// <param name="entrance">Entrance to be rendered. May be null</param>
        /// <param name="x">X1 coordinate.</param> 
        /// <param name="y">X2 coordinate.</param>
        /// <param name="entranceSize">Size of the entrance (if no entrance, line 3*entranceSize long is returned).</param>
        /// <param name="color">Color of this entrance (not lock).</param>
        /// <returns>Entrance rendered as lines.</returns>
        private List<Line> RenderHorizontalEntrance(Entrance entrance, double x, double y, double entranceSize, Color color)
        {
            List<Line> entranceLines = new List<Line>();
            Brush b = new SolidColorBrush(color);
            if (entrance != null && entrance.IsOpen())
            {
                // line - nothing - line
                entranceLines.Add(new Line() { X1 = x, Y1 = y, X2 = x + entranceSize, Y2 = y, Stroke = b, StrokeThickness = 1 });
                entranceLines.Add(new Line() { X1 = x + 2 * entranceSize, Y1 = y, X2 = x + 3 * entranceSize, Y2 = y, Stroke = b, StrokeThickness = 1 });
            }
            else
            {
                entranceLines.Add(new Line() { X1 = x, Y1 = y, X2 = x + entranceSize * 3, Y2 = y, Stroke = b, StrokeThickness = 1 });
            }
            return entranceLines;
        }

        /// <summary>
        /// Renders vertical entrance.
        /// </summary>
        /// <param name="entrance">Entrance to be rendered. May be null.</param>
        /// <param name="x">X1 coordinate.</param>
        /// <param name="y">X2 coordinate.</param>
        /// <param name="entranceSize">Size of the entrance (if no entrance, line 3*entranceSize long is returned).</param>
        /// <param name="color">Color to render this entrance (not lock).</param>
        /// <returns>Entrance rendered as lines.</returns>
        private List<Line> RenderVerticalEntrance(Entrance entrance, double x, double y, double entranceSize, Color color)
        {
            List<Line> entranceLines = new List<Line>();
            Brush b = new SolidColorBrush(color);
            if (entrance != null && entrance.IsOpen())
            {
                // line - nothing - line
                entranceLines.Add(new Line() { X1 = x, Y1 = y, X2 = x, Y2 = y + entranceSize, Stroke = b, StrokeThickness = 1 });
                entranceLines.Add(new Line() { X1 = x, Y1 = y + 2 * entranceSize, X2 = x, Y2 = y + 3 * entranceSize, Stroke = b, StrokeThickness = 1 });
            }
            else
            {
                entranceLines.Add(new Line() { X1 = x, Y1 = y, X2 = x, Y2 = y + 3 * entranceSize, Stroke = b, StrokeThickness = 1 });
            }

            return entranceLines;
        }

        /// <summary>
        /// Renders creature as a path.
        /// </summary>
        /// <param name="creature">Creature to be rendered.</param>
        /// <param name="x">Top corner x-coordinate of map block.</param>
        /// <param name="y">Top corner y-coordinate of map block.</param>
        /// <param name="blockSize">Size of the map block.</param>
        /// <returns></returns>
        private Path RenderCreature(AbstractCreature creature, double x, double y, double blockSize)
        {
            if (creature is AbstractPlayer)
            {
                return RenderPlayer((AbstractPlayer)creature, x, y, blockSize);
            }
            else
            {
                return new Path();
            }
        }

        /// <summary>
        /// Renders player as a path.
        /// </summary>
        /// <param name="player">Player to be rendered.</param>
        /// <param name="x">Top left corner x-coordinate of map block.</param>
        /// <param name="y">Top left cornet y-coordinate of map block.</param>
        /// <param name="blockSize">Size of the block.</param>
        /// <returns></returns>
        private Path RenderPlayer(AbstractPlayer player, double x, double y, double blockSize)
        {
            Path renderedPlayer = new Path();

            //LineGeometry firstLeg = new LineGeometry(new Point(1 / 3.0, 1), new Point(0.5, 2 / 3.0));
            //LineGeometry secondLeg = new LineGeometry(new Point(2 / 3.0, 1), new Point(0.5, 2 / 3.0));
            //LineGeometry body = new LineGeometry(new Point(0.5, 2 / 3.0), new Point(0.5, 1 / 6.0));
            //LineGeometry firstHand = new LineGeometry(new Point(0.5, 1 / 3.0), new Point(1 / 3.0, 0.5));
            //LineGeometry secondHand = new LineGeometry(new Point(0.5, 1 / 3.0), new Point(2 / 3.0, 0.5));
            //EllipseGeometry head = new EllipseGeometry(new Point(0.5, 1 / 6.0), 1 / 10.0, 1 / 10.0);

            GeometryGroup group = new GeometryGroup();
            //group.Children.Add(firstLeg);
            //group.Children.Add(secondLeg);
            //group.Children.Add(body);
            //group.Children.Add(firstHand);
            //group.Children.Add(secondHand);
            //group.Children.Add(head);
            group.Children.Add(Geometry.Parse(configuration.HumanPlayerPath));

            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(blockSize, blockSize));
            transformGroup.Children.Add(new TranslateTransform(x, y));
            group.Transform = transformGroup;

            renderedPlayer.Data = group;
            renderedPlayer.Stroke = new SolidColorBrush(Color.FromRgb(0, 153, 51));
            //renderedPlayer.StrokeThickness = 1;
            //renderedPlayer.Fill = new SolidColorBrush(Color.FromRgb(0, 153, 51));


            return renderedPlayer;
        }
    }
}
