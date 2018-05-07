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
        private VectorMapRenderer mapRenderer;

        public GameWindow(GameViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
            mapRenderer = new VectorMapRenderer(new RenderConfiguration(), viewModel.GameInstance.GameMap.WinningBlock);
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
            
            // add black background
            gameMapCanvas.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));

            // render map blocks
            // each block will be rendered as square
            double canvasW = gameMapCanvas.ActualWidth > 0 ? gameMapCanvas.ActualWidth : gameMapCanvas.MinWidth;
            double canvasH = gameMapCanvas.ActualHeight > 0 ? gameMapCanvas.ActualHeight : gameMapCanvas.MinHeight;

            Map gameMap = viewModel.GameMap;
            List<Shape> renderedMap = mapRenderer.RenderMap(gameMap.Grid, currPlayerPos, canvasW, canvasH);
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
                viewModel.GameLoopStep();
            } catch (Exception ex)
            {
                viewModel.AddGameMessage(ex.Message);
            } finally
            {
                RenderMap();
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
