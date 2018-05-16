using DungeonGame.Common;
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
        public const int MIN_AI_PLAYER_COUNT = 0;
        public const int MAX_AI_PLAYER_COUNT = 100;
        public const int MIN_MONSTER_COUNT = 0;
        public const int MAX_MONSTER_COUNT = 100;

        /// <summary>
        /// Flag used to control Closing event.
        /// </summary>
        private bool displayGameWindow = false;

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

            Game game;
            try
            {
                game = GenerateNewGame((NewGameSettingsModel)DataContext);
            } catch (Exception ex)
            {
                Utils.ShowErrorMessage($"Chyba při vytváření mapy. {ex.Message}");
                return;
            }
            GameWindow gameWindow = new GameWindow(new GameViewModel(game));
            App.Current.MainWindow = gameWindow;
            displayGameWindow = true;
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
            if (!Utils.CheckRangedInt(tbMapWidth.Text, ViewModelConstants.MIN_MAP_WIDTH, ViewModelConstants.MAX_MAP_WIDTH, "Šířka mapy není platná hodnota.", $"Šířka mapy musí být v rozsahu {ViewModelConstants.MIN_MAP_WIDTH}-{ViewModelConstants.MAX_MAP_WIDTH}."))
            {
                return false;
            }

            if (!Utils.CheckRangedInt(tbMapHeight.Text, ViewModelConstants.MIN_MAP_WIDTH, ViewModelConstants.MAX_MAP_WIDTH, "Výška mapy není platná hodnota.", $"Výška mapy musí být v rozsahu {ViewModelConstants.MIN_MAP_HEIGHT}-{ViewModelConstants.MAX_MAP_HEIGHT}."))
            {
                return false;
            }

            if (!Int32.TryParse(tbMapSeed.Text, out res))
            {
                Utils.ShowErrorMessage("Seed pro mapu není platná hodnota.");
                return false;
            }

            if (!Utils.CheckRangedInt(tbAiCount.Text, MIN_AI_PLAYER_COUNT, MAX_AI_PLAYER_COUNT, "Počet protihráčů není platná hodnota.", $"Počet protihráčů musí být v rozsahu {MIN_AI_PLAYER_COUNT}-{MAX_AI_PLAYER_COUNT}."))
            {
                return false;
            }

            if (tbPlayerName.Text == null || tbPlayerName.Text.Length == 0 || tbPlayerName.Text.Length > ViewModelConstants.MAX_PLAYER_NAME_LEN)
            {
                Utils.ShowErrorMessage($"Jméno hráče neobsahuje platnout hodnotu. Jméno hráče musí být řetězec délky 1..{ViewModelConstants.MAX_PLAYER_NAME_LEN}.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Display menu. Does not close this window.
        /// </summary>
        private void DisplayMenu()
        {
            MenuWindow menuWindow = new MenuWindow();
            App.Current.MainWindow = menuWindow;
            menuWindow.Show();
        }

        /// <summary>
        /// Generates new game from given settings.
        /// </summary>
        /// <param name="model">Model containing game settings.</param>
        /// <returns>Generated game.</returns>
        private Game GenerateNewGame(NewGameSettingsModel model)
        {
            string playerName = model.PlayerName;

            Game game;
            if (rbRandomMap.IsChecked.HasValue && rbRandomMap.IsChecked.Value == true)
            {
                int w = model.MapWidth;
                int h = model.MapHeight;
                int seed = model.MapSeed;
                int aiCount = model.AiCount;
                double monsterDensity = model.NormalizedMonsterDensity;
                double itemDensity = model.NormalizedItemDensity;
                game = GameGenerator.GenerateGame(w, h, seed, aiCount, monsterDensity, itemDensity, playerName); 
            } else if (rbImportedMap.IsChecked.HasValue && rbImportedMap.IsChecked.Value == true)
            {
                game = GameGenerator.GenerateGame(model.SelectedImportedMap.Map, playerName);
            } else
            {
                throw new Exception("Nebyla vybrána žádná volba nastavení mapy!");
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
            this.Close();
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

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!displayGameWindow)
            {
                DisplayMenu();
            }
        }
    }
}
