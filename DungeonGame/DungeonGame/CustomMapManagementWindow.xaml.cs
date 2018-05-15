using DungeonGame.Common;
using DungeonGame.ViewModel;
using Microsoft.Win32;
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
    /// Window used to manage custom maps.
    /// </summary>
    public partial class CustomMapManagementWindow : Window
    {
        public CustomMapManagementWindow()
        {
            DataContext = new MapManagementViewModel();
            InitializeComponent();
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

        private MapManagementViewModel GetViewModel()
        {
            return (MapManagementViewModel)DataContext;
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisplayMenu();
        }

        private void RefreshMapsBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                int failedMaps = GetViewModel().RefreshImportedMaps();
                MessageBox.Show($"Import map dokončen. Počet neúspěšně zpracovaných souborů: {failedMaps}.", "Import dokončen", MessageBoxButton.OK, MessageBoxImage.Information);
            } catch (Exception ex)
            {
                Utils.ShowErrorMessage($"Chyba při obnovování map! {ex.Message}");
            }
        }

        private void RemoveImportedMapBtnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button) || ((Button)sender).Tag == null)
            {
                return;
            }

            if (MessageBox.Show("Opravdu chcete smazat soubor s mapou?", "Smazat mapu", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    GetViewModel().RemoveImportedMap(((Button)sender).Tag.ToString());
                } catch (Exception ex)
                {
                    Utils.ShowErrorMessage($"Chyba při odstraňování mapy: {ex.Message}");
                }
            }
        }

        private void LoadMapBtnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                ValidateNames = true,
                Filter = "Dungeon map file (*.dmap)|*.dmap|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                GetViewModel().FileName = openFileDialog.FileName;
            }
        }

        private void ImportMapBtnClick(object sender, RoutedEventArgs e)
        {
            string fName = GetViewModel().FileName;
            try
            {
                GetViewModel().ImportMapFromCurrentFile();
            } catch (Exception ex)
            {
                Utils.ShowErrorMessage($"Chyba při importování mapy ze souboru {fName}. {ex.Message}");
            }
        }
    }
}
