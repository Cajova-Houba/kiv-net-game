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
using DungeonGame.Render;
using DungeonGame.Render.Configuration;

namespace DungeonGame
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private MapRenderer mapRenderer;

        public GameWindow(GameViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
            mapRenderer = new MapRenderer(new RenderConfiguration(), viewModel.GameInstance.GameMap.WinningBlock);
            RenderMap();
        }

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
                maxX = Math.Min(mapW - 1, 2 * recX );
            }
            else if (currPlayerPos.X + recX > mapW - 1)
            {
                // player is too close to the right border
                minX = Math.Max(0, mapW - (2 * recX ) - 1);
                maxX = mapW - 1;
            }
            else
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
                maxY = Math.Min(mapW - 1, 2 * recY );
            }
            else if (currPlayerPos.Y + recY > mapH - 1)
            {
                // player is too close to the bottom border
                minY = Math.Max(0, mapH - (2 * recY ) - 1);
                maxY = mapH - 1;
            }
            else
            {
                // player is not too close to bottom or top border
                minY = currPlayerPos.Y - recY;
                maxY = currPlayerPos.Y + recY;
            }


            // add black background
            gameMapCanvas.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));

            // render map blocks
            // each block will be rendered as square
            //double canvasW = gameMapCanvas.Width;
            //double canvasH = gameMapCanvas.Height;
            double canvasW = gameMapCanvas.ActualWidth > 0 ? gameMapCanvas.ActualWidth : gameMapCanvas.MinWidth;
            double canvasH = gameMapCanvas.ActualHeight > 0 ? gameMapCanvas.ActualHeight : gameMapCanvas.MinHeight;
            //canvasW = 500;
            //canvasH = 500;

            Map gameMap = viewModel.GameMap;
            List<Shape> renderedMap = mapRenderer.RenderMapBlocks(gameMap.Grid, canvasW, canvasH, minX, minY, maxX, maxY);
            gameMapCanvas.Children.Clear();
            foreach (Shape shape in renderedMap)
            {
                gameMapCanvas.Children.Add(shape);
            }
        }

        /// <summary>
        /// Send move action to the game core.
        /// </summary>
        /// <param name="direction">Direciton of the move.</param>
        private void Move(Direction direction)
        {
            GameViewModel viewModel = (GameViewModel)DataContext;
            viewModel.Move(direction);
            try
            {
                viewModel.GameInstance.GameLoopStep();
                RenderMap();
            } catch (Exception ex)
            {
                viewModel.GameMessages.Add(ex.Message);
            }
        }

        private void UpButtonClick(object sender, RoutedEventArgs e)
        {
            Move(Direction.NORTH);
        }

        private void RightButtonClick(object sender, RoutedEventArgs e)
        {
            Move(Direction.EAST);
        }

        private void DownButtonClick(object sender, RoutedEventArgs e)
        {
            Move(Direction.SOUTH);
        }

        private void LeftButtonClick(object sender, RoutedEventArgs e)
        {
            Move(Direction.WEST);
        }
    }

}
