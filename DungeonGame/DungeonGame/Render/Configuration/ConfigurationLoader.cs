﻿using System;
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
        public const string HUMAN_PLAYER_PATH = "M 0.333,1 L 0.5,0.667 L 0.5,0.267 M 0.333,0.5 L 0.5,0.333 L 0.667,0.5 M 0.667,1 L 0.5,0.667 M 0.4,0.167 A 0.1,0.1 1 1 0 0.6,0.167  A 0.1,0.1 1 1 0 0.4,0.167";
        public const string AI_PLAYER_PATH = "M 0.333,1 L 0.5,0.667 L 0.5,0.267 M 0.333,0.5 L 0.5,0.333 L 0.667,0.5 M 0.667,1 L 0.5,0.667 M 0.4,0.167 A 0.1,0.1 1 1 0 0.6,0.167  A 0.1,0.1 1 1 0 0.4,0.167";
        public const string FINAL_ROOM_COLOR = "#c10707";
        public const string ROOM_COLOR = "#000000";
    }

    /// <summary>
    /// Box-style class which holds rendering configuration.
    /// Configuration is stored in dictionary and several properties for accessing the most frequently used configrations are available.
    /// </summary>
    public class RenderConfiguration
    {
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
        /// All configuration this object holds.
        /// </summary>
        public Dictionary<String, Object> AvailableConfiguration { get; protected set; }

        /// <summary>
        /// Returns border color of regular room in string format.
        /// Preferred format is #rrggbb.
        /// </summary>
        public String RoomColor
        {
            get
            {
                if (AvailableConfiguration.ContainsKey(ROOM_COLOR_ID))
                {
                    return (String)AvailableConfiguration[ROOM_COLOR_ID];
                } else
                {
                    return DefaultRenderConfigurationConstatnts.ROOM_COLOR;
                }
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
                if (AvailableConfiguration.ContainsKey(FINAL_ROOM_COLOR_ID))
                {
                    return (String)AvailableConfiguration[FINAL_ROOM_COLOR_ID];
                } else
                {
                    return DefaultRenderConfigurationConstatnts.FINAL_ROOM_COLOR;
                }
            }
        }

        /// <summary>
        /// Vector path in wpf markup syntax for rendering human players.
        /// </summary>
        public String HumanPlayerPath {
            get
            {
                if (AvailableConfiguration.ContainsKey(HUMAN_PLAYER_PATH_ID))
                {
                    return (String)AvailableConfiguration[HUMAN_PLAYER_PATH_ID];
                } else
                {
                    return DefaultRenderConfigurationConstatnts.HUMAN_PLAYER_PATH;
                }
            }
        }

        /// <summary>
        /// Vector path in wpf markup syntax for rendering AI players.
        /// </summary>
        public String AIPLayerPath
        {
            get
            {
                if (AvailableConfiguration.ContainsKey(AI_PLAYER_PATH_ID))
                {
                    return (String)AvailableConfiguration[AI_PLAYER_PATH_ID];
                }
                else
                {
                    return DefaultRenderConfigurationConstatnts.AI_PLAYER_PATH;
                }
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
    }
}
