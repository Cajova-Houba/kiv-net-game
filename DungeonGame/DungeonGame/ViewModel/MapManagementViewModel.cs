﻿using DungeonGame.Common;
using GameCore.Map;
using GameCore.Map.Serializer;
using GameCore.Map.Serializer.Binary;
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
        

        public event PropertyChangedEventHandler PropertyChanged;

        private string fileName;

        /// <summary>
        /// File name which contains currently 'loaded' file (= prepared for import).
        /// It is expected that this field is filled before calling  ImportMapFromCurrentFile().
        /// </summary>
        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                OnPropertyChanged("FileName");
                OnPropertyChanged("MapImportEnabled");
            }
        }

        public bool MapImportEnabled
        {
            get { return FileName != null && FileName.Length > 0; }
        }

        /// <summary>
        /// Colleciton of imported maps. Note that this collection only acts a binding model for list view component,
        /// not as global source of imported maps.
        /// </summary>
        public ObservableCollection<ImportedMapWrapper> ImportedMaps { get; set; }

        public MapManagementViewModel()
        {
            FileName = "";
            ImportedMaps = new ObservableCollection<ImportedMapWrapper>();

            // load maps from global configuraiton to view model
            foreach (ImportedMapWrapper wrapper in GlobalConfiguration.GetInstance().ImportedMaps)
            {
                ImportedMaps.Add(wrapper);
            }
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
            ImportedMaps.Add(new ImportedMapWrapper() { Map = deserializedMap, FilePath = FileName });
            FileName = "";

            OnPropertyChanged("ImportedMaps");
        }

        /// <summary>
        /// Tries to re-load maps from map folder. This will clear all other previously loadded maps.
        /// </summary>
        /// <returns>Number of maps which failed to be imported.</returns>
        public int RefreshImportedMaps()
        {
            string mapsPath = Directory.GetCurrentDirectory() + "\\" + GlobalConfiguration.MAP_FOLDER_NAME;
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

        /// <summary>
        /// Removes imported map.
        /// </summary>
        /// <param name="mapFileName">File path of the map to be removed.</param>
        public void RemoveImportedMap(string mapFileName)
        {
            if (!File.Exists(mapFileName))
            {
                return;
            }

            // remove file
            File.Delete(mapFileName);

            // remove it from imported maps collection
            foreach(ImportedMapWrapper map in ImportedMaps)
            {
                if (map.FilePath.Equals(mapFileName))
                {
                    ImportedMaps.Remove(map);
                    break;
                }
            }
            OnPropertyChanged("ImportedMaps");
        }

        /// <summary>
        /// Clears collection of imported maps in global configuration and adds imported maps from this view model.
        /// </summary>
        public void FlushImportedMapsToGlobalConfiguration()
        {
            GlobalConfiguration.GetInstance().ImportedMaps.Clear();
            foreach(ImportedMapWrapper importedMap in ImportedMaps)
            {
                GlobalConfiguration.GetInstance().ImportedMaps.Add(importedMap);
            }
        }

    }
}
