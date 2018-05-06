using DungeonGame.Render.Configuration;
using GameCore.Map;
using GameCore.Objects.Creatures;
using GameCore.Objects.Items;
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
        /// Default height and width of one map block.
        /// </summary>
        public const double DEF_BLOCK_SIZE = 100;

        /// <summary>
        /// Actual height and width of one block.
        /// </summary>
        private double blockSize;

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
            blockSize = DEF_BLOCK_SIZE;
            this.finalBlockX = winningBlock.X;
            this.finalBlockY = winningBlock.Y;
        }

        /// <summary>
        /// Render map (with background) and returns it as a set of shapes.
        /// </summary>
        /// <param name="mapGrid">Map to be rendered.</param>
        /// <param name="centerBlock">Center block to render map around.</param>
        /// <param name="canvasW">Width of target canvas.</param>
        /// <param name="canvasH">Height of target canvas.</param>
        /// <returns>Map rendered as a list of shapes.</returns>
        public List<Shape> RenderMapBlocks(MapBlock[,] mapGrid, MapBlock centerBlock, double canvasW, double canvasH)
        {
            List<Shape> shapes = new List<Shape>();
            
            // target area is too small to render anything
            if (canvasW < this.blockSize || canvasH < this.blockSize)
            {
                return shapes;
            }

            int verticalBlockCount = (int)Math.Min((double)mapGrid.GetLength(1), canvasH / blockSize);
            int horizontalBlockCount = (int)Math.Min((double)mapGrid.GetLength(0), canvasW / blockSize);
            shapes.Add(new Rectangle() { Height = verticalBlockCount * blockSize, Width = horizontalBlockCount * blockSize, Fill = new SolidColorBrush(Color.FromRgb(255, 204, 102)) });

            int[] xBoundaries = GetXBoundaries(mapGrid, centerBlock, horizontalBlockCount);
            int[] yBoundaries = GetYBoundaries(mapGrid, centerBlock, verticalBlockCount);
            for (int i = xBoundaries[0]; i <= xBoundaries[1]; i++)
            {
                for (int j = yBoundaries[0]; j <= yBoundaries[1]; j++)
                {
                    List<Shape> renderedMapBlock = RenderMapBlock(mapGrid[i, j], (i-xBoundaries[0]) * blockSize, (j-yBoundaries[0]) * blockSize, blockSize);
                    shapes.AddRange(renderedMapBlock);
                }
            }

            return shapes;
        }

        /// <summary>
        /// Returns horizontal boundaries for rendering map blocks around center block.
        /// [minX,maxX].
        /// </summary>
        /// <param name="map">Map.</param>
        /// <param name="centerBlock">Center blocks.</param>
        /// <param name="horizontalBlockCount">Number of blocks to be rendered in horizontal direction.</param>
        /// <returns>Array with two items. First is the X coordinate of the leftmost block to be rendered, second is the X coordinate of the rightmost block to be rendered.</returns>
        private int[] GetXBoundaries(MapBlock[,] map, MapBlock centerBlock, int horizontalBlockCount)
        {
            int[] boundaries = new int[2];

            // number of blocks on each side from center block
            int numOfLeftBlocks = horizontalBlockCount / 2;
            int numOfRightBlocks = (int)Math.Ceiling(horizontalBlockCount / 2.0) - 1;

            // too close to the right border
            if (centerBlock.X + numOfRightBlocks >= map.GetLength(0))
            {
                boundaries[1] = map.GetLength(0) - 1;
                boundaries[0] = map.GetLength(0) - horizontalBlockCount;

            // too close to the left border
            } else if (centerBlock.X - numOfLeftBlocks < 0)
            {
                boundaries[0] = 0;
                boundaries[1] = horizontalBlockCount-1;

            // ok, somewhere in the middle
            } else
            {
                boundaries[0] = centerBlock.X - numOfLeftBlocks;
                boundaries[1] = centerBlock.X + numOfRightBlocks;
            }

            return boundaries;
        }

        /// <summary>
        /// Returns vertical boundaries for rendering map blocks around center block.
        /// [minY,maxY].
        /// </summary>
        /// <param name="map">Map.</param>
        /// <param name="centerBlock">Center block.</param>
        /// <param name="verticalBlockCount">NUmber of blocks to be rendered in vertical direction.</param>
        /// <returns>Array with two items. First is the Y coordinate of the top block to be rendered, second is the Y coordinate of the bottom block to be rendered.</returns>
        private int[] GetYBoundaries(MapBlock[,] map, MapBlock centerBlock, int verticalBlockCount)
        {
            int[] boundaries = new int[2];

            // number of blocks on each side from center block
            int numOfTopBlocks = verticalBlockCount / 2;
            int numOfBottomBlocks = (int)Math.Ceiling(verticalBlockCount / 2.0) - 1;

            // too close to the top border
            if (centerBlock.Y - numOfTopBlocks < 0)
            {
                boundaries[0] = 0;
                boundaries[1] = verticalBlockCount - 1;


            // too close to the bottom border
            } else if (centerBlock.Y + numOfBottomBlocks >= map.GetLength(1))
            {
                boundaries[0] = map.GetLength(1) - verticalBlockCount;
                boundaries[1] = map.GetLength(1) - 1;


            // ok, somewhere in the middle
            } else
            {
                boundaries[0] = centerBlock.Y - numOfTopBlocks;
                boundaries[1] = centerBlock.Y + numOfBottomBlocks;
            }

            return boundaries;
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

            if (mapBlock.Item != null)
            {
                renderedBlock.Add(RenderItem(mapBlock.Item, x, y, blockSize));
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
        /// Renders item as a path.
        /// 
        /// Item will be rendered in the top left corner (with dimensions (1/3)*blockSize x (1/3)*blockSize) of of the block.
        /// </summary>
        /// <param name="item">Item to be rendered.</param>
        /// <param name="x">Top left corner x-coordinate of map block.</param>
        /// <param name="y">Top left corner y-coordinate of map block.</param>
        /// <param name="blockSize">Height and width of the block.</param>
        /// <returns>Item rendered as path.</returns>
        private Path RenderItem(AbstractItem item, double x, double y, double blockSize)
        {
            if (item is AbstractArmor)
            {
                return RenderArmor((AbstractArmor)item, x, y, blockSize);
            } else
            {
                return new Path();
            }
        }

        /// <summary>
        /// Renders armor as a path.
        /// <param name="item">Armor to be rendered.</param>
        /// <param name="x">Top left corner x-coordinate of map block.</param>
        /// <param name="y">Top left corner y-coordinate of map block.</param>
        /// <param name="blockSize">Height and width of the block.</param>
        /// <returns>Armor rendered as path.</returns>
        private Path RenderArmor(AbstractArmor armor, double x, double y, double blockSize)
        {
            Path renderedArmor = new Path();

            GeometryGroup group = new GeometryGroup();
            group.Children.Add(Geometry.Parse(configuration.ArmorPath));

            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(blockSize/3, blockSize/3));
            transformGroup.Children.Add(new TranslateTransform(x, y));
            group.Transform = transformGroup;

            renderedArmor.Data = group;
            renderedArmor.Stroke = new SolidColorBrush(Color.FromRgb(0,0,0));


            return renderedArmor;
        }

        /// <summary>
        /// Renders creature as a path.
        /// </summary>
        /// <param name="creature">Creature to be rendered.</param>
        /// <param name="x">Top left corner x-coordinate of map block.</param>
        /// <param name="y">Top left corner y-coordinate of map block.</param>
        /// <param name="blockSize">Size of the map block.</param>
        /// <returns></returns>
        private Path RenderCreature(AbstractCreature creature, double x, double y, double blockSize)
        {
            if (creature is HumanPlayer)
            {
                return RenderHumanPlayer((AbstractPlayer)creature, x, y, blockSize);
            }
            else if (creature is AbstractPlayer)
            {
                return RenderAIPlayer((AbstractPlayer)creature, x, y, blockSize);
            }
            else
            {
                return new Path();
            }
        }

        /// <summary>
        /// Renders AI player as a path.
        /// </summary>
        /// <param name="player">Player to be rendered.</param>
        /// <param name="x">Top left corner x-coordinate of map block.</param>
        /// <param name="y">Top left cornet y-coordinate of map block.</param>
        /// <param name="blockSize">Size of the block.</param>
        /// <returns></returns>
        private Path RenderAIPlayer(AbstractPlayer player, double x, double y, double blockSize)
        {
            Path renderedPlayer = new Path();

            GeometryGroup group = new GeometryGroup();
            group.Children.Add(Geometry.Parse(configuration.AIPLayerPath));

            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(blockSize, blockSize));
            transformGroup.Children.Add(new TranslateTransform(x, y));
            group.Transform = transformGroup;

            renderedPlayer.Data = group;
            renderedPlayer.Stroke = new SolidColorBrush(Color.FromRgb(200, 0, 20));
            //renderedPlayer.StrokeThickness = 1;
            //renderedPlayer.Fill = new SolidColorBrush(Color.FromRgb(0, 153, 51));


            return renderedPlayer;
        }

        /// <summary>
        /// Renders human player as a path.
        /// </summary>
        /// <param name="player">Player to be rendered.</param>
        /// <param name="x">Top left corner x-coordinate of map block.</param>
        /// <param name="y">Top left cornet y-coordinate of map block.</param>
        /// <param name="blockSize">Size of the block.</param>
        /// <returns></returns>
        private Path RenderHumanPlayer(AbstractPlayer player, double x, double y, double blockSize)
        {
            Path renderedPlayer = new Path();

            GeometryGroup group = new GeometryGroup();
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
