using GameCore.Map;
using GameCore.Map.Serializer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.ViewModel
{
    /// <summary>
    /// View model for map management window.
    /// </summary>
    public class MapManagementViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// This folder which contains imported maps is expected to be in the same directory as executable.
        /// </summary>
        public const string MAP_FOLDER_NAME = "maps";

        public event PropertyChangedEventHandler PropertyChanged;

        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                OnPropertyChanged("FileName");
            }
        }

        public bool MapImportEnabled
        {
            get { return FileName != null && FileName.Length > 0; }
        }

        public ObservableCollection<Map> ImportedMaps { get; set; }

        public MapManagementViewModel()
        {
            FileName = "";
            ImportedMaps = new ObservableCollection<Map>();
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
        /// Imports map from file specified by FileName (property of this model).
        /// If no FileName is provided or file doesn't exist, exception is thrown.
        /// </summary>
        public void ImportMapFromCurrentFile()
        {
            if (!MapImportEnabled)
            {
                throw new Exception("Není vybrán žádný soubor.");
            }

            if (!File.Exists(FileName))
            {
                throw new Exception($"Soubor {FileName} nebyl nalezen!");
            }

            byte[] fileContent;
            fileContent = File.ReadAllBytes(FileName);
            Map deserializedMap = new BinaryMapSerializer().Deserialize(fileContent);
            ImportedMaps.Add(deserializedMap);

            OnPropertyChanged("ImportedMaps");
        }

        /// <summary>
        /// Tries to re-load maps from map folder. This will clear all other previously loadded maps.
        /// </summary>
        /// <returns>Number of maps which failed to be imported.</returns>
        public int RefreshImportedMaps()
        {
            string mapsPath = Directory.GetCurrentDirectory() + "\\" + MAP_FOLDER_NAME;
            if (!Directory.Exists(mapsPath))
            {
                throw new Exception($"Složka {mapsPath} obsahující importované mapy nenalezena!");
            }

            // keep the original filename in case user does load file -> refresh -> import map
            string origFileName = FileName;                                 
            ImportedMaps.Clear();
            int failedMaps = 0;
            foreach(string filePath in Directory.GetFiles(mapsPath))
            {
                try
                {
                    // no need to fire property changed event
                    fileName = filePath;
                    ImportMapFromCurrentFile();
                } catch (Exception ex)
                {
                    failedMaps++;
                }
            }
            fileName = origFileName;
            OnPropertyChanged("ImportedMaps");

            return failedMaps;
        }
    }
}
