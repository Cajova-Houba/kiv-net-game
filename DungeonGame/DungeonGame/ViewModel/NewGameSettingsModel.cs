using DungeonGame.Common;
using GameCore.Map.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.ViewModel
{
    /// <summary>
    /// View model for new game settings window.
    /// </summary>
    public class NewGameSettingsModel
    {
        public const int MAX_DENSITY = 100;
        public const int MIN_DENSITY = 0;
        public const int DEF_DENSITY = 50;
        public const int DEF_MONSTER_COUNT = 10;
        public const int DEF_AI_COUNT = 3;
        public const string DEF_PLAYER_NAME = "Dobrodruh #5";

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
        public int MapSeed {
            get { return mapSeed; }
            set { mapSeed = value; }
        }

        private int aiCount;
        public int AiCount
        {
            get { return aiCount; }
            set { aiCount = value; }
        }

        public int MaxDensity { get { return MAX_DENSITY; } }
        public int MinDensity { get { return MIN_DENSITY; } }

        // densities should be in range MIN_DENSITY..MAX_DENSITY
        public int MonsterDensity { get; set; }
        public int ItemDensity { get; set; }

        /// <summary>
        /// Returns monster density normalized to range 1..0.
        /// </summary>
        public double NormalizedMonsterDensity
        {
            get { return MonsterDensity / (double)MAX_DENSITY; }
        }

        /// <summary>
        /// Returns item density normalized to range 1..0
        /// </summary>
        public double NormalizedItemDensity
        {
            get { return ItemDensity / (double)MAX_DENSITY; }
        }

        private string playerName;
        public string PlayerName
        {
            get { return playerName; }
            set { playerName = value; }
        }

        public ImportedMapWrapper SelectedImportedMap { get; set; }

        public List<ImportedMapWrapper> ImportedMaps {
            get { return GlobalConfiguration.GetInstance().ImportedMaps; }
        }

        /// <summary>
        /// Initializes this view model with default values.
        /// </summary>
        public NewGameSettingsModel()
        {
            mapWidth = ViewModelConstants.DEF_MAP_WIDTH;
            mapHeight = ViewModelConstants.DEF_MAP_HEIGHT;
            mapSeed = ViewModelConstants.DEF_MAP_SEED;
            aiCount = DEF_AI_COUNT;
            MonsterDensity = DEF_DENSITY;
            ItemDensity = DEF_DENSITY;
            playerName = DEF_PLAYER_NAME;
        }
    }

    /// <summary>
    /// Class used as model for item density combobox.
    /// </summary>
    public class ItemsDensity
    {
        public int Value { get; set; }
        public String Name { get; set; }
    }
}
