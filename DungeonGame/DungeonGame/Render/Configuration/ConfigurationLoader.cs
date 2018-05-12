using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Render.Configuration
{
    /// <summary>
    /// Library class for loading configuration from files.
    /// </summary>
    public class ConfigurationLoader
    {

        /// <summary>
        /// Loads configuration from file and returns it as an object.
        /// </summary>
        /// <param name="pathToFile">Path to file to load configuration from. If such file is not found, empty configuration is returned.</param>
        /// <returns></returns>
        public static RenderConfiguration LoadConfiguration(string pathToFile)
        {
            if (!File.Exists(pathToFile))
            {
                return new RenderConfiguration();
            }

            Dictionary<String, object> data = new Dictionary<string, object>();
            foreach (var row in File.ReadAllLines(pathToFile))
            {
                // skip empty lines or lines starting with '#'
                if (row == null || row.Count() == 0 || row[0] == '#')
                {
                    continue;
                }
                data.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
            }

            return new RenderConfiguration(data);
        }
    }

    /// <summary>
    /// Class which holds default render configuration.
    /// If for some reasong loading configuration from file fails, values from this class are returned
    /// so that renderer has always something to work with.
    /// </summary>
    class DefaultRenderConfigurationConstatnts
    {
        // nice, isn't it?
        public const string HUMAN_PLAYER_PATH = "M 0.333,1 L 0.5,0.667 L 0.5,0.3 M 0.333,0.5 L 0.5,0.333 L 0.667,0.5 M 0.667,1 L 0.5,0.667 M 0.4,0.21 A 0.1,0.1 1 1 0 0.6,0.21  A 0.1,0.1 1 1 0 0.4,0.21";
        public const string AI_PLAYER_PATH = "M 0.333,1 L 0.5,0.667 L 0.5,0.3 M 0.333,0.5 L 0.5,0.333 L 0.667,0.5 M 0.667,1 L 0.5,0.667 M 0.4,0.21 A 0.1,0.1 1 1 0 0.6,0.21  A 0.1,0.1 1 1 0 0.4,0.21";
        public const string MONSTER_PATH = "M 0.33,0.83 A 0.167,0.167 1 1 0 0.667,0.83 A 0.167,0.167 1 1 0 0.333,0.83 M 0.33,0.80 L 0.416,0.50 L 0.50,0.67 L 0.583,0.50 L 0.66,0.80 M 0.43,0.83 L 0.567,0.83";
        public const string ARMOR_PATH = "M 0.25,0.20 L 0.75,0.20 L 0.75,0.60 L 0.50,0.80 L 0.25,0.60 L 0.25,0.20 M 0.25,0.40 L 0.75,0.40 M 0.50,0.20 L 0.50,0.80";
        public const string WEAPON_PATH = "M 0.50, 0.875 L 0.50,0.125 M 0.375,0.125 A 0.125,0.125 1 1 0 0.625,0.125 M 0.375,0.125 A 0.25,0.25 1 0 0 0.50,0.50 A 0.25,0.25 1 0 0 0.625,0.125";
        public const string ITEM_PATH = "M 0.25,0.25 L 0.125,0.50 L 0.125,0.90 L 0.875,0.90 L 0.875,0.50 L 0.75,0.25 L 0.25,0.25 M 0.125,0.50 L 0.875,0.50 M 0.45,0.50 L 0.45,0.65 L 0.55,0.65 L 0.55,0.50";
        public const string FINAL_ROOM_COLOR = "#c10707";
        public const string ROOM_COLOR = "#000000";
    }

    /// <summary>
    /// Box-style class which holds rendering configuration.
    /// Configuration is stored in dictionary and several properties for accessing the most frequently used configrations are available.
    /// </summary>
    public class RenderConfiguration
    {
        public const string MONSTER_PATH = "monster_path";

        /// <summary>
        /// Unique id to identify human player path property in configuration dictionary.
        /// </summary>
        public const string HUMAN_PLAYER_PATH_ID = "human_player_path";

        /// <summary>
        /// Unique id to identify ai player path property in configuration dictionary.
        /// </summary>
        public const string AI_PLAYER_PATH_ID = "ai_player_path";

        /// <summary>
        /// Unique id to identify border color of the final room property in configuration dictionary.
        /// </summary>
        public const string FINAL_ROOM_COLOR_ID = "final_room_color";

        /// <summary>
        /// Unique id to identify border color ofregular room in configuration dictionary.
        /// </summary>
        public const string ROOM_COLOR_ID = "room_color";

        /// <summary>
        /// Unique id to identify armor path in configuration dictionary.
        /// </summary>
        public const string ARMOR_PATH_ID = "shield_path";

        /// <summary>
        /// Unique id to identify weapon path in configuration directory.
        /// </summary>
        public const string WEAPON_PATH_ID = "weapon_path";

        /// <summary>
        /// Unique id to identify item path in configuration directory.
        /// </summary>
        public const string ITEM_PATH_ID = "item_path";

        /// <summary>
        /// All configuration this object holds.
        /// </summary>
        public Dictionary<String, Object> AvailableConfiguration { get; protected set; }

        public String ArmorPath
        {
            get
            {
                return (String)TryGetConfiguration(ARMOR_PATH_ID, DefaultRenderConfigurationConstatnts.ARMOR_PATH);
            }
        }

        /// <summary>
        /// Returns path for rendering weapon.
        /// </summary>
        public String WeaponPath
        {
            get
            {
                return (String)TryGetConfiguration(WEAPON_PATH_ID, DefaultRenderConfigurationConstatnts.WEAPON_PATH);
            }
        }

        /// <summary>
        /// Returns path for rendering items.
        /// </summary>
        public String ItemPath
        {
            get
            {
                return (String)TryGetConfiguration(ITEM_PATH_ID, DefaultRenderConfigurationConstatnts.ITEM_PATH);
            }
        }

        /// <summary>
        /// Returns border color of regular room in string format.
        /// Preferred format is #rrggbb.
        /// </summary>
        public String RoomColor
        {
            get
            {
                return (String)TryGetConfiguration(ROOM_COLOR_ID, DefaultRenderConfigurationConstatnts.ROOM_COLOR);
            }
        }

        /// <summary>
        /// Returns the final room color in string format.
        /// Preferred format is #rrggbb.
        /// </summary>
        public String FinalRoomColor
        {
            get
            {
                return (String)TryGetConfiguration(FINAL_ROOM_COLOR_ID, DefaultRenderConfigurationConstatnts.FINAL_ROOM_COLOR);
            }
        }

        /// <summary>
        /// Vector path in wpf markup syntax for rendering human players.
        /// </summary>
        public String HumanPlayerPath {
            get
            {
                return (String)TryGetConfiguration(HUMAN_PLAYER_PATH_ID, DefaultRenderConfigurationConstatnts.HUMAN_PLAYER_PATH);
            }
        }

        /// <summary>
        /// Vector path in wpf markup syntax for rendering AI players.
        /// </summary>
        public String AIPLayerPath
        {
            get
            {
                return (String)TryGetConfiguration(AI_PLAYER_PATH_ID, DefaultRenderConfigurationConstatnts.AI_PLAYER_PATH);
            }
        }

        public String MonsterPath
        {
            get
            {
                return (String)TryGetConfiguration(MONSTER_PATH, DefaultRenderConfigurationConstatnts.MONSTER_PATH);
            }
        }
        
        /// <summary>
        /// Initializes this configuration object.
        /// </summary>
        /// <param name="configuration">Configuration dictionary.</param>
        public RenderConfiguration(Dictionary<String, object> configuration)
        {
            AvailableConfiguration = configuration;
        }

        /// <summary>
        /// Initializes this configuration with empty configuration dictionary.
        /// </summary>
        public RenderConfiguration() : this(new Dictionary<string, object>())
        {

        }

        /// <summary>
        /// Checks if the configuration contains configuration item with given key and returns it, if it exists. Otherwise returns default value.
        /// </summary>
        /// <param name="configurationKey">Key for configuration item.</param>
        /// <returns>Configuration for given key or default value if it doesn't exist.</returns>
        private Object TryGetConfiguration(string configurationKey, Object defaultValue)
        {
            if (AvailableConfiguration.ContainsKey(configurationKey))
            {
                return AvailableConfiguration[configurationKey];
            } else
            {
                return defaultValue;
            }
        }
    }
}
