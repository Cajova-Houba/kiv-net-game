using GameCore.Map;
using GameCore.Map.Serializer;
using GameCore.Map.Serializer.Binary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Common
{
    /// <summary>
    /// Singleton class used to hold global configuration of the whole application. Currently, only imported maps are here.
    /// Instance of this class should be retrieved at the beginning of the app so that configuration is loaded immediately after
    /// app starts.
    /// </summary>
    public class GlobalConfiguration
    {
        public static GlobalConfiguration GetInstance()
        {
            if (instance == null)
            {
                instance = new GlobalConfiguration();
            }

            return instance;
        }
        private static GlobalConfiguration instance;

        /// This folder which contains imported maps is expected to be in the same directory as executable.
        /// </summary>
        public const string MAP_FOLDER_NAME = "maps";

        private List<ImportedMapWrapper> importedMaps;
        /// <summary>
        /// Hold imported maps. Is never null.
        /// </summary>
        public List<ImportedMapWrapper> ImportedMaps {
            get { return importedMaps; }
            set
            {
                if (value == null)
                {
                    importedMaps = new List<ImportedMapWrapper>();
                } else
                {
                    importedMaps = value;
                }
            }
        }

        /// <summary>
        /// Number of files which failed to be processed when loading maps.
        /// </summary>
        public int FailedMaps { get; protected set; }

        private GlobalConfiguration()
        {
            ImportedMaps = new List<ImportedMapWrapper>();
            LoadMaps();
        }

        /// <summary>
        /// Tries to load maps from MAP_FOLDER_NAME directory which is supposed so be in the same place as executable.
        /// </summary>
        private void LoadMaps()
        {
            string mapsPath = Directory.GetCurrentDirectory() + "\\" + GlobalConfiguration.MAP_FOLDER_NAME;
            if (!Directory.Exists(mapsPath))
            {
                return;
            }

            ImportedMaps.Clear();
            int failedMaps = 0;
            foreach (string filePath in Directory.GetFiles(mapsPath))
            {
                if(!ImportMapFromFile(filePath))
                {
                    failedMaps++;
                }
            }

            FailedMaps = failedMaps;
        }

        /// <summary>
        /// Imports map from file specified by FileName (property of this model).
        /// Ef error occurs, returns false, otherwise true.
        /// </summary>
        /// <returns>True if map was imported.</returns>
        private bool ImportMapFromFile(string fileName)
        {

            if (fileName == null || !File.Exists(fileName))
            {
                return false;
            }

            try
            {
                byte[] fileContent;
                fileContent = File.ReadAllBytes(fileName);
                Map deserializedMap = new BinaryMapSerializer().Deserialize(fileContent);
                ImportedMaps.Add(new ImportedMapWrapper() { Map = deserializedMap, FilePath = fileName });
                return true;
            } catch (Exception ex)
            {
                return false;
            }
        }

    }

    /// <summary>
    /// Simple wrapper for imported maps.
    /// </summary>
    public class ImportedMapWrapper
    {
        public Map Map { get; set; }
        public string FilePath { get; set; }
    }
}
