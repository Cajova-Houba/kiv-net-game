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
            NewGameSettingsWindow newGameSettingsWindow = new NewGameSettingsWindow();
            App.Current.MainWindow = newGameSettingsWindow;
            this.Close();
            newGameSettingsWindow.Show();
        }

        private void OnExitBtnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditorBtnClick(object sender, RoutedEventArgs e)
        {
            EditorWindow editorWindow = new EditorWindow();
            App.Current.MainWindow = editorWindow;
            this.Close();
            editorWindow.Show();
        }

        private void ImportMapBtnClick(object sender, RoutedEventArgs e)
        {
            CustomMapManagementWindow mapManagementWindow = new CustomMapManagementWindow();
            App.Current.MainWindow = mapManagementWindow;
            this.Close();
            mapManagementWindow.Show();
        }
    }
}
