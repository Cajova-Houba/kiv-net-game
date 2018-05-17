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
using System.IO;

namespace DungeonGame.ViewModel
{
    /// <summary>
    /// View model for editor window.
    /// </summary>
    public class EditorViewModel : INotifyPropertyChanged
    {

        public const string PLAYER_IMAGE_FILE_NAME = "human-player.png";
        public const string AI_PLAYER_IMAGE_FILE_NAME = "ai-player.png";
        public const string MONSTER_IMAGE_FILE_NAME = "monster.png";
        public const string WEAPON_IMAGE_FILE_NAME = "weapon.png";
        public const string ARMOR_IMAGE_FILE_NAME = "armor.png";
        public const string ITEM_IMAGE_FILE_NAME = "item.png";
        public const string UNKNOWN_FILE_NAME = "unknown.png";

        public event PropertyChangedEventHandler PropertyChanged;
        
        public string MapName { get; set; }
        
        public int MapWidth { get; set; }
        
        public int MapHeight { get; set; }
        
        public int MapSeed { get; set; }
        
        public bool GeneratePanelEnabled { get; set; }

        public bool HumanPlayerPlaced { get; protected set; }

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
            MapWidth = ViewModelConstants.DEF_MAP_WIDTH;
            MapHeight = ViewModelConstants.DEF_MAP_HEIGHT;
            MapSeed = ViewModelConstants.DEF_MAP_SEED;
            GeneratePanelEnabled = true;
            MapName = "Nová mapa";
            HumanPlayerPlaced = false;

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
            items.Add(new EditorToolboxItem() { Name = "Hráč", Tooltip = "Umístí hráče na hrací plochu.", ItemType = EditorToolboxItemType.HUMAN_PLAYER });
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
        /// Load map to this view model.
        /// </summary>
        /// <param name="map">Map to be loaded.</param>
        public void LoadMap(Map map)
        {
            // try to load all map items and if it's ok, place it, otherwise
            // throw exception, this way the old map will still remain in editor
            List<EditorToolboxItem> tmpPlacedItems = new List<EditorToolboxItem>();
            foreach(MapBlock mb in map.Grid)
            {
                if (mb.Creature != null)
                {
                    EditorToolboxItemType type;
                    if (mb.Creature is Monster) { type = EditorToolboxItemType.MONSTER; }
                    else if (mb.Creature is HumanPlayer) { type = EditorToolboxItemType.HUMAN_PLAYER; }
                    else if (mb.Creature is AbstractPlayer) { type = EditorToolboxItemType.AI_PLAYER; }
                    else
                    {
                        throw new Exception($"Neznámý typ bytosti {mb.Creature.GetType()}!");
                    }
                    tmpPlacedItems.Add(new EditorToolboxItem() { UID = mb.Creature.UniqueId, Name = mb.Creature.Name, ItemType = type });
                }

                if (mb.Item != null)
                {
                    EditorToolboxItemType type;
                    if (mb.Item is AbstractWeapon) { type = EditorToolboxItemType.WEAPON; }
                    else if (mb.Item is AbstractArmor) { type = EditorToolboxItemType.ARMOR; }
                    else if (mb.Item is AbstractInventoryItem) { type = EditorToolboxItemType.ITEM; }
                    else
                    {
                        throw new Exception($"Neznámý typ předmětu {mb.Item.GetType()}!");
                    }
                    tmpPlacedItems.Add(new EditorToolboxItem() { UID = mb.Item.UniqueId, Name = mb.Item.Name, ItemType = type });
                }
            }

            PlacedItems.Clear();
            tmpPlacedItems.ForEach(placedItem => PlacedItems.Add(placedItem));
            GameMap = map;
            MapWidth = map.Width;
            MapHeight = map.Height;
            MapName = map.MapName;
            GeneratePanelEnabled = true;
            DeselectToolboxItem();

            OnPropertyChanged("GeneratePanelEnabled");
            OnPropertyChanged("PlacedItems");
            OnPropertyChanged("MapWidth");
            OnPropertyChanged("Mapheight");
            OnPropertyChanged("MapName");
        }

