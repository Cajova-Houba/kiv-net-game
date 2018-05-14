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
    /// Class for rendering map which uses vector UI components. Must be called from STA thread.
    /// 
    /// </summary>
    public class VectorMapRenderer : IMapRenderer
    {
        /// <summary>
        /// Actual height and width of one block.
        /// </summary>
        private double blockSize;

        /// <summary>
        /// Configuration used for rendering.
        /// </summary>
        private RenderConfiguration configuration;

        /// <summary>
        /// Initializes this map renderer with configuration.
        /// </summary>
        /// <param name="renderConfiguration"></param>
        public VectorMapRenderer(RenderConfiguration renderConfiguration)
        {
            configuration = renderConfiguration;
            blockSize = MapRendererConstants.DEF_BLOCK_SIZE;
        }

        /// <summary>
        /// Render map (with background) and returns it as a set of shapes.
        /// </summary>
        /// <param name="mapGrid">Map to be rendered.</param>
        /// <param name="centerBlock">Center block to render map around.</param>
        /// <param name="canvasW">Width of target canvas.</param>
        /// <param name="canvasH">Height of target canvas.</param>
        /// <returns>Map rendered as a list of shapes.</returns>
        public List<UIElement> RenderMap(Map map, MapBlock centerBlock, double canvasW, double canvasH)
        {
            List<UIElement> renderedMap = new List<UIElement>();
            
            // target area is too small to render anything
            if (canvasW < this.blockSize || canvasH < this.blockSize)
            {
                return renderedMap;
            }

            MapBlock[,] mapGrid = map.Grid;
            int verticalBlockCount = (int)Math.Min((double)map.Height, canvasH / blockSize);
            int horizontalBlockCount = (int)Math.Min((double)map.Width, canvasW / blockSize);
            renderedMap.Add(new Rectangle() { Height = verticalBlockCount * blockSize, Width = horizontalBlockCount * blockSize, Fill = new SolidColorBrush(Color.FromRgb(255, 204, 102)) });

            int[] xBoundaries = GetXBoundaries(mapGrid, centerBlock, horizontalBlockCount);
            int[] yBoundaries = GetYBoundaries(mapGrid, centerBlock, verticalBlockCount);
            for (int i = xBoundaries[0]; i <= xBoundaries[1]; i++)
            {
                for (int j = yBoundaries[0]; j <= yBoundaries[1]; j++)
                {
                    List<Shape> renderedMapBlock = RenderMapBlock(mapGrid[i, j], (i-xBoundaries[0]) * blockSize, (j-yBoundaries[0]) * blockSize, blockSize, map.WinningBlock.X, map.WinningBlock.Y);
                    renderedMap.AddRange(renderedMapBlock);
                }
            }

            return renderedMap;
        }


        public List<UIElement> RenderWholeMap(Map map)
        {
            List<UIElement> renderedMap = new List<UIElement>();

            int verticalBlockCount = map.Height;
            int horizontalBlockCount = map.Width;
            renderedMap.Add(new Rectangle() { Height = verticalBlockCount * blockSize, Width = horizontalBlockCount * blockSize, Fill = new SolidColorBrush(Color.FromRgb(255, 204, 102)) });

            for (int i = 0; i < horizontalBlockCount; i++)
            {
                for (int j = 0; j < verticalBlockCount; j++)
                {
                    renderedMap.AddRange(RenderMapBlock(map.Grid[i, j], i*blockSize, j*blockSize, blockSize, map.WinningBlock.X, map.WinningBlock.Y));
                }
            }

            return renderedMap;
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
        /// <param name="winningBlockX">X coordinate (from map grid) of winning map block.</param>
        /// <param name="winningBlockY">Y coordinate (from map grid) of winning map block.</param>
        /// <returns>Block rendered as a set of shapes.</returns>
        private List<Shape> RenderMapBlock(MapBlock mapBlock, double x, double y, double blockSize, int winningBlockX, int winningBlockY)
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
                if (mapBlock.X == winningBlockX && mapBlock.Y == winningBlockY)
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
            } else if (item is AbstractWeapon)
            {
                return RenderWeapon((AbstractWeapon)item, x, y, blockSize);
            } else if (item is AbstractInventoryItem) {
                return RenderItem((AbstractInventoryItem)item, x, y, blockSize);
            } else
            {
                return new Path();
            }
        }

        /// <summary>
        /// Renders armor as a path.
        /// <param name="armor">Armor to be rendered.</param>
        /// <param name="x">Top left corner x-coordinate of map block.</param>
        /// <param name="y">Top left corner y-coordinate of map block.</param>
        /// <param name="blockSize">Height and width of the block.</param>
        /// <returns>Armor rendered as path.</returns>
        private Path RenderArmor(AbstractArmor armor, double x, double y, double blockSize)
        {
            return RenderPath(configuration.ArmorPath, x, y, blockSize / 3, Color.FromRgb(0, 0, 0), null);
        }

        /// <summary>
        /// Renders weapon as a path.
        /// </summary>
        /// <param name="weapon">Weapon to be rendered.</param>
        /// <param name="x">Top left corner x-coordinate of map block.</param>
        /// <param name="y">Top left corner y-coordinate of map block.</param>
        /// <param name="blockSize">Height and width of the block.</param>
        /// <returns>Weapon rendered as path.</returns>
        private Path RenderWeapon(AbstractWeapon weapon, double x, double y, double blockSize)
        {
            return RenderPath(configuration.WeaponPath, x, y, blockSize/3, Color.FromRgb(0,0,0), null);
        }

        /// <summary>
        /// Renders item as a path.
        /// </summary>
        /// <param name="item">Item to be rendered.</param>
        /// <param name="x">Top left corner x-coordinate of map block.</param>
        /// <param name="y">Top left corner y-coordinate of map block.</param>
        /// <param name="blockSize">Height and width of the block.</param>
        /// <returns>Item rendered as a path.</returns>
        private Path RenderItem(AbstractInventoryItem item, double x, double y, double blockSize)
        {
            return RenderPath(configuration.ItemPath, x, y, blockSize / 3, Color.FromRgb(0, 0, 0), null);
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
            // don't render dead creatures
            if (!creature.Alive)
            {
                return new Path();
            }

            if (creature is HumanPlayer)
            {
                return RenderHumanPlayer((AbstractPlayer)creature, x, y, blockSize);
            }
            else if (creature is AbstractPlayer)
            {
                return RenderAIPlayer((AbstractPlayer)creature, x, y, blockSize);
            }
            else if (creature is Monster)
            {
                return RenderMonster((Monster)creature, x, y, blockSize);
            }
            else
            {
                return new Path();
            }
        }

        /// <summary>
        /// Renders monster as a path.
        /// </summary>
        /// <param name="monster">Monster to be rendered.</param>
        /// <param name="x">Top left corner x-coordinate of map block.</param>
        /// <param name="y">Top left cornet y-coordinate of map block.</param>
        /// <param name="blockSize">Size of the block.</param>
        /// <returns></returns>
        private Path RenderMonster(Monster monster, double x, double y, double blockSize)
        {
            return RenderPath(configuration.MonsterPath, x, y, blockSize, Color.FromRgb(200, 0, 20), CreateHpBar(monster));
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
            return RenderPath(configuration.AIPLayerPath, x, y, blockSize, Color.FromRgb(200, 0, 20), CreateHpBar(player));
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
            return RenderPath(configuration.HumanPlayerPath, x, y, blockSize, Color.FromRgb(0, 153, 51), CreateHpBar(player));
        }

        /// <summary>
        /// Renders path as a path object.
        /// </summary>
        /// <param name="path">Path to be rendered</param>
        /// <param name="x">Top left corner x-coordinate of map block.</param>
        /// <param name="y">Top left cornet y-coordinate of map block.</param>
        /// <param name="blockSize">Size of the block.</param>
        /// <param name="strokeColor">Stroke color of the path.</param>
        /// <param name="hpBar">Optional HP bar, leave null if not to be rendered.</param>
        /// <returns>Rendered path</returns>
        private Path RenderPath(String path, double x, double y, double blockSize, Color strokeColor, GeometryGroup hpBar)
        {
            Path renderedPath = new Path();

            GeometryGroup group = new GeometryGroup();
            group.Children.Add(Geometry.Parse(path));
            if (hpBar != null)
            {
                group.Children.Add(hpBar);
            }

            group.Transform = CreateBlockTransform(x, y, blockSize);

            renderedPath.Data = group;
            renderedPath.Stroke = new SolidColorBrush(strokeColor);


            return renderedPath;
        }

        /// <summary>
        /// Creates transformation for particular position and block size.
        /// </summary>
        /// <param name="x">Top left corner x-coordinate of map block.</param>
        /// <param name="y">Top left cornet y-coordinate of map block.</param>
        /// <param name="blockSize">Size of the block.</param>
        /// <returns>Transform group.</returns>
        private TransformGroup CreateBlockTransform(double x, double y, double blockSize)
        {
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(blockSize, blockSize));
            transformGroup.Children.Add(new TranslateTransform(x, y));
            return transformGroup;
        }

        /// <summary>
        /// Creates HP bar for creature rendered as GeometryGroup.
        /// HP bar is represented as a line spanning from 1/3 to 2/3 of bounding rectangle and shortens with player's health. 
        /// 
        /// </summary>
        /// <param name="creature"></param>
        /// <returns></returns>
        private GeometryGroup CreateHpBar(AbstractCreature creature)
        {
            // player HP points scaled to 0..1
            double relativeCreatureHealth = Math.Max(0, creature.CurrentHitPoints / creature.MaxHitPoints);
            GeometryGroup hpBar = new GeometryGroup();
            hpBar.Children.Add(new LineGeometry(new Point(1.0 / 3, 0.05), new Point(1.0 / 3 + relativeCreatureHealth * 1.0 / 3, 0.05)));

            return hpBar;
        }
    }
}
