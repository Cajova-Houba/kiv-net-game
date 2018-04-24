using GameCore.Map;
using GameCore.Map.Generator;
using GameCore.Objects.Creatures;
using GameCore.Objects.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Model
{
    /// <summary>
    /// View model for game related things.
    /// </summary>
    public class GameViewModel
    {
        public const int MAP_SEED = 123456;
        
        /// <summary>
        /// Returns game messages to be displayed in bottom panel.
        /// </summary>
        public ObservableCollection<String> GameMessages
        {
            get
            {
                return new ObservableCollection<string>(new String[] {
                    "Enjoy!",
                    "Game started.",
                    "Players generated.",
                    "Map generated.",
                    "New game initialization."
                });
            }
        }
        
        /// <summary>
        /// Returns player's inventory.
        /// </summary>
        public ObservableCollection<InventoryItemModel> Inventory { get; protected set; }
        
        /// <summary>
        /// Returns map of the current game.
        /// </summary>
        public Map GameMap { get; protected set; }
        
        /// <summary>
        /// Returns instance of current player.
        /// </summary>
        public AbstractPlayer Player { get; protected set; }

        public GameViewModel()
        {
            // game initialization will be somehow placed here
            GameMap = MapGeneratorFactory.CreateSimpleMapGenerator().GenerateMap(20, 20, MAP_SEED);
            Player = new HumanPlayer("Test player", GameMap.Grid[2,2]);
            Player.Inventory.AddRange(new BasicItem[]
            {
                new BasicItem("Test item", new MapBlock(), 10),
                new BasicItem("Test item 2", new MapBlock(), 10),
                new BasicItem("Some cool item", new MapBlock(), 10)
            });
            Inventory = new ObservableCollection<InventoryItemModel>();
            foreach(AbstractInventoryItem invItem in Player.Inventory)
            {
                Inventory.Add(new InventoryItemModel() { ModelObject = invItem });
            }
        }
    }
}
