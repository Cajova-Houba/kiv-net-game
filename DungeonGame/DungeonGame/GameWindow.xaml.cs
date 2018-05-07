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
using System.Threading;
using System.ComponentModel;
using System.Windows.Threading;

namespace DungeonGame
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private IMapRenderer mapRenderer;

        /// <summary>
        /// Buffer for map canvas. One thread will render map after game loop,
        /// another one will then pull items from this buffer and puts them to canvas. 
        /// </summary>
        protected List<UIElement> canvasBuffer;

        protected DispatcherTimer renderToCanvasTimer;

        protected Thread renderToCanvasThread;

        protected Thread gameThread;

        protected GameViewModel viewModel;

        protected int lastTimeCanvasRendered;

        public GameWindow(GameViewModel viewModel)
        {
            DataContext = viewModel;
            this.viewModel = viewModel;
            InitializeComponent();
            mapRenderer = new VectorMapRenderer(new RenderConfiguration());
            canvasBuffer = new List<UIElement>();
            //RenderMap();
            //renderToCanvasThread = new Thread(RenderMapFromBufferToCanvas);
            //renderToCanvasThread.SetApartmentState(ApartmentState.STA);

            renderToCanvasTimer = new DispatcherTimer();
            renderToCanvasTimer.Tick += RenderMapFromBufferToCanvas;
            renderToCanvasTimer.Interval = new TimeSpan(100);
            lastTimeCanvasRendered = 0;

            gameThread = new Thread(PerformGameLoopStep);
            gameThread.SetApartmentState(ApartmentState.STA);

            //renderToCanvasThread.Start();
            renderToCanvasTimer.Start();
            gameThread.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (renderToCanvasThread != null && renderToCanvasThread.IsAlive)
            {
                renderToCanvasThread.Abort();
            }

            if(renderToCanvasTimer != null && renderToCanvasTimer.IsEnabled)
            {
                renderToCanvasTimer.Stop();
            }

            if (gameThread.IsAlive)
            {
                gameThread.Abort();
            }
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
        /// Timer body which renders map from buffer to canvas. Uses monitor to lock canvasBufferCollection.
        /// </summary>
        private void RenderMapFromBufferToCanvas(object sender, EventArgs e)
        {
            // render 10 frames per second
            int fps = 10;

            // milliseconds per frame
            double mspf = 1000.0 / fps;

            // last time map was rendered
            int lastTime = lastTimeCanvasRendered;
            
            //current time
            double currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            // lock buffer and render stuff from buffer
            if ((currentTime - lastTime) >= mspf)
            {
                GameViewModel gameView = viewModel;
                while(!Monitor.TryEnter(gameView))
                {
                    Thread.Sleep(100);
                }
                List<UIElement> renderedMap = mapRenderer.RenderMap(gameView.GameMap, gameView.Player.Position, gameMapCanvas.ActualWidth, gameMapCanvas.ActualHeight);
                Monitor.Exit(gameView);

                Application.Current.Dispatcher.Invoke(() => {
                    gameMapCanvas.Children.Clear();
                    foreach(UIElement element in renderedMap) {
                        gameMapCanvas.Children.Add(element);
                    }
                });

                // update last rendered time
                lastTimeCanvasRendered += (int)currentTime;
            }
        }

        /// <summary>
        /// Thread body which performs one game loop step and renders map to buffer.
        /// </summary>
        private void PerformGameLoopStep()
        {
            // perform 1 game loop step every 5 per seconds => 0.2 game loop steps per second
            double gsps = 1.0/5;

            // milliseconds per game loop step
            double mspgs = 1000.0 / gsps;

            // last time game loop step was performed
            int lastTime = 0;

            // game step loop
            bool stopLoop = false;
            while (!stopLoop)
            {
                GameViewModel gameView = viewModel;

                //current time
                // todo: fix time measurement
                double currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                if ((currentTime - lastTime) >= mspgs)
                {
                    // lock game view and perform game loop step and update properties
                    while (!Monitor.TryEnter(gameView))
                    {
                        Thread.Sleep(100);
                    }
                    try
                    {
                        gameView.GameLoopStep();
                    } catch(Exception ex)
                    {
                        // todo: something
                    }
                    gameView.NotifyPropertyChanges();
                    stopLoop = gameView.GameInstance.IsWinner;
                    Monitor.Exit(gameView);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
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
            List<UIElement> renderedMap = mapRenderer.RenderMap(gameMap, currPlayerPos, canvasW, canvasH);
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
            //try
            //{
            //    viewModel.GameLoopStep();
            //} catch (Exception ex)
            //{
            //    viewModel.AddGameMessage(ex.Message);
            //} finally
            //{
            //    RenderMap();
            //}
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
