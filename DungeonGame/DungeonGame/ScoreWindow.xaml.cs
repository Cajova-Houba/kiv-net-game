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
    /// Window used to show total score after game is over.
    /// </summary>
    public partial class ScoreWindow : Window
    {
        public ScoreWindow()
        {
            DataContext = new ScoreViewModel();
            InitializeComponent();
        }

        /// <summary>
        /// Initializes this window with given model.
        /// </summary>
        /// <param name="viewModel">View model to be used as this window's data context.</param>
        public ScoreWindow(ScoreViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void CloseBtnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
