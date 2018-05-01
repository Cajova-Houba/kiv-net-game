using GameCore.Game;
using GameCore.Game.Actions;
using GameCore.Map;
using GameCore.Map.Generator;
using GameCore.Objects.Creatures;
using GameCore.Objects.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Model 
{
    /// <summary>
    /// View model for game related things.
    /// </summary>
    public class GameViewModel : INotifyPropertyChanged
    {
        public const int MAP_SEED = 123456;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public Game GameInstance { get; protected set; }

        /// <summary>
        /// Initializes this view model with game instance.
        /// </summary>
        /// <param name="game">Game instance.</param>
        public GameViewModel(Game game)
        {
            GameMap = game.GameMap;
            if (game.HumanPlayers.Count > 0)
            {
                Player = game.HumanPlayers[0];
                Inventory = new ObservableCollection<InventoryItemModel>();
                foreach (AbstractInventoryItem invItem in Player.Inventory)
                {
                    Inventory.Add(new InventoryItemModel() { ModelObject = invItem });
                }
            }
            GameInstance = game;

        }

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

        /// <summary>
        /// Send move action for player to the game core.
        /// </summary>
        /// <param name="direction">Direction of move.</param>
        public void Move(Direction direction)
        {
            Player.NextAction = new Move() { Actor = Player, Direction = direction };
        }

        /// <summary>
        /// Perform game loop step and call property updated.
        /// </summary>
        public void GameLoopStep()
        {
            GameInstance?.GameLoopStep();
            OnPropertyChanged("Player.Position");
            OnPropertyChanged("Inventory");
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