        /// <summary>
        /// Generate new map from values stored in this view model.
        /// Note that this will also remove all placed items.
        /// </summary>
        public void GenerateMap()
        {
            GameMap = MapGeneratorFactory.CreateSimpleMapGenerator().GenerateMap(MapWidth, MapHeight, MapSeed);
            PlacedItems.Clear();
            GameMap.MapName = MapName;
            HumanPlayerPlaced = false;
            OnPropertyChanged("PlacedItems");
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

            int uid = -1;
            switch(item.ItemType)
            {
                case EditorToolboxItemType.HUMAN_PLAYER:
                    if (HumanPlayerPlaced)
                    {
                        throw new Exception("Na hrací plochu lze umístit pouze jednoho lidského hráče!");
                    }
                    GameMap.Grid[mapX, mapY].Creature = new HumanPlayer("Human player", GameMap.Grid[mapX, mapY]);
                    uid = GameMap.Grid[mapX, mapY].Creature.UniqueId;
                    HumanPlayerPlaced = true;
                    break;

                case EditorToolboxItemType.AI_PLAYER:
                    GameMap.Grid[mapX, mapY].Creature = AIPlayerFactory.CreateSimpleAIPLayer("Simple AI Player", GameMap.Grid[mapX, mapY]);
                    uid = GameMap.Grid[mapX, mapY].Creature.UniqueId;
                    break;

                case EditorToolboxItemType.MONSTER:
                    GameMap.Grid[mapX, mapY].Creature = MonsterFactory.CreateRandomMonster(GameMap.Grid[mapX, mapY]);
                    uid = GameMap.Grid[mapX, mapY].Creature.UniqueId;
                    break;

                case EditorToolboxItemType.ITEM:
                    GameMap.Grid[mapX, mapY].Item = ItemFactory.CreateBasicItem(GameMap.Grid[mapX, mapY]);
                    uid = GameMap.Grid[mapX, mapY].Item.UniqueId;
                    break;

                case EditorToolboxItemType.ARMOR:
                    GameMap.Grid[mapX, mapY].Item = ItemFactory.CreateLeatherArmor(GameMap.Grid[mapX, mapY]);
                    uid = GameMap.Grid[mapX, mapY].Item.UniqueId;
                    break;

                case EditorToolboxItemType.WEAPON:
                    GameMap.Grid[mapX, mapY].Item = ItemFactory.CreateAxe(GameMap.Grid[mapX, mapY]);
                    uid = GameMap.Grid[mapX, mapY].Item.UniqueId;
                    break;

                default:
                    throw new Exception($"Neznámý typ umisťovaného předmětu: {item.ItemType}!");
            }
            EditorToolboxItem placedItem = item.Clone();
            placedItem.UID = uid;
            PlacedItems.Add(placedItem);

            OnPropertyChanged("PlacedItems");
        }

        /// <summary>
        /// Removes placed item from the map.
        /// </summary>
        /// <param name="uid">Uid of item to be removed.</param>
        public void RemovePlacedItem(int uid)
        {
            GameMap?.RemoveObjectFromMapByUid(uid);
            foreach(EditorToolboxItem item in PlacedItems)
            {
                if (item.UID == uid)
                {
                    PlacedItems.Remove(item);
                    if (item.ItemType == EditorToolboxItemType.HUMAN_PLAYER)
                    {
                        HumanPlayerPlaced = false;
                    }
                    break;
                }
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

        /// <summary>
        /// UID of placed item. Used when removing placed items from map.
        /// </summary>
        public int UID { get; set; }
        public String Name { get; set; }
        public String Tooltip { get; set; }
        public BitmapImage Icon { get; set; }
        private EditorToolboxItemType itemType;
        public EditorToolboxItemType ItemType
        {
            get { return itemType; }
            set
            {
                itemType = value;
                Icon = LoadIconByItemType(itemType);
            }
        }

        public EditorToolboxItem Clone()
        {
            return new EditorToolboxItem() { Name = this.Name, Tooltip = this.Tooltip, Icon = this.Icon, ItemType = this.ItemType };
        }

        /// <summary>
        /// Loads icon for editor toolbox item by type.
        /// </summary>
        /// <param name="type">Item type.</param>
        /// <returns>Icon.</returns>
        private BitmapImage LoadIconByItemType(EditorToolboxItemType type)
        {
            BitmapImage image = null;
            string fName = Directory.GetCurrentDirectory() + "\\img\\";

            switch (type)
            {
                case EditorToolboxItemType.HUMAN_PLAYER:
                    fName += EditorViewModel.PLAYER_IMAGE_FILE_NAME;
                    break;

                case EditorToolboxItemType.AI_PLAYER:
                    fName += EditorViewModel.AI_PLAYER_IMAGE_FILE_NAME;
                    break;

                case EditorToolboxItemType.MONSTER:
                    fName += EditorViewModel.MONSTER_IMAGE_FILE_NAME;
                    break;

                case EditorToolboxItemType.WEAPON:
                    fName += EditorViewModel.WEAPON_IMAGE_FILE_NAME;
                    break;

                case EditorToolboxItemType.ARMOR:
                    fName += EditorViewModel.ARMOR_IMAGE_FILE_NAME;
                    break;

                case EditorToolboxItemType.ITEM:
                    fName += EditorViewModel.ITEM_IMAGE_FILE_NAME;
                    break;

                default:
                    fName += EditorViewModel.UNKNOWN_FILE_NAME;
                    break;
            }

            if (File.Exists(fName))
            {
                try
                {
                    image = new BitmapImage(new Uri(fName));
                } catch (Exception ex)
                {
                    // do nothing if the image is corrupted, or just bad
                    // is someone messes with this, let him have it
                    return null;
                }
            }

            return image;

        }
    }

    /// <summary>
    /// Possible types of items (or rather entities) which can be placed on the map.
    /// </summary>
    public enum EditorToolboxItemType
    {
        AI_PLAYER,
        HUMAN_PLAYER,
        MONSTER,
        ITEM,
        ARMOR,
        WEAPON
    }
}
