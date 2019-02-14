using DungeonGame.Common;
using DungeonGame.Render;
using DungeonGame.ViewModel;
using GameCore.Map;
using GameCore.Map.Serializer;
using GameCore.Map.Serializer.Binary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

        private EditorViewModel GetViewModel()
        {
            return (EditorViewModel)DataContext;
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

        /// <summary>
        /// Exports currently edited map to binary file
        /// </summary>
        private void ExportMap()
        {
            if (GetViewModel().GameMap == null)
            {
                MessageBox.Show("V editoru není žádná mapa k uložení.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                ValidateNames = true,
                Filter = "Dungeon map file (*.dmap)|*.dmap|All files (*.*)|*.*"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                try
                {
                    IMapSerializer<byte[], byte[]> serializer = new BinaryMapSerializer();
                    File.WriteAllBytes(fileName, serializer.Serialize(GetViewModel().GameMap));
                } catch (Exception ex)
                {
                    Utils.ShowErrorMessage($"Chyba při exportu mapy do souboru {fileName}: {ex.Message}.");
                }
            }
        }

        /// <summary>
        /// Tries to load map from file to editor.
        /// </summary>
        private void LoadMap()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                ValidateNames = true,
                Filter = "Dungeon map file (*.dmap)|*.dmap|All files (*.*)|*.*"
            };
            string fileName;
            if(openFileDialog.ShowDialog() == true)
            {
                fileName = openFileDialog.FileName;

                // try to load map from file
                Map newMap;
                try
                {
                    byte[] fileContent = File.ReadAllBytes(fileName);
                    newMap = new BinaryMapSerializer().Deserialize(fileContent);
                } catch (Exception ex)
                {
                    Utils.ShowErrorMessage($"Chyba při čtení souboru {fileName}: {ex.Message}. Soubor je poškozený, nebo má neplatný formát.");
                    return;
                }

                // try to load map to editor
                try
                {
                    GetViewModel().LoadMap(newMap);
                    RenderMap();
                } catch (Exception ex)
                {
                    Utils.ShowErrorMessage($"Chyba při načítání mapy do editoru. {ex.Message}"); 
                    return;
                }
            }

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
            EditorViewModel viewModel = GetViewModel();

            // if the map in view model is not null, ask user if he really wants to generate new map
            if ( viewModel.GameMap != null &&
                MessageBox.Show("Opravdu chcete smazat současnou mapu a vygenerovat novou?", "Nová mapa", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) {
                return;
            }

            // check values used to generate new map and don't proceed if they're not correct
            if(!CheckValuesForGenerating())
            {
                return;
            }
            
            viewModel.GenerateMap();
            RenderMap();
        }


        private void EditorMapCanvasClick(object sender, MouseButtonEventArgs e)
        {
            // click position
            double x = e.GetPosition(cMap).X;
            double y = e.GetPosition(cMap).Y;
            EditorViewModel viewModel = GetViewModel();

            // get clicked block coordinates
            int[] mapBlockPos = mapRenderer.CalculateMapBlockPosition(viewModel.GameMap, x, y);

            // place item if possible
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
        }


        private void RemovePlacedItemBtnClick(object sender, RoutedEventArgs e)
        {
            // uid of placed item to be removed is bound to button tag
            int uid;
            string tag = ((Button)sender).Tag.ToString();
            if (!Int32.TryParse(tag, out uid))
            {
                // error for some reason, return
                return;
            }

            GetViewModel().RemovePlacedItem(uid);
            RenderMap();
        }

        private void SaveMenuItemClick(object sender, RoutedEventArgs e)
        {
            ExportMap();
        }

        private void LoadMenuItemClick(object sender, RoutedEventArgs e)
        {
            LoadMap();
        }
    }

    /// <summary>
    /// Class for displaying map in editor with custom MeasureOverride.
    /// </summary>
    class EditorMapCanvas : Canvas
    {
        /// <summary>
        /// Returns size needed to fit all of its' children.
        /// </summary>
        /// <param name="constraint">Constraint.</param>
        /// <returns>Actual size needed for this canvas.</returns>
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
