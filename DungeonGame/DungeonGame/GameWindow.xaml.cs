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
using DungeonGame.ViewModel;
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
        /// <summary>
        /// For rendering the game map.
        /// </summary>
        private IMapRenderer mapRenderer;

        /// <summary>
        /// Timer for 'thread' which will render map to canvas.
        /// </summary>
        protected DispatcherTimer renderToCanvasTimer;

        /// <summary>
        /// Timer for game loop steps.
        /// </summary>
        protected DispatcherTimer gameLoopStepTimer;

        /// <summary>
        /// This window's view model.
        /// </summary>
        protected GameViewModel viewModel;

        /// <summary>
        /// Last time canvas was rendered. Used to set render speed.
        /// </summary>
        protected double lastTimeCanvasRendered;

        /// <summary>
        /// Last time game loop step was performed. Used to set game speed (not necessarily AI speed though).
        /// </summary>
        protected double lastTimeGameLoopStep;

        public GameWindow(GameViewModel viewModel)
        {
            DataContext = viewModel;
            this.viewModel = viewModel;
            InitializeComponent();
            Init();
        }

        public GameWindow()
        {
            InitializeComponent();
            Init();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            PauseGame();

            // ask user if he really wants to exit
            bool close = MessageBox.Show("Opravdu chcete odejít?", "Ukončit hru", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes;
            if (!close) {
                ResumeGame();
                e.Cancel = true;
            } else
            {
                // display menu again
                DisplayMenu();
            }
        }

        /// <summary>
        /// Start timers.
        /// </summary>
        private void ResumeGame()
        {
            if (renderToCanvasTimer != null && !renderToCanvasTimer.IsEnabled)
            {
                renderToCanvasTimer.Start();
            }

            if (gameLoopStepTimer != null && !gameLoopStepTimer.IsEnabled)
            {
                gameLoopStepTimer.Start();
            }
        }

        /// <summary>
        /// Stop game timers.
        /// </summary>
        private void PauseGame()
        {
            if (renderToCanvasTimer != null && renderToCanvasTimer.IsEnabled)
            {
                renderToCanvasTimer.Stop();
            }

            if (gameLoopStepTimer != null && gameLoopStepTimer.IsEnabled)
            {
                gameLoopStepTimer.Stop();
            }
        }

        /// <summary>
        /// Creates new menu window and sets it as current app window.
        /// Does NOT close this window.
        /// </summary>
        private void DisplayMenu()
        {
            MenuWindow menuWindow = new MenuWindow();
            App.Current.MainWindow = menuWindow;
            menuWindow.Show();
        }

        private void Init()
        {
            mapRenderer = new VectorMapRenderer(new RenderConfiguration());

            renderToCanvasTimer = new DispatcherTimer();
            renderToCanvasTimer.Tick += RenderMapFromBufferToCanvas;
            renderToCanvasTimer.Interval = new TimeSpan(100);
            lastTimeCanvasRendered = 0;

            gameLoopStepTimer = new DispatcherTimer();
            gameLoopStepTimer.Tick += PerformGameLoopStep;
            gameLoopStepTimer.Interval = new TimeSpan(100);
            lastTimeGameLoopStep = 0;

            renderToCanvasTimer.Start();
            gameLoopStepTimer.Start();
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
            double lastTime = lastTimeCanvasRendered;
            
            //current time
            double currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            // lock game view, render map and pass rendering task to dispatcher
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
                lastTimeCanvasRendered = currentTime;
            }
        }

        /// <summary>
        /// Thread body which performs one game loop step and renders map to buffer.
        /// </summary>
        private void PerformGameLoopStep(object sender, EventArgs e)
        {
            // game loop steps per second
            double gsps = 10;

            // milliseconds per game loop step
            double mspgs = 1000.0 / gsps;

            // last time game loop step was performed
            double lastTime = lastTimeGameLoopStep;
            
            GameViewModel gameView = viewModel;

            //current time
            double currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            // lock game view, perform game loop step and pass task with data context update
            if ((currentTime - lastTime) >= mspgs)
            {
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
                if (gameView.GameInstance.IsWinner)
                {
                    ((DispatcherTimer)sender).Stop();
                    viewModel.AddGameMessage($"Player {gameView.GameInstance.Winner.Name} has won!");
                }
                lastTimeGameLoopStep = currentTime;
                Application.Current.Dispatcher.Invoke(() => {
                    viewModel = gameView;
                    DataContext = viewModel;
                    gameView.NotifyPropertyChanges();
                });
                Monitor.Exit(gameView);
            }
        }

        /// <summary>
        /// Send move action to the game core.
        /// </summary>
        /// <param name="direction">Direciton of the move.</param>
        private void Move(Direction direction)
        {
            GameViewModel viewModel = (GameViewModel)DataContext;

            // if the given direction is occupied, attack instead
            MapBlock mb = viewModel.Player.Position.NextBlock(direction);
            if (mb != null && mb.Occupied)
            {
                Attack(direction);
            } else
            {
                viewModel.Move(direction);
            }

        }

        /// <summary>
        /// Attack in given direction.
        /// </summary>
        /// <param name="direction">Direction of attack.</param>
        private void Attack(Direction direction)
        {
            GameViewModel viewModel = (GameViewModel)DataContext;
            viewModel.Attack(direction);
        }

        /// <summary>
        /// Pick ups item on the current block.
        /// </summary>
        private void PickUp()
        {
            GameViewModel viewModel = (GameViewModel)DataContext;
            viewModel.PickUp();
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

        private void PickUpButtonClick(object sender, RoutedEventArgs e)
        {
            PickUp();
        }

        /// <summary>
        /// Key press on main window, used to move around map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case System.Windows.Input.Key.Up:
                    Move(Direction.NORTH);
                    break;

                case System.Windows.Input.Key.Right:
                    Move(Direction.EAST);
                    break;

                case System.Windows.Input.Key.Down:
                    Move(Direction.SOUTH);
                    break;

                case System.Windows.Input.Key.Left:
                    Move(Direction.WEST);
                    break;

                case System.Windows.Input.Key.Space:
                    PickUp();
                    break;
            }
        }
       
    }
   
}
