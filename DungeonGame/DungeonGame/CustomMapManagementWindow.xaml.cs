using DungeonGame.ViewModel;
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

        /// <summary>
        /// Shows simple error message dialog with OK button.
        /// </summary>
        /// <param name="errorMessage"></param>
        private void ShowErrorMessage(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
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
                ShowErrorMessage($"Chyba při obnovování map! {ex.Message}");
            }
        }
    }
}
