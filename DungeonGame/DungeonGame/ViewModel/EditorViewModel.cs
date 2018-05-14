using GameCore.Map;
using GameCore.Map.Generator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DungeonGame.ViewModel
{
    /// <summary>
    /// View model for editor window.
    /// </summary>
    public class EditorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int mapWidth;
        public int MapWidth
        {
            get { return mapWidth; }
            set { mapWidth = value; }
        }

        private int mapHeight;
        public int MapHeight
        {
            get { return mapHeight; }
            set { mapHeight = value; }
        }

        private int mapSeed;
        public int MapSeed
        {
            get { return mapSeed; }
            set { mapSeed = value; }
        }

        private bool generatePanelEnabled;
        public bool GeneratePanelEnabled
        {
            get { return generatePanelEnabled; }
            set { generatePanelEnabled = value; }
        }

        private List<EditorToolboxItem> toolboxItems;
        /// <summary>
        /// Collection with static number of items.
        /// </summary>
        public List<EditorToolboxItem> ToolboxItems
        {
            get { return toolboxItems; }
        }

        private EditorToolboxItem selectedToolboxItem;
        public EditorToolboxItem SelectedToolboxItem
        {
            get { return selectedToolboxItem; }
            set { selectedToolboxItem = value; }
        }
        
        /// <summary>
        /// Items from toolbox which were placed on the map.
        /// </summary>
        public ObservableCollection<EditorToolboxItem> PlacedItems{ get; protected set;}

        /// <summary>
        /// Generated map.
        /// </summary>
        public Map GameMap { get; set; }

        /// <summary>
        /// Initializes this model with default values.
        /// </summary>
        public EditorViewModel()
        {
            mapWidth = ViewModelConstants.DEF_MAP_WIDTH;
            mapHeight = ViewModelConstants.DEF_MAP_HEIGHT;
            mapSeed = ViewModelConstants.DEF_MAP_SEED;
            generatePanelEnabled = true;

            selectedToolboxItem = null;
            toolboxItems = CreateToolboxItems();

            PlacedItems = new ObservableCollection<EditorToolboxItem>();
            PlacedItems.Add(new EditorToolboxItem() { Name = "Protihráč", Tooltip = "Umístí protihráče na hrací plochu." });
        }

        /// <summary>
        /// Creates items available in toolbox.
        /// </summary>
        /// <returns>Available toolbox items.</returns>
        private List<EditorToolboxItem> CreateToolboxItems()
        {
            List<EditorToolboxItem> items = new List<EditorToolboxItem>();
            items.Add(new EditorToolboxItem() { Name = "Protihráč", Tooltip = "Umístí protihráče na hrací plochu." });
            items.Add(new EditorToolboxItem() { Name = "Monstrum", Tooltip = "Umístí monstrum na hrací plochu." });
            items.Add(new EditorToolboxItem() { Name = "Zbraň", Tooltip = "Umístí zbraň na hrací plochu." });
            items.Add(new EditorToolboxItem() { Name = "Zbroj", Tooltip = "Umístí zbroj na hrací plochu." });
            items.Add(new EditorToolboxItem() { Name = "Poklad", Tooltip = "Umístí poklad, který může hráč sebrat do inventáře, na hrací plochu." });

            return items;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// Deselects toolbox item.
        /// </summary>
        public void DeselectToolboxItem()
        {
            selectedToolboxItem = null;

        }

        /// <summary>
        /// Generate new map from values stored in this view model.
        /// </summary>
        public void GenerateMap()
        {
            GameMap = MapGeneratorFactory.CreateSimpleMapGenerator().GenerateMap(MapWidth, MapHeight, MapSeed);
        }
    }

    /// <summary>
    /// One item displayed in toolbox - player, monster, item, ...
    /// Contains name, tool tip and icon.
    /// </summary>
    public class EditorToolboxItem
    {
        public String Name { get; set; }
        public String Tooltip { get; set; }
        public BitmapImage Icon { get; set; }
    }
}
