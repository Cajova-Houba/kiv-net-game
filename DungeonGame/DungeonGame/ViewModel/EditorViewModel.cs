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
using GameCore.Objects.Creatures;
using GameCore.Objects.Creatures.AIPlayers;
using GameCore.Objects.Items;

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
        
        public EditorToolboxItem SelectedToolboxItem { get; set; }

        public int SelectedToolboxItemIndex { get; set; }
        
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

            SelectedToolboxItem = null;
            SelectedToolboxItemIndex = -1;
            toolboxItems = CreateToolboxItems();

            PlacedItems = new ObservableCollection<EditorToolboxItem>();
        }

        /// <summary>
        /// Creates items available in toolbox.
        /// </summary>
        /// <returns>Available toolbox items.</returns>
        private List<EditorToolboxItem> CreateToolboxItems()
        {
            List<EditorToolboxItem> items = new List<EditorToolboxItem>();
            items.Add(new EditorToolboxItem() { Name = "Protihráč", Tooltip = "Umístí protihráče na hrací plochu.", ItemType = EditorToolboxItemType.AI_PLAYER});
            items.Add(new EditorToolboxItem() { Name = "Monstrum", Tooltip = "Umístí monstrum na hrací plochu.", ItemType = EditorToolboxItemType.MONSTER });
            items.Add(new EditorToolboxItem() { Name = "Zbraň", Tooltip = "Umístí zbraň na hrací plochu.", ItemType = EditorToolboxItemType.WEAPON });
            items.Add(new EditorToolboxItem() { Name = "Zbroj", Tooltip = "Umístí zbroj na hrací plochu.", ItemType = EditorToolboxItemType.ARMOR });
            items.Add(new EditorToolboxItem() { Name = "Poklad", Tooltip = "Umístí poklad, který může hráč sebrat do inventáře, na hrací plochu.", ItemType = EditorToolboxItemType.ITEM });

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
            SelectedToolboxItem = null;
            SelectedToolboxItemIndex = -1;

            OnPropertyChanged("SelectedToolboxItem");
            OnPropertyChanged("SelectedToolboxItemIndex");
        }

        /// <summary>
        /// Generate new map from values stored in this view model.
        /// </summary>
        public void GenerateMap()
        {
            GameMap = MapGeneratorFactory.CreateSimpleMapGenerator().GenerateMap(MapWidth, MapHeight, MapSeed);
        }

        /// <summary>
        /// Places SelectedToolboxItem to the map. If no item is selected, nothing happnes.
        /// 
        /// If the map block is occupied, coordinates are out of range, map is null or item type is unknown, exception is raised.
        /// 
        /// </summary>
        /// <param name="mapX">X coordinate of map block to place this item.</param>
        /// <param name="mapY">Y coordinate of map block to place this item.</param>
        public void PlaceSelectedToolboxItem(int mapX, int mapY)
        {
            EditorToolboxItem item = SelectedToolboxItem;
            // no item selected => do nothing
            if (item == null)
            {
                return;
            }

            if (GameMap == null)
            {
                throw new Exception("Není herní mapa!");
            } else if (mapX < 0 || mapX >= GameMap.Width || mapY < 0 || mapY >= GameMap.Height)
            {
                throw new Exception($"[{mapX},{mapY}] není v rozsahu mapy!");
            } else if ((item.ItemType == EditorToolboxItemType.AI_PLAYER || item.ItemType == EditorToolboxItemType.MONSTER) && GameMap.Grid[mapX, mapY].Occupied)
            {
                throw new Exception($"Na pozici [{mapX},{mapY}] už je umístěn hráč, nebo monstrum!");
            } else if ((item.ItemType == EditorToolboxItemType.ITEM || item.ItemType == EditorToolboxItemType.ARMOR || item.ItemType == EditorToolboxItemType.WEAPON) && GameMap.Grid[mapX, mapY].Item != null)
            {
                throw new Exception($"Na pozici [{mapX},{mapY}] už je umístěn předmět!");
            }

            PlacedItems.Add(item.Clone());
            switch(item.ItemType)
            {
                case EditorToolboxItemType.AI_PLAYER:
                    GameMap.Grid[mapX, mapY].Creature = AIPlayerFactory.CreateSimpleAIPLayer("Simple AI Player", GameMap.Grid[mapX, mapY]);
                    break;

                case EditorToolboxItemType.MONSTER:
                    GameMap.Grid[mapX, mapY].Creature = MonsterFactory.CreateRandomMonster(GameMap.Grid[mapX, mapY]);
                    break;

                case EditorToolboxItemType.ITEM:
                    GameMap.Grid[mapX, mapY].Item = ItemFactory.CreateBasicItem(GameMap.Grid[mapX, mapY]);
                    break;

                case EditorToolboxItemType.ARMOR:
                    GameMap.Grid[mapX, mapY].Item = ItemFactory.CreateLeatherArmor(GameMap.Grid[mapX, mapY]);
                    break;

                case EditorToolboxItemType.WEAPON:
                    GameMap.Grid[mapX, mapY].Item = ItemFactory.CreateAxe(GameMap.Grid[mapX, mapY]);
                    break;

                default:
                    throw new Exception($"Neznámý typ umisťovaného předmětu: {item.ItemType}!");
            }

            OnPropertyChanged("PlacedItems");
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
        public EditorToolboxItemType ItemType { get; set; }

        public EditorToolboxItem Clone()
        {
            return new EditorToolboxItem() { Name = this.Name, Tooltip = this.Tooltip, Icon = this.Icon, ItemType = this.ItemType };
        }
    }

    /// <summary>
    /// Possible types of items (or rather entities) which can be placed on the map.
    /// </summary>
    public enum EditorToolboxItemType
    {
        AI_PLAYER,
        MONSTER,
        ITEM,
        ARMOR,
        WEAPON
    }
}
