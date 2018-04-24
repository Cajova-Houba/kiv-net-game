using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GameCore.Objects.Items;
using DungeonGame.Model;
using GameCore.Map;
using GameCore.Objects.Creatures;
using GameCore.Map.Generator;

namespace DungeonGame
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();
            //DrawPlaceholderMap();
            RenderMap();
        }

        private void DrawPlaceholderMap()
        {
            //Uri mapResourceUri = new Uri(@"\img\map-placeholder.jpg");
            Uri mapResourceUri = new Uri("pack://application:,,,/img/map-placeholder.jpg");
            BitmapImage placeholderMap = new BitmapImage(mapResourceUri);
            gameMapCanvas.Background = new ImageBrush(placeholderMap);
        }

        /// <summary>
        /// Render map to the canvas component
        /// </summary>
        private void RenderMap()
        {
            // 5x5 block around player
            // 2 + player block + 2 = 2*rec + 1
            int recX = 2;
            int recY = 2;

            GameViewModel viewModel = (GameViewModel)DataContext;
            // get current player position
            MapBlock currPlayerPos = viewModel.Player.Position;

            // get blocks around player to be rendered
            // select area around player so that player is in the center of that area
            // if that's not possible (corners, borders) display arrea of same size with player somewhere in that area
            int mapW = viewModel.GameMap.Width;
            int mapH = viewModel.GameMap.Height;
            if (mapW <= 0 || mapH <= 0)
            {
                // nothing to render
                return;
            }
            int minX, maxX, minY, maxY;

            // min/max x boundaries
            if (currPlayerPos.X - recX < 0)
            {
                // player is too close to the left border
                minX = 0;
                maxX = Math.Min(mapW - 1, 2 * recX + 1);
            } else if (currPlayerPos.X + recX > mapW - 1)
            {
                // player is too close to the right border
                minX = Math.Max(0, mapW - (2 * recX + 1));
                maxX = mapW - 1;
            } else
            {
                // player is not too close to left or right border
                minX = currPlayerPos.X - recX;
                maxX = currPlayerPos.X + recX;
            }

            // min/max y boundaries
            if (currPlayerPos.Y - recY < 0)
            {
                // player is too close to the top border
                minY = 0;
                maxY = Math.Min(mapW - 1, 2 * recY + 1);
            }
            else if (currPlayerPos.Y + recY > mapH - 1)
            {
                // player is too close to the bottom border
                minY = Math.Max(0, mapH - (2 * recY + 1));
                maxY = mapH - 1;
            }
            else
            {
                // player is not too close to bottom or top border
                minY = currPlayerPos.Y - recY;
                maxY = currPlayerPos.Y + recY;
            }


            // render background 
            gameMapCanvas.Background = new SolidColorBrush(Color.FromRgb(0,0,0));

            // render map blocks
            // each block will be rendered as square
            //double canvasW = gameMapCanvas.Width;
            //double canvasH = gameMapCanvas.Height;
            double canvasW = 530;
            double canvasH = 418;
            int blockCount = Math.Max(maxX + 1 - minX, maxY + 1 - minY);
            double blockSize = Math.Min(canvasW / (double)blockCount, canvasH / (double)blockCount);
            gameMapCanvas.Children.Add(new Rectangle() { Height = blockCount*blockSize, Width = blockCount*blockSize, Fill = new SolidColorBrush(Color.FromRgb(255,204,102) )});

            Map gameMap = viewModel.GameMap;
            for(int i = minX; i <= maxX; i++)
            {
                for(int j = minY; j <= maxY; j++)
                {
                    List<Shape> renderedMapBlock = RenderMapBlock(gameMap.Grid[i, j], i * blockSize, j * blockSize, blockSize);
                    foreach(Shape shape in renderedMapBlock)
                    {
                        gameMapCanvas.Children.Add(shape);
                    }
                }
            }
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
            renderedBlock.AddRange(RenderHorizontalEntrance(mapBlock.North, x, y, entranceSize));
            renderedBlock.AddRange(RenderHorizontalEntrance(mapBlock.South, x, y+blockSize, entranceSize));
            renderedBlock.AddRange(RenderVerticalEntrance(mapBlock.East, x+blockSize, y, entranceSize));
            renderedBlock.AddRange(RenderVerticalEntrance(mapBlock.West, x, y, entranceSize));

            if(mapBlock.Creature != null)
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
        /// <returns>Entrance rendered as lines.</returns>
        private List<Line> RenderHorizontalEntrance(Entrance entrance, double x, double y, double entranceSize)
        {
            List<Line> entranceLines = new List<Line>();
            Brush b = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            if (entrance != null && entrance.IsOpen())
            {
                // line - nothing - line
                entranceLines.Add(new Line() { X1 = x, Y1 = y, X2 = x + entranceSize, Y2 = y , Stroke = b, StrokeThickness = 1});
                entranceLines.Add(new Line() { X1 = x + 2 * entranceSize, Y1 = y, X2 = x + 3 * entranceSize, Y2 = y, Stroke = b, StrokeThickness = 1 });
            }
            else
            {
                entranceLines.Add(new Line() { X1 = x, Y1 = y, X2 = x + entranceSize*3, Y2 = y, Stroke = b, StrokeThickness = 1 });
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
        /// <returns>Entrance rendered as lines.</returns>
        private List<Line> RenderVerticalEntrance(Entrance entrance, double x, double y, double entranceSize)
        {
            List<Line> entranceLines = new List<Line>();
            Brush b = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            if (entrance != null && entrance.IsOpen())
            {
                // line - nothing - line
                entranceLines.Add(new Line() { X1 = x, Y1 = y, X2 = x, Y2 = y + entranceSize, Stroke = b, StrokeThickness = 1 });
                entranceLines.Add(new Line() { X1 = x, Y1 = y + 2 * entranceSize, X2 = x, Y2 = y + 3 * entranceSize, Stroke = b, StrokeThickness = 1 });
            } else
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
            } else
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

            LineGeometry firstLeg = new LineGeometry(new Point(1 / 3.0, 1), new Point(0.5, 2 / 3.0));
            LineGeometry secondLeg = new LineGeometry(new Point(2 / 3.0, 1), new Point(0.5, 2 / 3.0));
            LineGeometry body = new LineGeometry(new Point(0.5, 2 / 3.0), new Point(0.5, 1 / 6.0));
            LineGeometry firstHand = new LineGeometry(new Point(0.5, 1 / 3.0), new Point(1 / 3.0, 0.5));
            LineGeometry secondHand = new LineGeometry(new Point(0.5, 1 / 3.0), new Point(2 / 3.0, 0.5));
            EllipseGeometry head = new EllipseGeometry(new Point(0.5, 1 / 6.0), 1 / 10.0, 1 / 10.0);

            GeometryGroup group = new GeometryGroup();
            group.Children.Add(firstLeg);
            group.Children.Add(secondLeg);
            group.Children.Add(body);
            group.Children.Add(firstHand);
            group.Children.Add(secondHand);
            group.Children.Add(head);

            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(blockSize, blockSize));
            transformGroup.Children.Add(new TranslateTransform(x, y));
            group.Transform = transformGroup;

            renderedPlayer.Data = group;
            renderedPlayer.Stroke = new SolidColorBrush(Color.FromRgb(0, 153, 51));
            renderedPlayer.StrokeThickness = 1;
            renderedPlayer.Fill = new SolidColorBrush(Color.FromRgb(0, 153, 51));



            return renderedPlayer;
        }
    }
    

}
