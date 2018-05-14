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
        public const int DEF_MAP_WIDTH = 10;
        public const int DEF_MAP_HEIGHT = 10;
        public const int DEF_MAP_SEED = IMapGeneratorConstants.NO_SEED;
        public const int DEF_AI_COUNT = 3;
        public const int DEF_MONSTER_COUNT = 10;
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

        private int monsterCount;
        public int MonsterCount
        {
            get { return monsterCount; }
            set { monsterCount = value; }
        }

        private List<ItemsDensity> itemsDensities;
        public List<ItemsDensity> ItemsDensities
        {
            get { return itemsDensities; }
        }

        private ItemsDensity selectedDensity;
        public ItemsDensity SelectedDensity
        {
            get { return selectedDensity; }
            set { selectedDensity = value; }
        }

        private string playerName;
        public string PlayerName
        {
            get { return playerName; }
            set { playerName = value; }
        }

        /// <summary>
        /// Initializes this view model with default values.
        /// </summary>
        public NewGameSettingsModel()
        {
            mapWidth = DEF_MAP_WIDTH;
            mapHeight = DEF_MAP_HEIGHT;
            mapSeed = DEF_MAP_SEED;
            aiCount = DEF_AI_COUNT;
            monsterCount = DEF_MONSTER_COUNT;
            playerName = DEF_PLAYER_NAME;

            itemsDensities = new List<ItemsDensity>();
            itemsDensities.Add(new ItemsDensity() { Value = 3, Name = "Málo" });
            itemsDensities.Add(new ItemsDensity() { Value = 2, Name = "Středně" });
            itemsDensities.Add(new ItemsDensity() { Value = 1, Name = "Hodně" });
            selectedDensity = itemsDensities[1];
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
