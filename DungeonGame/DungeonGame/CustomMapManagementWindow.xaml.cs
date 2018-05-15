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

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisplayMenu();
        }
    }
}
