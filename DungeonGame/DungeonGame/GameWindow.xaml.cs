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

namespace DungeonGame
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {

        public String PlayerName { get { return "Jméno hráče"; } }

        /// <summary>
        /// Returns game messages to be displayed in bottom panel.
        /// </summary>
        public ObservableCollection<String> GameMessages { get {
                return new ObservableCollection<string>(new String[] {
                    "Enjoy!",
                    "Game started.",
                    "Players generated.",
                    "Map generated.",
                    "New game initialization."
                });
            } }

        /// <summary>
        /// Returns player's inventory.
        /// </summary>
        public ObservableCollection<InventoryItemModel> Inventory
        {
            get
            {
                return new ObservableCollection<InventoryItemModel>(new InventoryItemModel[]
                {
                    new InventoryItemModel(){ModelObject = new BasicItem("Test item", new MapBlock(), 10)},
                    new InventoryItemModel(){ModelObject = new BasicItem("Test item 2", new MapBlock(), 10)},
                    new InventoryItemModel(){ModelObject = new BasicItem("Some cool item", new MapBlock(), 10)},
                });
            }
        }


        /// <summary>
        /// Returns instance of current player.
        /// </summary>
        public AbstractPlayer Player
        {
            get
            {
                return new HumanPlayer("Test player", new MapBlock());
            }
        }

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
