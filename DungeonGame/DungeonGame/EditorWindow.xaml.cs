using DungeonGame.Common;
using DungeonGame.Render;
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
    /// Window used to create game map.
    /// </summary>
    public partial class EditorWindow : Window
    {
        private IMapRenderer mapRenderer;

        public EditorWindow()
        {
            InitializeComponent();
            mapRenderer = new VectorMapRenderer(new Render.Configuration.RenderConfiguration());
            RenderMap();
        }

        /// <summary>
        /// Renders map from view model to canvas.
        /// </summary>
        private void RenderMap()
        {
            EditorViewModel model = (EditorViewModel)DataContext;
            if (model.GameMap != null)
            {
                List<UIElement> renderedMap = mapRenderer.RenderWholeMap(model.GameMap);
                cMap.Children.Clear();
                renderedMap.ForEach(uiElement => cMap.Children.Add(uiElement));
            }
        }

        /// <summary>
        /// Displays menu window. Does not cancel this window.
        /// </summary>
        private void DisplayMenu()
        {
            MenuWindow menuWindow = new MenuWindow();
            App.Current.MainWindow = menuWindow;
            menuWindow.Show();
        }

        /// <summary>
        /// Checks that all values used to generate map are ok.
        /// </summary>
        /// <returns>True if values are ok.</returns>
        private bool CheckValuesForGenerating()
        {
            if (!Utils.CheckRangedInt(tbMapWidth.Text, ViewModelConstants.MIN_MAP_WIDTH, ViewModelConstants.MAX_MAP_WIDTH, "Šířka mapy není platná hodnota.", $"Šířka mapy musí být v rozsahu {ViewModelConstants.MIN_MAP_WIDTH}-{ViewModelConstants.MAX_MAP_WIDTH}."))
            {
                return false;
            }

            if (!Utils.CheckRangedInt(tbMapHeight.Text, ViewModelConstants.MIN_MAP_WIDTH, ViewModelConstants.MAX_MAP_WIDTH, "Výška mapy není platná hodnota.", $"Výška mapy musí být v rozsahu {ViewModelConstants.MIN_MAP_HEIGHT}-{ViewModelConstants.MAX_MAP_HEIGHT}."))
            {
                return false;
            }

            int res;
            if (!Int32.TryParse(tbMapSeed.Text, out res))
            {
                MessageBox.Show("Seed pro mapu není platná hodnota.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void OnEditorClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisplayMenu();
        }

        private void CloseMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GenerateBtnClick(object sender, RoutedEventArgs e)
        {
            if(!CheckValuesForGenerating())
            {
                return;
            }

            EditorViewModel viewModel = (EditorViewModel)DataContext;
            viewModel.GenerateMap();
            DataContext = viewModel;
            RenderMap();
        }

        private void EditorMapCanvasClick(object sender, MouseButtonEventArgs e)
        {
            double x = e.GetPosition(cMap).X;
            double y = e.GetPosition(cMap).Y;
            EditorViewModel viewModel = (EditorViewModel)DataContext;

            int[] mapBlockPos = mapRenderer.CalculateMapBlockPosition(viewModel.GameMap, x, y);

            if(mapBlockPos[0] != -1 && mapBlockPos[1] != -1 && viewModel.SelectedToolboxItem != null)
            {
                try
                {
                    viewModel.PlaceSelectedToolboxItem(mapBlockPos[0], mapBlockPos[1]);
                    viewModel.DeselectToolboxItem();
                    RenderMap();
                } catch (Exception ex)
                {
                    MessageBox.Show($"Chyba: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            //MessageBox.Show($"kliknuto na blok {mapBlockPos[0]},{mapBlockPos[1]}");
        }
        
    }

    /// <summary>
    /// Class for displaying map in editor with custom MeasureOverride.
    /// </summary>
    class EditorMapCanvas : Canvas
    {
        protected override Size MeasureOverride(Size constraint)
        {
            base.MeasureOverride(constraint);

            var desiredSize = new Size();
            foreach (UIElement child in Children)
            {
                double left = GetLeft(child);
                left = Double.IsNaN(left) ? 0 : left;
                double top = GetTop(child);
                top = Double.IsNaN(top) ? 0 : top;

                desiredSize = new Size(
                    Math.Max(desiredSize.Width, left + child.DesiredSize.Width),
                    Math.Max(desiredSize.Height, top + child.DesiredSize.Height));
            }
            return desiredSize;
        }
    }
}
