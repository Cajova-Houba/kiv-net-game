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
using System.Text.RegularExpressions;
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
        public const int MIN_MAP_WIDTH = 3;
        public const int MAX_MAP_WIDTH = 100;
        public const int MIN_MAP_HEIGHT = 3;
        public const int MAX_MAP_HEIGHT = 100;
        public const int MIN_AI_PLAYER_COUNT = 0;
        public const int MAX_AI_PLAYER_COUNT = 100;
        public const int MIN_MONSTER_COUNT = 0;
        public const int MAX_MONSTER_COUNT = 100;


        public NewGameSettingsWindow()
        {
            InitializeComponent();
        }

        private void StartGameBtnClick(object sender, RoutedEventArgs e)
        {
            if(!CheckValues())
            {
                return;
            }
            GameWindow gameWindow = new GameWindow(new GameViewModel(GenerateNewGame((NewGameSettingsModel)DataContext)));
            App.Current.MainWindow = gameWindow;
            this.Close();
            gameWindow.Show();
        }

        /// <summary>
        /// Checks that all field contain right values.
        /// </summary>
        /// <returns></returns>
        private bool CheckValues()
        {
            int res = 0;
            if (!CheckRangedInt(tbMapWidth.Text, MIN_MAP_WIDTH, MAX_MAP_WIDTH, "Šířka mapy není platná hodnota.", $"Šířka mapy musí být v rozsahu {MIN_MAP_WIDTH}-{MAX_MAP_WIDTH}."))
            {
                return false;
            }

            if (!CheckRangedInt(tbMapHeight.Text, MIN_MAP_WIDTH, MAX_MAP_WIDTH, "Výška mapy není platná hodnota.", $"Výška mapy musí být v rozsahu {MIN_MAP_HEIGHT}-{MAX_MAP_HEIGHT}."))
            {
                return false;
            }

            if (!Int32.TryParse(tbMapSeed.Text, out res))
            {
                MessageBox.Show("Seed pro mapu není platná hodnota.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!CheckRangedInt(tbAiCount.Text, MIN_AI_PLAYER_COUNT, MAX_AI_PLAYER_COUNT, "Počet protihráčů není platná hodnota.", $"Počet protihráčů musí být v rozsahu {MIN_AI_PLAYER_COUNT}-{MAX_AI_PLAYER_COUNT}."))
            {
                return false;
            }

            if (!CheckRangedInt(tbMonsterCount.Text, MIN_MONSTER_COUNT, MAX_MONSTER_COUNT, "Počet monster není platná hodnota.", $"Počet monster musí být v rozsahu {MIN_MONSTER_COUNT}-{MAX_MONSTER_COUNT}."))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to parse integer from source and checks it's range.
        /// </summary>
        /// <param name="source">String with integer to be checked.</param>
        /// <param name="min">Min allowed value.</param>
        /// <param name="max">Max allowed value.</param>
        /// <param name="parseErrorMessage">Error message to be displayed if error occurs during parsing.</param>
        /// <param name="rangeErrorMessage">Error message to be displayed if error occurs during rarnge check.</param>
        /// <returns>True if source is ok integer.</returns>
        private bool CheckRangedInt(string source, int min, int max, string parseErrorMessage, string rangeErrorMessage)
        {
            int res = 0;
            if (!Int32.TryParse(source, out res))
            {
                MessageBox.Show(parseErrorMessage, "Chybe", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            } else
            {
                if (res < min || res > max)
                {
                    MessageBox.Show(rangeErrorMessage, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
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
            itemCount = r.Next((w * h) / 10) + ((w*h)/10 * Math.Max(itemsDensity.Value, 1));
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

        /// <summary>
        /// Returns true if passed text is a number.
        /// </summary>
        /// <param name="text">Text containing number</param>
        /// <returns>True if the text is number.</returns>
        private bool ValidateNumberInput(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }

        private void NumberTextBoxPreview(object sender, TextCompositionEventArgs e)
        {
            e.Handled = ValidateNumberInput(e.Text);
        }
    }
}
