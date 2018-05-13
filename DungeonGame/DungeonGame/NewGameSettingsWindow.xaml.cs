using DungeonGame.ViewModel;
using GameCore.Game;
using GameCore.Map;
using GameCore.Map.Generator;
using GameCore.Objects.Creatures;
using GameCore.Objects.Creatures.AIPlayers;
using GameCore.Objects.Items;
using GameCore.Objects.Items.Armors;
using GameCore.Objects.Items.Weapons;
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
using System.Windows.Shapes;

namespace DungeonGame
{
    /// <summary>
    /// Window used to set up parameter for new game. 
    /// </summary>
    public partial class NewGameSettingsWindow : Window
    {
        public NewGameSettingsWindow()
        {
            InitializeComponent();
        }

        private void StartGameBtnClick(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow(new GameViewModel(GenerateNewGame((NewGameSettingsModel)DataContext)));
            App.Current.MainWindow = gameWindow;
            this.Close();
            gameWindow.Show();
        }

        /// <summary>
        /// Generates new game from given settings.
        /// </summary>
        /// <param name="model">Model containing game settings.</param>
        /// <returns>Generated game.</returns>
        private Game GenerateNewGame(NewGameSettingsModel model)
        {
            int w = model.MapWidth;
            int h = model.MapHeight;
            int seed = model.MapSeed;
            int aiCount = model.AiCount;
            int monsterCount = model.MonsterCount;
            ItemsDensity itemsDensity = model.SelectedDensity;

            // sets of occupied position for creatures and items, '{x}:{y}'
            HashSet<String> creatureOccupiedPositions = new HashSet<string>();
            HashSet<string> itemsOccupiedPositions = new HashSet<string>();

            Random r = new Random();
            Map gameMap = MapGeneratorFactory.CreateSimpleMapGenerator().GenerateMap(w, h, seed);
            Game game = new Game() { GameMap = gameMap };

            // place human player
            int x = r.Next(w);
            int y = r.Next(h);
            AbstractPlayer player = new HumanPlayer(model.PlayerName, gameMap.Grid[x, y]);
            game.AddHumanPlayer(player);
            creatureOccupiedPositions.Add($"{x}:{y}");
            
            // place AI players
            for(int i = 0; i < aiCount; i++)
            {
                // keep generating positions until player is placed or limit of tries is reached
                int maxTries = 10;
                int tries = 0;
                bool placed = false;
                while(!placed && (tries < maxTries))
                {
                    tries++;
                    x = r.Next(w);
                    y = r.Next(h);
                    if (!creatureOccupiedPositions.Contains($"{x}:{y}")) {
                        game.AddAIPlayer(AIPlayerFactory.CreateSimpleAIPLayer($"Simple AI Player {i + 1}", gameMap.Grid[x, y]));
                        creatureOccupiedPositions.Add(($"{x}:{y}"));
                        placed = true;
                    }
                }
                
                if (!placed)
                {
                    // todo: some error message
                }
            }
            
            // place monsters
            for(int i = 0; i < monsterCount; i++)
            {
                // keep generating positions until player is placed or limit of tries is reached
                int maxTries = 10;
                int tries = 0;
                bool placed = false;
                while (!placed && (tries < maxTries))
                {
                    tries++;
                    x = r.Next(w);
                    y = r.Next(h);
                    if (!creatureOccupiedPositions.Contains($"{x}:{y}"))
                    {
                        game.AddMonster(MonsterFactory.CreateRandomMonster(gameMap.Grid[x, y]));
                        creatureOccupiedPositions.Add(($"{x}:{y}"));
                        placed = true;
                    }
                }

                if (!placed)
                {
                    // todo: some error message
                }
            }

            // place items
            int itemCount = r.Next((w * h) / (10 * Math.Max(itemsDensity.Value,1)));
            for(int i = 0; i < itemCount; i++)
            {
                // keep generating positions until item is placed or limit of tries is reached
                int maxTries = 10;
                int tries = 0;
                bool placed = false;
                while (!placed && (tries < maxTries))
                {
                    tries++;
                    x = r.Next(w);
                    y = r.Next(h);
                    if (!itemsOccupiedPositions.Contains($"{x}:{y}"))
                    {
                        game.AddItem(ItemFactory.CreateRandomItem(gameMap.Grid[x,y]));
                        itemsOccupiedPositions.Add(($"{x}:{y}"));
                        placed = true;
                    }
                }

                if (!placed)
                {
                    // todo: some error message
                }
            }
            return game;
        }

        /// <summary>
        /// Closes this window and displays menu again.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackBtnClick(object sender, RoutedEventArgs e)
        {
            MenuWindow menuWindow = new MenuWindow();
            App.Current.MainWindow = menuWindow;
            this.Close();
            menuWindow.Show();
        }
    }
}
