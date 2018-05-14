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

        public void RenderMap()
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
            EditorViewModel viewModel = (EditorViewModel)DataContext;
            viewModel.GenerateMap();
            DataContext = viewModel;
            RenderMap();
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
