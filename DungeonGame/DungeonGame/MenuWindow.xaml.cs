using DungeonGame.Model;
using GameCore.Game;
using GameCore.Map;
using GameCore.Map.Generator;
using GameCore.Objects.Creatures;
using GameCore.Objects.Creatures.AIPlayers;
using GameCore.Objects.Items;
using GameCore.Objects.Items.Armors;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DungeonGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();
        }

        private void newGameBtn_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow(new GameViewModel(GenerateNewGame()));
            App.Current.MainWindow = gameWindow;
            this.Close();
            gameWindow.Show();
        }

        private Game GenerateNewGame()
        {
            Map gameMap = MapGeneratorFactory.CreateSimpleMapGenerator().GenerateMap(10, 10, 123456);
            AbstractPlayer player = new HumanPlayer("Test player", gameMap.Grid[2, 2]);
            player.Inventory.AddRange(new BasicItem[]
            {
                new BasicItem("Test item", new MapBlock(), 10),
                new BasicItem("Test item 2", new MapBlock(), 10),
                new BasicItem("Some cool item", new MapBlock(), 10)
            });

            Game game = new Game() { GameMap = gameMap };
            game.AddHumanPlayer(player);
            game.AddAIPlayer(AIPlayerFactory.CreateSimpleAIPLayer("Test AI Player", gameMap.Grid[3, 3]));
            new LeatherArmor("Leather armor", game.GameMap.Grid[1, 1]);

            return game;
        }
    }
}
