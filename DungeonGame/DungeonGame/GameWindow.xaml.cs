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

namespace DungeonGame
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {

        public ObservableCollection<String> GameMessages { get {
                return new ObservableCollection<string>(new String[] {
                    "Enjoy!",
                    "Game started.",
                    "Players generated.",
                    "Map generated.",
                    "New game initialization."
                });
            } set { } }

        public GameWindow()
        {
            DataContext = this;
            InitializeComponent();
            DrawPlaceholderMap();
        }

        private void DrawPlaceholderMap()
        {
            //Uri mapResourceUri = new Uri(@"\img\map-placeholder.jpg");
            Uri mapResourceUri = new Uri("pack://application:,,,/img/map-placeholder.jpg");
            BitmapImage placeholderMap = new BitmapImage(mapResourceUri);
            gameMap.Background = new ImageBrush(placeholderMap);
        }
    }
}
